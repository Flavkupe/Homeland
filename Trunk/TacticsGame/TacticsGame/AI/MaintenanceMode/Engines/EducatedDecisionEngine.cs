using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.Utility;
using TacticsGame.GameObjects.Buildings;
using System.Diagnostics;
using TacticsGame.Items;
using TacticsGame.GameObjects.Owners;
using TacticsGame.Metrics;
using TacticsGame.GameObjects.Visitors;
using TacticsGame.World;
using TacticsGame.GameObjects.Zones;

namespace TacticsGame.AI.MaintenanceMode.Engines
{
    public class EducatedDecisionEngine : TacticsGame.AI.MaintenanceMode.IDecisionEngine
    {
        private IPreferenceEngine preferenceEngine = null;

        private ITownMarketManager townMarketManager = null; 

        public EducatedDecisionEngine()
        {
            this.preferenceEngine = new EducatedPreferenceEngine();
            this.townMarketManager = new TownMarketManager(this.preferenceEngine);
        }

        public EducatedDecisionEngine(IPreferenceEngine preferenceEngine, ITownMarketManager townMarketManager)
        {
            this.preferenceEngine = preferenceEngine ?? new EducatedPreferenceEngine();
            this.townMarketManager = townMarketManager ?? new TownMarketManager(this.preferenceEngine);
        }

        public IPreferenceEngine PreferenceEngine
        {
            get { return this.preferenceEngine; }
        }

        public virtual Decision MakeDecision(IMakeDecisions unit)
        {
            int propensityToGoSell = CalculatePropensityToSell(unit);
            int propensityToGoBuy = CalculatePropensityToBuy(unit);
            int propensityToGather = CalculatePropensityToGather(unit);
            int propensityToLeisure = CalculatePropensityToLeisure(unit);

            int maxVal = new [] { propensityToGoSell, propensityToGather, propensityToLeisure, propensityToGoBuy }.Max();

            Decision choice;

            if (propensityToGather == maxVal)            
            {
                choice = DecideWhatToGather(unit);
            }
            else if (propensityToGoSell == maxVal)
            {
                choice = Decision.Sell;
            }
            else if (propensityToGoBuy == maxVal)
            {
                choice = Decision.Buy;
            }
            else
            {
                choice = Decision.RestAtHome;
            }

            MetricsLogger.LogAppend(MetricType.Decisions, choice.ToString(), 1);
            return choice;
        }

        public virtual Decision MakeOwnerDecision(Owner owner)
        {
            if (owner.CanCraft) 
            {
                return Decision.Craft;
            }
           
            if (owner.OwnedBuilding.Visitors.Count == 0)
            {
                return Decision.Idle;
            }
            
            // TODO
            return new Decision[] { Decision.Sell, Decision.Sell, Decision.Buy }.GetRandomItem();
        }

        public virtual DecisionMakingUnit DetermineTargetVisitor(IMakeDecisions unit, Decision decision, List<DecisionMakingUnit> list)
        {
            List<DecisionMakingUnit> visitors = new List<DecisionMakingUnit>(list.Where<DecisionMakingUnit>(a => a is Visitor));

            if (decision == Decision.Sell)
            {
                Dictionary<string, int> itemsInterestedInSelling = this.DecideWhatToSell(unit);
                return this.DecideWhichVisitorToSellTo(unit, visitors, itemsInterestedInSelling);
            }
            else if (decision == Decision.Buy)
            {
                return this.DecideWhichVisitorToBuyFrom(unit, visitors);
            }
            else
            {
                Debug.Assert(false, "Shouldn't reach this code!");
                return null;
            }
        }

        private DecisionMakingUnit DecideWhichVisitorToBuyFrom(IMakeDecisions unit, List<DecisionMakingUnit> visitors)
        {                       
            return visitors.GetRandomItem();
        }

