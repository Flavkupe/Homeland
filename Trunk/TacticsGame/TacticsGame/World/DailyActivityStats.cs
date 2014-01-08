using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;

namespace TacticsGame.World
{
    public class DailyActivityStats
    {
        private Dictionary<string, ItemOrder> itemsSold = new Dictionary<string, ItemOrder>();

        private Dictionary<string, ItemOrder> itemsBought = new Dictionary<string, ItemOrder>();

        private Dictionary<string, int> itemsCollected = new Dictionary<string, int>();

        private Dictionary<string, int> itemsCrafted = new Dictionary<string, int>();

        private List<ObjectValuePair<DecisionMakingUnit>> taxesPaid = new List<ObjectValuePair<DecisionMakingUnit>>();

        private List<ObjectValuePair<DecisionMakingUnit>> unitRevenue = new List<ObjectValuePair<DecisionMakingUnit>>();        

        private List<ObjectValuePair<DecisionMakingUnit>> unitExpenses = new List<ObjectValuePair<DecisionMakingUnit>>();

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

        public Dictionary<string, int> ItemsCrafted
        {
            get { return itemsCrafted; }
            set { itemsCrafted = value; }
        }

        public Dictionary<string, int> ItemsCollected
        {
            get { return itemsCollected; }
            set { itemsCollected = value; }
        }

        public Dictionary<string, ItemOrder> ItemsBought
        {
            get { return itemsBought; }
            set { itemsBought = value; }
        }

        public Dictionary<string, ItemOrder> ItemsSold
        {
            get { return itemsSold; }
            set { itemsSold = value; }
        }
    }
}
