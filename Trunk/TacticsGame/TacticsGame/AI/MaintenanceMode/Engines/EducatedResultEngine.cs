using System;
using System.Collections.Generic;
using System.Linq;
using TacticsGame.GameObjects.Owners;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;
using TacticsGame.Metrics;
using TacticsGame.Crafting;

namespace TacticsGame.AI.MaintenanceMode.Engines
{
    public class EducatedResultEngine : IActionResultEngine
    {
        private IGenerateLoot lootEngine;        

        private IPreferenceEngine preferenceEngine;

        public EducatedResultEngine(IPreferenceEngine preferenceEngine)
            : this()
        {
            this.preferenceEngine = preferenceEngine;          
        }

        public EducatedResultEngine()
        {
            if (this.preferenceEngine == null)
            {
                this.preferenceEngine = new EducatedPreferenceEngine();
            }

            this.lootEngine = new LootGenerationEngine();
        }

        public virtual ActivityResult GetActionResult(UnitManagementActivity activity)
        {
            if (activity.Unit is Owner)
            {
                return this.GetOwnerActionResult(activity);
            }
            else
            {
                return this.GetUnitActionResult(activity);
            }
        }

        public virtual ActivityResult GetUnitActionResult(UnitManagementActivity activity)
        {            
            Decision decision = activity.Decision;
            DecisionMakingUnit unit = activity.Unit;

            ActivityResult result = null;

            if (decision == Decision.GetLumber)
            {
                result = DetermineGetLumberResults(unit);
            }
            else if (decision == Decision.MineStone)
            {
                result = DetermineMineStoneResults(unit);
            }
            else if (decision == Decision.MineOre)
            {
                result = DetermineMineOreResults(unit);
            }
            else if (decision == Decision.Hunt)
            {
                result = DetermineHuntResults(unit);
            }
            else if (decision == Decision.Forage)
            {
                result = DetermineForageResults(unit);
            }
            else if (decision == Decision.Sell)
            {
                result = DetermineSellResults(unit, activity.TargetBuilding.Owner);
                result.BuildingTarget = activity.TargetBuilding;
                result.TransactionTarget = activity.TargetBuilding.Owner;                
            }
            else if (decision == Decision.Buy)
            {
                result = DetermineBuyResults(unit, activity.TargetBuilding.Owner);
                result.BuildingTarget = activity.TargetBuilding;
                result.TransactionTarget = activity.TargetBuilding.Owner;                
            }
            else if (decision == Decision.RestAtHome)
            {
                result = DetermineRestResults(unit);
            }
            else
            {
                result = new ActivityResult();
            }

            return result; 
        }     

        public virtual ActivityResult GetOwnerActionResult(UnitManagementActivity activity)
        {
            Decision decision = activity.Decision;
            Owner unit = activity.Unit as Owner;

            ActivityResult result = null;

            if (decision == Decision.Sell)
            {
                result = DetermineSellResults(unit, activity.TargetVisitor);
                result.VisitorTarget = activity.TargetVisitor;
                result.TransactionTarget = activity.TargetVisitor;
                result.ActionPointCost = 3;                                                
            }
            else if (decision == Decision.Buy)
            {
                result = DetermineBuyResults(unit, activity.TargetVisitor);
                result.VisitorTarget = activity.TargetVisitor;
                result.TransactionTarget = activity.TargetVisitor;
                result.ActionPointCost = 3;                   
            }
            else if (decision == Decision.Craft)
            {
                result = this.DetermineCraftResults(unit);
                result.ActionPointCost = 1;                
            }
            else
            {
                result = new ActivityResult();
            }

            return result;
        }

        private ActivityResult DetermineRestResults(DecisionMakingUnit unit)
        {
            return new ActivityResult()
            {
                ActionPointCost = -2, 
            };
        }   