        private DecisionMakingUnit DecideWhichVisitorToSellTo(IMakeDecisions unit, List<DecisionMakingUnit> list, Dictionary<string, int> itemsInterestedInSelling)
        {
            List<ObjectValuePair<Visitor>> visitorsByValue = new List<ObjectValuePair<Visitor>>();
            int counter = 3;
            while (itemsInterestedInSelling.Count > 0 && counter > 0)
            {
                counter--;
                string itemName = itemsInterestedInSelling.GetMaxItem<string>();
                itemsInterestedInSelling.Remove(itemName);                
                foreach (Visitor visitor in list)
                {
                    int propensity = 0;
                    propensity += visitor.BuysItems && visitor.Inventory.Money > 0 ? 0 : Params.Prohibitive; // will only sell to visitors that will buy items
                    propensity += PropensityToSellItemToUnit(unit, new Item(itemName), visitor);
                    if (propensity > 0)
                    {
                        propensity += Utilities.GetRandomNumber(0, Params.VisitorToSellTo_ExtraVariance); // Give random chance to pick between equal shops
                        visitorsByValue.Add(new ObjectValuePair<Visitor>(visitor, propensity));
                    }
                }
            }

            Visitor choice = visitorsByValue.GetMax<Visitor>();
            MetricsLogger.LogAppend(MetricType.VisitorToSellTo, choice == null ? "NULL" : choice.DisplayName, 1);
            return choice;
        }

        public virtual Building DetermineTargetBuilding(IMakeDecisions unit, Decision decision, List<Building> list)
        {
            Debug.Assert(list.Count > 0, "Should always have a building...");

            if (decision == Decision.Sell)
            {
                Dictionary<string, int> itemsInterestedInSelling = this.DecideWhatToSell(unit);
                return this.DecideWhichBuildingToSellTo(unit, list, itemsInterestedInSelling);                
            }
            else if (decision == Decision.Buy)
            {
                return this.DecideWhichBuildingToBuyFrom(unit, list);
            }
 
            return list[Utilities.GetRandomNumber(0, list.Count - 1)];
        }

        /// <summary>
        /// Unit decides which building to buy from.
        /// </summary>
        private Building DecideWhichBuildingToBuyFrom(IMakeDecisions unit, List<Building> list)
        {
            List<ObjectValuePair<Building>> buildingsByValue = new List<ObjectValuePair<Building>>();
            foreach (Building building in list)
            {
                int propensity = 0;
                propensity += building.IsBuildingThatSellsThings && building.IsBuildingWithStock ? 0 : Params.Prohibitive; // will not shop at non-free market buildings
                if (building.IsBuildingWithOwner)
                {
                    Owner owner = building.Owner;
                    propensity += PropensityToBuyFromUnit(unit, owner);

                    if (propensity > 0)
                    {
                        propensity += Utilities.GetRandomNumber(0, Params.BuildingToBuyFrom_ExtraVariance); // Random chance to pick between equal shops
                        buildingsByValue.Add(new ObjectValuePair<Building>(building, propensity));
                    }
                }
            }

            Building choice = buildingsByValue.GetMax<Building>();
            MetricsLogger.LogAppend(MetricType.BuildingToBuyFrom, choice == null ? "NULL" : choice.DisplayName, 1);
            return choice;
        }

        private static int PropensityToBuyFromUnit(IMakeDecisions unitBuying, DecisionMakingUnit unitSelling)
        {
            int propensity = Params.BuildingToBuyFrom_Starting; // Starting bit
            propensity += unitSelling.Inventory.Items.Count > 0 ? 0 : Params.Prohibitive; // Shop has no inventory                    
            propensity += (unitBuying.UpgradesEquipment && unitBuying is DecisionMakingUnit && unitBuying.WantsToUpgradeWeapon
                        && unitSelling.HasItemUpgradeInStock(unitBuying as DecisionMakingUnit, ItemType.Weapon)) ? Params.BuildingToBuyFrom_HasWeapUpgrade : 0;

            MetricsLogger.LogAppendStatistic(MetricType.WantsToBuyFrom, string.Format("{0}=>{1}", unitBuying.DisplayName, unitSelling.DisplayName), propensity.MinCap(0));
            return propensity;
        }

