using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Metrics;
using TacticsGame.Items.SpecialStats;

namespace TacticsGame.AI.MaintenanceMode.Engines
{
    public class EducatedPreferenceEngine : IPreferenceEngine
    {
        /// <summary>
        /// How willing the unit is to part with the item.
        /// </summary>
        public virtual int DetermineWillingnessToSellItem(IMakeDecisions unit, Item item)
        {
            int propensity = 0;
            int itemCount = unit.Inventory.GetItemCount(item);           
            
            propensity -= unit.Preferences.ItemPreference.GetPreference(item); // Want to keep preferred items
            propensity += Utilities.GetRandomNumber(0, 10); // Randomness of decision for all other things equal

            propensity += (itemCount - 1) * 10; // Additional 10% chance per extra item

            //propensity += itemCount == 0 ? 0 : (int)((double)((itemCount - 1) * 10) * Math.Log10(((itemCount) * 10))); // For every item over 1, steady increase in willingness to sell             
            //propensity += ((int)Rarity.Artifact - (int)item.Stats.Rarity) * 5; // The more common the item, the more they want to sell it

            int final = propensity.MinCap(0);
            MetricsLogger.LogAppendStatistic(MetricType.WillingnessToSell, unit.DisplayName, final);
            MetricsLogger.LogAppendStatistic(MetricType.WillingnessToSell, item.DisplayName, final);

            return final;
        }

        /// <summary>
        /// How willing the unit is to part with the item.
        /// </summary>
        public virtual int DetermineWillingnessToBuyItem(IMakeDecisions unit, Item item)
        {
            if (unit.Preferences.ItemPreference.WillNotBuyItem(item)) return Params.Prohibitive; // will not shop if the owner won't buy item

            bool isWeaponUpgrade = unit is DecisionMakingUnit && unit.WantsToUpgradeWeapon && item.Stats.Type == ItemType.Weapon && ((DecisionMakingUnit)unit).ItemIsUpgrade(item, ItemType.Weapon);
            bool isArmorUpgrade = unit is DecisionMakingUnit && unit.WantsToUpgradeArmor && item.Stats.Type == ItemType.Armor && ((DecisionMakingUnit)unit).ItemIsUpgrade(item, ItemType.Armor);

            int propensity = 0;
            int itemCount = unit.Inventory.GetItemCount(item);

            propensity += unit.Preferences.ItemPreference.GetPreference(item); // Want to buy preferred items            
            propensity -= itemCount == 0 ? 0 : (int)((double)((itemCount - 1) * unit.Preferences.ItemPreference.QuantityIntoleranceModifier) * Math.Log10(itemCount)); // For every item over 1, steady decrease in willingness to buy            
            propensity += isWeaponUpgrade ? 30 : 0; // Wants to buy a weapon!
            propensity += isWeaponUpgrade && (item.Stats as WeaponStats).WeaponType == unit.CurrentStats.WeaponTypePreference ? 30 : 0; // If weapon is preferred type, doubles willingness
            propensity += isArmorUpgrade ? 30 : 0; // Wants to buy armor!            

            // To prevent hireable units from buying useless weapons.
            propensity += unit.UpgradesEquipment && item.Stats.Type == ItemType.Weapon && !isWeaponUpgrade ? Params.Prohibitive : 0; // Does not buy weapon non-upgrades if cares about upgrading
            propensity += unit.UpgradesEquipment && item.Stats.Type == ItemType.Armor && !isArmorUpgrade ? Params.Prohibitive : 0; // Does not buy armor non-upgrades if cares about upgrading

            int final = propensity.MinCap(0);
            MetricsLogger.LogAppendStatistic(MetricType.WillingnessToBuy_Units, unit.DisplayName, final);
            MetricsLogger.LogAppendStatistic(MetricType.WillingnessToBuy_Items, item.DisplayName, final);

            if (propensity > 0)
            {
                // Only add this if the unit is already interested in buying the item.
                propensity += Utilities.GetRandomNumber(0, 10); // Randomness of decision for all other things equal
            }

            return propensity;
        }

