
namespace TacticsGame.World
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;

    public class TownMarketManager : ITownMarketManager
    {
        private IPreferenceEngine preferenceEngine = null;

        public TownMarketManager(IPreferenceEngine preferenceEngine)
        {
            this.preferenceEngine = preferenceEngine;
        }

        public DecisionMakingUnit GetActualBestDealVendorByItem(List<DecisionMakingUnit> units, Item wantedItem) 
        {
            int minCost = int.MaxValue; 
            List<DecisionMakingUnit> validUnits = units.Where(vendor => vendor.IsTrader && vendor.Inventory.HasItem(wantedItem)).ToList();
            if (validUnits.Count == 0)
            {
                return null;
            }

            List<Tuple<int, DecisionMakingUnit>> costs = new List<Tuple<int, DecisionMakingUnit>>();
            foreach(DecisionMakingUnit unit in validUnits) 
            {
                int cost = this.preferenceEngine.ExpectedSellValue(unit, wantedItem);
                costs.Add(new Tuple<int, DecisionMakingUnit>(cost, unit));    
                minCost = Math.Min(minCost, cost);            
            }

            return costs.Where(a => a.Item1 <= minCost).GetRandomItem().Item2;             
        }

        public DecisionMakingUnit GetActualBestBuyerByItem(List<DecisionMakingUnit> units, string wantedItem)
        {
            int minCost = int.MaxValue;
            List<DecisionMakingUnit> validUnits = units.Where(vendor => vendor.IsTrader && vendor.Inventory.HasItem(wantedItem)).ToList();
            if (validUnits.Count == 0)
            {
                return null;
            }

            List<Tuple<int, DecisionMakingUnit>> costs = new List<Tuple<int, DecisionMakingUnit>>();
            foreach (DecisionMakingUnit unit in validUnits)
            {
                int cost = this.preferenceEngine.ExpectedSellValue(unit, new Item(wantedItem));
                costs.Add(new Tuple<int, DecisionMakingUnit>(cost, unit));
                minCost = Math.Min(minCost, cost);
            }

            return costs.Where(a => a.Item1 <= minCost).GetRandomItem().Item2;
        }
    }
}
