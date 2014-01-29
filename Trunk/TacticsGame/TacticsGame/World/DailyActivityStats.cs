using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;
using TacticsGame.Simulation;

namespace TacticsGame.World
{
    public class DailyActivityStats
    {
        private List<Item> itemsSoldByUnits = new List<Item>();
                
        private List<Item> itemsSoldByShops = new List<Item>();

        private List<Item> itemsBoughtByUnits = new List<Item>();

        private List<Item> itemsBoughtByShops = new List<Item>();

        private List<Item> itemsCollected = new List<Item>();

        private List<Item> itemsCrafted = new List<Item>();

        private List<ObjectValuePair<DecisionMakingUnit>> taxesPaid = new List<ObjectValuePair<DecisionMakingUnit>>();        

        private List<ObjectValuePair<DecisionMakingUnit>> unitRevenue = new List<ObjectValuePair<DecisionMakingUnit>>();        

        private List<ObjectValuePair<DecisionMakingUnit>> unitExpenses = new List<ObjectValuePair<DecisionMakingUnit>>();

        public List<ObjectValuePair<DecisionMakingUnit>> TaxesPaid
        {
            get { return taxesPaid; }
            set { taxesPaid = value; }
        }

        public List<ObjectValuePair<DecisionMakingUnit>> UnitRevenue
        {
            get { return unitRevenue; }
            set { unitRevenue = value; }
        }

        public List<ObjectValuePair<DecisionMakingUnit>> UnitExpenses
        {
            get { return unitExpenses; }
            set { unitExpenses = value; }
        }        

        public List<Item> ItemsCrafted
        {
            get { return itemsCrafted; }
            set { itemsCrafted = value; }
        }

        public List<Item> ItemsCollected
        {
            get { return itemsCollected; }
            set { itemsCollected = value; }
        }

        public List<Item> ItemsBoughtByUnits
        {
            get { return itemsBoughtByUnits; }
            set { itemsBoughtByUnits = value; }
        }

        public List<Item> ItemsSoldByUnits
        {
            get { return itemsSoldByUnits; }
            set { itemsSoldByUnits = value; }
        }

        public List<Item> ItemsSoldByShops
        {
            get { return itemsSoldByShops; }
            set { itemsSoldByShops = value; }
        }

        public List<Item> ItemsBoughtByShops
        {
            get { return itemsBoughtByShops; }
            set { itemsBoughtByShops = value; }
        }

        public int VisitorTaxesCollected { get; set; }

        public int DailyTaxesCollected { get; set; }

        public int SalesTaxesCollected { get; set; }

        public void Update(UnitActivityUpdateStatus activityStatus)
        {
            DecisionMakingUnit unit = activityStatus.Activity.Unit;
            UnitManagementActivity activity = activityStatus.Activity;
            if (activityStatus.ShouldAnnounceActivityResults && activityStatus.Results != null)
            {
                ActivityResult results = activityStatus.Results;
                if (unit.IsShopOwner) 
                {
                    if (activity.Decision == Decision.Buy && results.ItemsGained != null)
                    {
                        this.ItemsBoughtByShops.AddRange(results.ItemsGained);
                    }
                    else if (activity.Decision == Decision.Sell && results.ItemsLost != null)
                    {
                        this.ItemsSoldByShops.AddRange(results.ItemsLost);
                    }
                    else if (activity.Decision == Decision.Craft && results.ItemsGained != null) 
                    {
                        this.ItemsCrafted.AddRange(results.ItemsGained);
                    }
                }
                else if (unit.IsVisitor)
                {
                }
                else
                {
                    if (activity.Decision == Decision.Buy && results.ItemsGained != null)
                    {
                        this.ItemsBoughtByUnits.AddRange(results.ItemsGained);
                    }
                    else if (activity.Decision == Decision.Sell && results.ItemsLost != null)
                    {
                        this.ItemsSoldByUnits.AddRange(results.ItemsLost);
                    }
                    else if (results.ItemsGained != null)
                    {
                        this.ItemsCollected.AddRange(results.ItemsGained);
                    }
                }                                
            }
        }
    }
}