        /// <summary>
        /// Make a buying or selling bid.
        /// </summary>
        /// <param name="unit">Unit making bid</param>
        /// <param name="item">Item being sold or inspected for purchase</param>
        /// <param name="sellingBid">Whether the unit is selling the item (true) or buying the item (false)</param>
        /// <returns>The bid, after all is considered. How much they offer to buy for, or to sell for.</returns>
        public virtual int MakeBid(IMakeDecisions unit, Item item, bool sellingBid)
        {                        
            int willingnessToTradeItem = sellingBid ? DetermineWillingnessToSellItem(unit, item) :
                                                      DetermineWillingnessToBuyItem(unit, item);
            if (willingnessToTradeItem <= 0)
            {
                return 0;
            }

            int bid = MakeAppraisal(unit, item);            

            // cut the price by half of the willingness to sell, as a percent, but make sure it's at least half. Or increase price if want to buy.            
            bid = sellingBid ? Math.Max(bid / 2, bid - (int)(((float)Math.Abs((int)willingnessToTradeItem) / 100.0f) * (float)bid)) :
                               Math.Max(bid / 2, bid + (int)(((float)Math.Abs((int)willingnessToTradeItem) / 100.0f) * (float)bid));

            // cut the price by markup to buy, or increase by markup to sell.
            bid = sellingBid ? Math.Max(1, bid + Utilities.GetRangePercent(bid, 0, unit.Preferences.ItemPreference.PriceMarkupRange)) :
                               Math.Max(1, bid - Utilities.GetRangePercent(bid, 0, unit.Preferences.ItemPreference.PriceMarkupRange));
            
            string format = sellingBid ? "{0} asks {1} for {2}" : "{0} bids {1} for {2}";
            //Console.WriteLine(format, unit.DisplayName, bid, item.DisplayName);

            MetricsLogger.LogAppendStatistic(sellingBid ? MetricType.SellingBid : MetricType.BuyingBid, unit.DisplayName, bid);
            return bid;
        }
        /// <summary>
        /// Unit calculates value of the item based on skills and a potential boost.
        /// </summary>
        /// <param name="unit">Unit doing the appraisal.</param>
        /// <param name="item">Appraised item.</param>
        /// <param name="boost">Boosts correctness rate based on past experiences.</param>
        /// <returns>Unit's appraised value of the item.</returns>
        public virtual int MakeAppraisal(IMakeDecisions unit, Item item, int boost = 0) 
        {
            int itemDifficulty = 10 + (int)item.Stats.Rarity * ((int)Math.Log10(item.Stats.Cost) + 1) + 1; 
            int appraisalSkill = (int)Math.Ceiling((double)unit.CurrentStats.Cunning / 50) + // 1 point for each cunning
                                 ((int)unit.CurrentStats.Skills.GetSkill(UnitSkills.SkillType.Appraisal).SkillLevel * 3) + // 3 points per appraisal
                                 ((int)unit.CurrentStats.Skills.GetSkill(UnitSkills.SkillType.Merchant).SkillLevel) +  // 1 point per merchant skill
                                 boost + // bonus boost, such as for reassessment
                                 Utilities.GetRandomNumber(0,2); // luck
            int inaccuracy = Math.Max(0, itemDifficulty - appraisalSkill); // 0 is right on the ball. 10 is 10%
            float window = 10.0f * (float)Utilities.GetRandomNumber(-inaccuracy, inaccuracy); // guess window, percent
            window = window.GetClampedValue(-75.0f, 75.0f); // Can't go too far off...
            int appraisal = item.Stats.Cost + (int)((window / 100.0f) * item.Stats.Cost);

            //Console.WriteLine("{0} appraises {1} for {2}", unit.DisplayName, appraisal, item.DisplayName);

            int finalInaccuracy = Math.Abs(item.Stats.Cost - appraisal);
            MetricsLogger.LogAppendStatistic(MetricType.AppraisalInaccuracy, item.DisplayName, finalInaccuracy);
            MetricsLogger.LogAppendStatistic(MetricType.AppraisalInaccuracy, unit.DisplayName, finalInaccuracy);
            
            if (unit.TransactionHistory.ContainsKey(item.ObjectName)) 
            {                
                return ((unit.TransactionHistory[item.ObjectName] + appraisal) / 2); // If previous xaction, get average of appraisal and past deal
            }
            
            return appraisal;
        }