        private ActivityResult DetermineCraftResults(IMakeDecisions unit)
        {
            List<Recipe> recipes = unit.KnownRecipes.Where<Recipe>(a => a.CanCraft(unit)).ToList<Recipe>();
            Recipe recipe = recipes.GetRandomItem();

            ActivityResult result = new ActivityResult();
            recipe.Results.ForEach(a => result.AddItemsToItemsGained(a.Number, a.Item));
            recipe.Ingredients.ForEach(a => result.AddItemsToItemsLost(a.Number, a.Item));
            recipe.Results.ForEach(a => unit.AcquireItems(a.Number, a.Item, AcquiredItemSource.Crafted));
            recipe.Ingredients.ForEach(a => unit.Inventory.RemoveItems(a.Item, a.Number));
            
            return result;
        }

        protected virtual ActivityResult DetermineBuyResults(IMakeDecisions buyingUnit, IMakeDecisions sellingUnit)
        {
            ActivityResult result = new ActivityResult();                        

            int numberOfItemsWanted = Math.Min(Utilities.GetRandomNumber(1, 3), sellingUnit.Inventory.Items.Count);
            //Console.WriteLine("{0} wants to buy {1} items from {2}", buyingUnit.DisplayName, numberOfItemsWanted, sellingUnit.DisplayName);
            List<Item> itemsToBuy = this.DecideItemsToTryToBuy(buyingUnit, sellingUnit);

            for (int i = 0; i < Math.Min(itemsToBuy.Count, numberOfItemsWanted); ++i)
            {                
                Item item = itemsToBuy.GetRandomItem();
                itemsToBuy.Remove(item);
                if (this.ProcessSale(sellingUnit, buyingUnit, item, result, false))
                {
                    if (item.Stats.Type == ItemType.Weapon)
                    {
                        // If already bought a weapon, will not buy another weapon.
                        itemsToBuy.RemoveAll(a => a.Stats.Type == ItemType.Weapon); 
                    }
                }
            }

            if (result.ItemsGained != null)
            {
                foreach (Item item in result.ItemsGained) 
                {
                    MetricsLogger.LogAppend(MetricType.ItemsBought, item.DisplayName, 1);
                    MetricsLogger.LogAppendByUnit("ItemsBought", buyingUnit.DisplayName, item.DisplayName, 1);
                }
            }

            result.ActionPointCost = 1;            
            return result;
        }
        
        /// <summary>
        /// Creates a list of items that the unit wants to buy, based on sellingUnit's inventory and buyingUnit's preferences.
        /// </summary>
        private List<Item> DecideItemsToTryToBuy(IMakeDecisions buyingUnit, IMakeDecisions sellingUnit)
        {
            List<Item> items = new List<Item>();
            if (buyingUnit.WantsToUpgradeWeapon && sellingUnit is Owner)
            {
                items.AddRange(((Owner)sellingUnit).GetListOfItemUpgrades(buyingUnit as DecisionMakingUnit, ItemType.Weapon).Where(a => this.UnitThinksHeCanAfford(buyingUnit, a)));
            }

            if (buyingUnit.WantsToUpgradeArmor && sellingUnit is Owner)
            {
                items.AddRange(((Owner)sellingUnit).GetListOfItemUpgrades(buyingUnit as DecisionMakingUnit, ItemType.Armor).Where(a => this.UnitThinksHeCanAfford(buyingUnit, a)));
            }

            if (items.Count == 0)
            {
                // No upgrades available... put rest of inventory there
                items.AddRange(sellingUnit.Inventory.Items);
            }

            return items;
        }

        /// <summary>
        /// Whether unit thinks he can afford the item.
        /// </summary>
        private bool UnitThinksHeCanAfford(IMakeDecisions unit, Item item)
        {
            return this.preferenceEngine.MakeAppraisal(unit, item) < unit.Inventory.Money;
        }