        private Building DecideWhichBuildingToSellTo(IMakeDecisions unit, List<Building> list, Dictionary<string, int> itemsInterestedInSelling)
        {
            Dictionary<Building, int> buildingsByValue = new Dictionary<Building, int>();
            
            int counter = 3;
            while (itemsInterestedInSelling.Count > 0 && counter > 0)
            {
                counter--;
                string itemName = itemsInterestedInSelling.GetMaxItem();
                itemsInterestedInSelling.Remove(itemName);
                
                foreach (Building building in list)
                {
                    int propensity = Params.BuildingToSellTo_Starting;
                    propensity += building.IsBuildingThatBuysThings && building.IsBuildingWithStock ? 0 : Params.Prohibitive; // will not shop at non-free market buildings
                    if (building.IsBuildingWithOwner)
                    {
                        Owner owner = building.Owner;
                        propensity += PropensityToSellItemToUnit(unit, new Item(itemName), owner);

                        if (propensity > 0)
                        {
                            propensity += Utilities.GetRandomNumber(0, Params.BuildingToSellTo_ExtraVariance); // Give random chance to pick between equal shops
                            buildingsByValue.SetOrIncrement(building, propensity);
                        }
                    }
                }
            }

            Building choice = buildingsByValue.GetMaxItem<Building>();             
            MetricsLogger.LogAppend(MetricType.BuildingToSellTo, choice == null ? "NULL" : choice.DisplayName, 1);
            return choice;
        }

        private static int PropensityToSellItemToUnit(IMakeDecisions unitSelling, Item item, DecisionMakingUnit unitBuying)
        {
            if (unitBuying.Preferences.ItemPreference.WillNotBuyItem(item)) return Params.Prohibitive; // will not shop if the owner won't buy item            
            int propensity = 0; 
            propensity += unitBuying.Preferences.ItemPreference.GetPreference(item); // Add owner's preference for the item to propensity to buy from owner
            propensity += unitBuying.Inventory.Money > Params.BuildingToSellTo_MinimumMoney ? 0 : Params.Prohibitive; // TODO: find a better number than 10... 
            propensity += unitBuying.Inventory.Money > item.Stats.Cost ? Params.MidLow : 0; // Shopkeep can afford the item... 
            propensity += ParamUtils.PropensityToGetMoreItemsByQuantity(unitBuying.Inventory.GetItemCount(item)); // Add/Subtract the willingness of the buying unit to get more items... ie less propensity if he has multiple of the items.            
            propensity = propensity.MinCap(0);
            MetricsLogger.LogAppendStatistic(MetricType.WantToSellTo, string.Format("{0}=>{1}", unitSelling.DisplayName, unitBuying.DisplayName), propensity);                        
            return propensity;
        }

        protected virtual Dictionary<string, int> DecideWhatToSell(IMakeDecisions unit)
        {
            Dictionary<string, int> propensityToSellType = new Dictionary<string, int>(); // How much units wants to sell item types, to find the right store.
            foreach (Item item in unit.Inventory.Items)
            {
                int willingnessToSell = this.preferenceEngine.DetermineWillingnessToSellItem(unit, item);
                propensityToSellType.SetOrIncrement(item.ObjectName, willingnessToSell);
            }

            return propensityToSellType;
        }

        protected virtual Decision DecideWhatToGather(IMakeDecisions unit)
        {
            int propensityForStone = CalculatePropensityToGatherStone(unit);
            int propensityForLumber = CalculatePropensityToGatherWood(unit);
            int propensityForMining = CalculatePropensityToMineOre(unit);
            int propensityForHunting = CalculatePropensityToHunt(unit);
            int propensityForForaging = CalculatePropensityToForage(unit);
            int maxVal = new[] { propensityForStone, propensityForLumber, propensityForMining, propensityForHunting, propensityForForaging }.Max();

            if (propensityForLumber == maxVal)
            {
                return Decision.GetLumber;
            }
            else if (propensityForStone == maxVal)
            {
                return Decision.MineStone;
            }
            else if (propensityForHunting == maxVal)
            {
                return Decision.Hunt;
            }
            else if (propensityForForaging == maxVal)
            {
                return Decision.Forage;
            }
            else
            {
                return Decision.MineOre;
            }
        }        