        /// <summary>
        /// Given currentBid, unit tries to improve the bid by lowering it to sell the item.
        /// </summary>
        public virtual int ImproveBid(IMakeDecisions unit, Item item, int currentBid)
        {
            int assessmentBoost = (unit.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Merchant) / 2) + 1; // Use merchant skill to guess better
            int reassessment = this.MakeAppraisal(unit, item, assessmentBoost); // Try to guess the price again with more info.
            if (reassessment >= currentBid)
            {
                // If reassessment is higher than current bid, will either make a lower bid, push with current, or give up                                          
                int chance = Utilities.GetRandomNumber(0, 10);
                if (chance < 2)
                    reassessment = 0; // Feel ripped off. Bail.
                else if (chance < 6)
                    reassessment = currentBid; // Stick to the bid. 
                else
                    reassessment = currentBid - Utilities.GetRangePercent(currentBid, 1, 20); // Try to improve the bid by 1-20%
            }

            // If reassessment is lower than current bid, then maybe it's more accurate and will be successful
            //Console.WriteLine("{0} rebids {1} for {2}", unit.DisplayName, reassessment, item.DisplayName);

            int improvement = (Math.Abs(currentBid - item.Stats.Cost)) - (Math.Abs(reassessment - item.Stats.Cost));
            int percentChange = (int)(((double)reassessment / (double)currentBid) * 100.0d) - 100;
            MetricsLogger.LogAppendStatistic(MetricType.ReassessmentImprovement, item.DisplayName, improvement);
            MetricsLogger.LogAppendStatistic(MetricType.ReassessmentImprovement, unit.DisplayName, improvement);
            MetricsLogger.LogAppendStatistic(MetricType.ReassessmentPercentChange, item.DisplayName, percentChange);
            MetricsLogger.LogAppendStatistic(MetricType.ReassessmentPercentChange, unit.DisplayName, percentChange);
            MetricsLogger.LogAppend(reassessment == 0 ? MetricType.ReassessmentBail : MetricType.ReassessmentRetry, unit.DisplayName, 1);            

            return reassessment;            
        }

        public virtual bool UnitWantsToBuyItem(IMakeDecisions unitBuying, IMakeDecisions unitSelling, Item item, int ask)
        {
            if (unitBuying.Inventory.Money < ask)
            {
                //Console.WriteLine("Buyer {0} cannot afford bid {1}", unitBuying.DisplayName, ask);
                return false; // Can't afford bid
            }

            int opposingBid = this.MakeBid(unitBuying, item, false);

            int leeway = 0; // Percentage willing to go over initial bid
            leeway += (unitSelling.CurrentStats.Cunning / 50); // Every 50 levels of cunning equal one percent chance to go over bid.
            leeway += ((int)unitSelling.CurrentStats.Skills.GetSkill(UnitSkills.SkillType.Merchant).SkillLevel) * 2;  // Every merchant skill = 2% chance.
            leeway += Utilities.GetRandomNumber(0, 2); // Willing to go between 0 and 2 percent lower
            leeway += (int)item.Stats.Rarity * 2; // Willing to go over for rare goods. 
            leeway += unitBuying.Preferences.ItemPreference.GetDominantPreference(item); // Get dominant preferences for item. Add directly.
            int amountWillingToAdd = (int)(((double)leeway / 100.0d) * (double)opposingBid);
            //Console.WriteLine("Buyer {0} is willing to pay {1} and add at most an extra {2}", unitBuying.DisplayName, opposingBid, amountWillingToAdd);

            MetricsLogger.LogAppendStatistic(MetricType.UnitWantsToBuy, unitBuying.DisplayName, ask);
            MetricsLogger.LogAppendStatistic(MetricType.UnitWantsToBuy, item.DisplayName, ask);
            
            return ask <= Math.Max(0, opposingBid + amountWillingToAdd); // As long as it's at least 0 gold, accept the bid.            
        }


        public virtual int ExpectedSellValue(DecisionMakingUnit a, Item item)
        {
            throw new NotImplementedException();
        }

        public virtual int GetMinimumSellValue(GameObjects.Units.IMakeDecisions unit, Items.Item item)
        {
            throw new NotImplementedException();
        }
    }
}