        protected virtual ActivityResult DetermineSellResults(IMakeDecisions sellingUnit, IMakeDecisions buyingUnit)
        {                        
            ActivityResult result = new ActivityResult();            

            List<ObjectValuePair<Item>> sortedList = new List<ObjectValuePair<Item>>();
            foreach (Item item in sellingUnit.Inventory.Items)
            {
                int willingness = this.preferenceEngine.DetermineWillingnessToSellItem(sellingUnit, item);
                if (willingness > 0)
                {
                    sortedList.Add(new ObjectValuePair<Item>(item, willingness));
                }
            }

            if (sortedList.Count > 0)
            {
                sortedList.Sort();

                int numberOfItemsToSell = Utilities.GetRandomNumber(1, sortedList.Count);
                //Console.WriteLine("{0} wants to sell {1} items to {2}", sellingUnit.DisplayName, numberOfItemsToSell, buyingUnit.DisplayName);
                for (int i = numberOfItemsToSell - 1; i >= 0; --i)
                {
                    this.ProcessSale(sellingUnit, buyingUnit, sortedList[i].Object, result, true);
                }
            }
            
            if (result.ItemsLost != null)
            {
                foreach (Item item in result.ItemsLost)
                {
                    MetricsLogger.LogAppend(MetricType.ItemsSold, item.DisplayName, 1);
                    MetricsLogger.LogAppendByUnit("ItemsSold", sellingUnit.DisplayName, item.DisplayName, 1);
                }
            }

            result.ActionPointCost = 1;
            return result;
        }

        /// <summary>
        /// Perform a sale. Return whether the sale was successful.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ProcessSale(IMakeDecisions seller, IMakeDecisions buyer, Item item, ActivityResult result, bool initiatorIsSeller)
        {           
            // Generate an initial bid
            int bid = this.preferenceEngine.MakeBid(seller, item, true);

            if (bid <= 0)
            {
                // Does not want to sell item.
                return false;
            }

            // Number of bids per item trying to sell depends on merchant skill (bartering) and cunning
            int numBids = Utilities.GetRandomNumber(2, 2 + seller.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Merchant) +
                                                            seller.CurrentStats.Cunning / 50);
            for (int i = 0; i < numBids; ++i)
            {
                // Check if the buyer wants to buy item. If so, sell item. Otherwise, continue bidding.
                if (this.preferenceEngine.UnitWantsToBuyItem(buyer, seller, item, bid))
                {
                    if (initiatorIsSeller)
                    {
                        // Seller sold goods... record results with seller as reference
                        result.AddExistingItemToItemsLost(item);
                        result.MoneyMade = result.MoneyMade.HasValue ? result.MoneyMade + bid : bid;
                    }
                    else
                    {
                        // Buyer bought goods... record results with buyer as reference
                        result.AddExistingItemToItemsGained(item);
                        result.MoneyLost = result.MoneyMade.HasValue ? result.MoneyMade + bid : bid;
                    }

                    seller.Inventory.RemoveItem(item);
                    seller.Inventory.Money += bid;
                                        
                    buyer.AcquireItem(item, initiatorIsSeller ? AcquiredItemSource.ItemWasSoldTo : AcquiredItemSource.BoughtItem);
                    buyer.Inventory.Money -= bid;

                    seller.TransactionHistory.SetOrAverage(item.ObjectName, bid);
                    buyer.TransactionHistory.SetOrAverage(item.ObjectName, bid);

                    //Console.WriteLine("Bidder {0} sold {1} to {2} for {3}", seller.DisplayName, item.DisplayName, buyer.DisplayName, bid);
                    return true; // seller sold the item... transaction over
                }

                //Console.WriteLine("Buyer {0} is unwilling to pay {1} for {2}", buyer.DisplayName, bid, item.DisplayName);

                bid = this.preferenceEngine.ImproveBid(seller, item, bid);
                if (bid == 0)
                {
                    //Console.WriteLine("Bidder {0} bailed on the sale of {1}", seller.DisplayName, item.DisplayName);
                    return false; // bidder bailed                    
                }                
            }

            // Did not buy
            return false;
        }