        public virtual int DetermineDecisionDuration(IMakeDecisions unit, Decision decision)
        {
            return Utilities.GetRandomNumber(5, 8);
        }

        /////////
        #region Propensity Calculations        
        /////////

        protected virtual int CalculatePropensityToBuy(IMakeDecisions unit)
        {
            int total = Params.PropensityToBuy_Starting;
            //total += unit.PreviousDecision == Decision.Sell && (unit.LastResult == null || unit.LastResult.ActionSuccess == true) ? Params.PropensityToBuy_JustSold : 0; // After selling they almost always wanna buy something
            total += unit.PreviousDecision == Decision.Buy ? Params.Prohibitive: 0; // Don't usually buy twice in a row
            total += unit.Inventory.Money > Params.PropensityToBuy_LotsOfMoneyAmount ? Params.PropensityToBuy_LotsOfMoney : 0; // Have plenty of money... high chance of spending it
            total += unit.Inventory.Money > Params.PropensityToBuy_TooLittleMoneyAmount ? 0 : Params.Prohibitive; // If you have little money, don't spend it.
            total += unit.WantsToUpgradeWeapon || unit.WantsToUpgradeArmor ? Params.PropensityToBuy_WantsToUpgradeWeaponOrArmor : 0; // Unit wants to upgrade weapon or armor            

            if (!unit.WantsToUpgradeArmor && !unit.WantsToUpgradeWeapon)
            {
                total = 0;
            }

            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "ToBuy", total.MinCap(0));

            total += Utilities.GetRandomNumber(0, Params.RandomVariance);

            return total;
        }

        protected virtual int CalculatePropensityToLeisure(IMakeDecisions unit)
        {
            if (unit.CurrentStats.ActionPoints >= unit.BaseStats.ActionPoints)
            {
                // If unit is full in AP, have very little incentive to rest.
                return 1;
            }

            int total = Params.PropensityToLeisure_Starting;

            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "ToLeisure", total);

            total += Utilities.GetRandomNumber(0, Params.RandomVariance);

            total += unit.CurrentStats.ActionPoints <= 2 ? Params.Mid : 0;
            total += unit.CurrentStats.ActionPoints == 0 ? Params.VeryHigh : 0;