        protected virtual ActivityResult DetermineMineStoneResults(IMakeDecisions unit)
        {
            ActivityResult result = new ActivityResult();
            int modifier = unit.CurrentStats.Physical / 40;
            int numGathered = Utilities.GetRandomNumber(0, 2) + modifier;
            modifier += (unit.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Mining) - 1).MinCap(0) / 2; // Mining skill helps
            result.AddItemsToItemsGained(numGathered, "Stone");
            unit.AcquireItems(numGathered, "Stone", AcquiredItemSource.Gathered);            
            IEnumerable<string> loot = this.lootEngine.GetActivityLoot(unit, Decision.MineStone);
            unit.AcquireItems(loot, AcquiredItemSource.Gathered);
            result.AddItemsToItemsGained(loot);
            result.ActionPointCost = 2;
            return result;
        }

        protected virtual ActivityResult DetermineGetLumberResults(IMakeDecisions unit)
        {
            ActivityResult result = new ActivityResult();
            int modifier = unit.CurrentStats.Physical / 30;
            modifier += (unit.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Woodsman) - 1).MinCap(0) / 2; // Mining skill helps
            int numGathered = Utilities.GetRandomNumber(1, 2) + modifier;            
            result.AddItemsToItemsGained(numGathered, "Wood");
            unit.AcquireItems(numGathered, "Wood", AcquiredItemSource.Gathered);            
            IEnumerable<string> loot = this.lootEngine.GetActivityLoot(unit, Decision.GetLumber);
            unit.AcquireItems(loot, AcquiredItemSource.Gathered);
            result.AddItemsToItemsGained(loot);
            return result;
        }

        protected virtual ActivityResult DetermineHuntResults(IMakeDecisions unit)
        {
            ActivityResult result = new ActivityResult();            
            int modifier = unit.CurrentStats.Cunning / 30;
            modifier += (unit.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Woodsman) - 1).MinCap(0);            
            int numGathered = Utilities.GetRandomNumber(0, 1) + modifier;
            result.AddItemsToItemsGained(numGathered, "Leather");
            unit.AcquireItems(numGathered, "Leather", AcquiredItemSource.Gathered);
            IEnumerable<string> loot = this.lootEngine.GetActivityLoot(unit, Decision.Hunt);
            unit.AcquireItems(loot, AcquiredItemSource.Gathered);
            result.AddItemsToItemsGained(loot);
            result.ActionPointCost = 2;
            return result;
        }

        protected virtual ActivityResult DetermineForageResults(IMakeDecisions unit)
        {
            ActivityResult result = new ActivityResult();
            int modifier = unit.CurrentStats.Mental / 25;
            modifier += (unit.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Woodsman) - 1).MinCap(0) / 2;
            modifier += (unit.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Herbalism) - 1).MinCap(0);
            int numGathered = Utilities.GetRandomNumber(1, 2) + modifier;
            IEnumerable<string> loot = this.lootEngine.GetActivityLoot(unit, Decision.GetLumber, modifier);
            unit.AcquireItems(loot, AcquiredItemSource.Gathered);
            result.AddItemsToItemsGained(loot);
            result.ActionPointCost = 2;
            return result;
        }

        private ActivityResult DetermineMineOreResults(IMakeDecisions unit)
        {
            ActivityResult result = new ActivityResult();
            int modifier = unit.CurrentStats.Physical / 40;
            modifier += (unit.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Mining) - 2).MinCap(0) / 3; // Mining skill helps
            modifier += (unit.CurrentStats.Skills.GetSkillLevel(UnitSkills.SkillType.Appraisal) - 3).MinCap(0) / 3; // High appraisal helps
            int numGathered = Utilities.GetRandomNumber(0, 1) + modifier;
            result.AddItemsToItemsGained(numGathered, "IronOre");
            unit.AcquireItems(numGathered, "IronOre", AcquiredItemSource.Gathered);
            IEnumerable<string> loot = this.lootEngine.GetActivityLoot(unit, Decision.MineOre);
            unit.AcquireItems(loot, AcquiredItemSource.Gathered);
            result.AddItemsToItemsGained(loot);
            result.ActionPointCost = 2;
            return result;
        }
    }
}