            return total;
        }

        protected virtual int CalculatePropensityToGatherWood(IMakeDecisions unit)
        {
            int total = 10;
            int numItems = unit.Inventory.GetItemCount(ItemMetadata.WoodcuttingLoot);
            total += numItems == 0 ? 30 : 0; // no wood... increase likeleyhood
            total += numItems > 4 ? -Params.High : 0; // already have lots
            total += numItems > 10 ? Params.Prohibitive : 0; // already have way lots
            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "Wood", total.MinCap(0));
            total += Utilities.GetRandomNumber(0, Params.RandomVariance);
            return total;
        }

        protected virtual int CalculatePropensityToForage(IMakeDecisions unit)
        {
            int total = 10;

            int numItems = unit.Inventory.GetItemCount(ItemMetadata.HuntingLoot);

            total += numItems == 0 ? 30 : 0; // no items... increase likeleyhood
            total += numItems > 6 ? -Params.MidLow : 0; // already have lots
            total += numItems > 8 ? -Params.MidLow : 0; // already have lots
            total += numItems > 12 ? Params.Prohibitive : 0; // already have way lots            

            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "Forage", total.MinCap(0));
            total += Utilities.GetRandomNumber(0, Params.RandomVariance);
            return total;
        }

        protected virtual int CalculatePropensityToHunt(IMakeDecisions unit)
        {
            int total = 10;
            int numItems = unit.Inventory.GetItemCount(ItemMetadata.HuntingLoot);
            total += numItems == 0 ? 30 : 0; // no leather... increase likeleyhood
            total += numItems > 3 ? -Params.High : 0; // already have lots
            total += numItems > 10 ? Params.Prohibitive : 0; // already have way lots
            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "Hunt", total.MinCap(0));
            total += Utilities.GetRandomNumber(0, Params.RandomVariance);
            return total;
        }

        protected virtual int CalculatePropensityToMineOre(IMakeDecisions unit)
        {
            int total = 10;
            int numItems = unit.Inventory.GetItemCount(ItemMetadata.MiningLoot);
            total += numItems == 0 ? 30 : 0; // no ore... increase likeleyhood
            total += numItems > 4 ? -Params.High : 0; // already have lots
            total += numItems > 10 ? Params.Prohibitive : 0; // already have way lots
            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "Ore", total.MinCap(0));
            total += Utilities.GetRandomNumber(0, Params.RandomVariance);
            return total;
        }

        protected virtual int CalculatePropensityToGatherStone(IMakeDecisions unit)
        {
            int total = 10;
            int numItems = unit.Inventory.GetItemCount(ItemMetadata.StoneMiningLoot);
            total += numItems == 0 ? 30 : 0; // no stone... increase likeleyhood
            total += numItems > 4 ? -Params.High : 0; // already have lots
            total += numItems > 10 ? Params.Prohibitive : 0; // already have way lots
            total += unit.CurrentStats.Physical < 30 ? -20 : 0; // Weaker units prefer not to mine stone            
            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "ToStone", total.MinCap(0));
            total += Utilities.GetRandomNumber(0, Params.RandomVariance);
            return total;
        }

        protected virtual int CalculatePropensityToGather(IMakeDecisions unit)
        {
            int total = 10; 
            total += unit.Inventory.Items.Count == 0 ? 40 : 0; // no items... increase likeleyhood                        
            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "ToGather", total.MinCap(0));
            total += Utilities.GetRandomNumber(0, Params.RandomVariance);
            return total;
        }

        protected virtual int CalculatePropensityToSell(IMakeDecisions unit)
        {
            int total = 0;
            total += unit.PreviousDecision == Decision.Sell ? Params.Prohibitive : 0; // Don't usually sell twice in a row                        
            total += unit.PreviousDecision == Decision.Buy && (unit.LastResult == null || unit.LastResult.ActionSuccess) ? Params.Prohibitive : 0; // Don't sell right after buying
            total += unit.Inventory.Items.Count > 6 ? Params.MidLow : 0; // Too many items... want to empty out.
            total += unit.Inventory.GetMaxItemCount() > 4 ? Params.Mid : 0; // Too many of one item... go sell it
            total += unit.Inventory.GetMaxItemCount() > 10 ? Params.Mid : 0; // Seriously, go sell.
            total += unit.Inventory.Items.Count <= 2 ? Params.Prohibitive : 0; // Few items... not motivated to go sell
            MetricsLogger.LogAppendStatistic(MetricType.Propensity, "ToSell", total.MinCap(0));
            total += Utilities.GetRandomNumber(0, Params.RandomVariance);
            return total;            
        }

        /////////
        #endregion
        /////////

        public bool DecisionRequiresBuilding(Decision decision)
        {
            return decision == Decision.Buy || decision == Decision.Sell;
        }

        public bool DecisionRequiresVisitor(Decision decision)
        {
            return decision == Decision.Buy || decision == Decision.Sell;
        }


        public bool DecisionRequiresExitZone(Decision decision)
        {
            return decision == Decision.GetLumber || decision == Decision.Hunt || decision == Decision.MineGems || decision == Decision.MineOre ||
                   decision == Decision.MineStone || decision == Decision.Forage;
        }

        public ExitZone GetTargetZone(Decision decision, IEnumerable<ExitZone> zones) 
        {
            if (decision == Decision.GetLumber || decision == Decision.Forage)
            {
                return zones.Where(a => a.LeadsToWood).GetRandomItem();
            }

            if (decision == Decision.Hunt)
            {
                return zones.Where(a => a.LeadsToHunt).GetRandomItem();
            }

            if (decision == Decision.MineGems || decision == Decision.MineStone || decision == Decision.MineOre)
            {
                return zones.Where(a => a.LeadsToStone).GetRandomItem();
            }

            return null;
        }

    }
}
