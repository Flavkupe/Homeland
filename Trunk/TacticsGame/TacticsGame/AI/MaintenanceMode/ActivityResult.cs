using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.Utility;
using TacticsGame.GameObjects.Visitors;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.AI.MaintenanceMode
{
    /// <summary>
    /// Results of doing an action
    /// </summary>
    [Serializable]
    public class ActivityResult
    {
        public ActivityResult()
        {
            this.ActionPointCost = 1;
        }

        private bool actionSuccess = true;

        public CountingDictionary<Item> GainedItemsCounter = new CountingDictionary<Item>();
        public CountingDictionary<Item> LostItemsCounter = new CountingDictionary<Item>();        

        public int ActionPointCost { get; set; }

        public int? MoneyMade { get; set; }

        public int? MoneyLost { get; set; }

        public List<Item> ItemsGained { get; set; }

        public List<Item> ItemsLost { get; set; }

        public bool UnitIsDone { get; set; }        

        public bool ActionSuccess
        {
            get { return actionSuccess; }
            set { actionSuccess = value; }
        }

        public Building BuildingTarget { get; set; }

        public DecisionMakingUnit VisitorTarget { get; set; }

        public IMakeDecisions TransactionTarget { get; set; }

        public void AddItemsToList(List<Item> list, int numGathered, string item, CountingDictionary<Item> counter = null)
        {
            if (numGathered > 0)
            {
                for (int i = 0; i < numGathered; ++i)
                {
                    Item newItem = new Item(item);
                    list.Add(newItem);

                    if (counter != null)
                    {
                        counter.AddItemUsingName(newItem);
                    }
                }
            }
        }

        private void AddExistingItemToList<T>(List<T> list, T item, CountingDictionary<T> counter = null)
        {           
            list.Add(item);
            
            if (counter != null)
            {
                counter.AddItemUsingName(item);
            }
        }

        public void AddExistingItemToItemsGained(Item item)
        { 
            if (this.ItemsGained == null)
            {
                this.ItemsGained = new List<Item>();
            }

            this.AddExistingItemToList<Item>(this.ItemsGained, item, this.GainedItemsCounter);
        }

        public void AddExistingItemToItemsLost(Item item)
        {
            if (this.ItemsLost == null)
            {
                this.ItemsLost = new List<Item>();
            }

            this.AddExistingItemToList<Item>(this.ItemsLost, item, this.LostItemsCounter);
        }

        public void AddItemsToItemsGained(IEnumerable<string> items)
        {
            if (this.ItemsGained == null)
            {
                this.ItemsGained = new List<Item>();
            }

            foreach (string item in items)
            {
                AddItemsToList(this.ItemsGained, 1, item, this.GainedItemsCounter);
            }
        }

        public void AddItemsToItemsGained(int numGathered, string item)
        {
            if (this.ItemsGained == null)
            {
                this.ItemsGained = new List<Item>();
            }

            AddItemsToList(this.ItemsGained, numGathered, item, this.GainedItemsCounter);
        }

        public void AddItemsToItemsLost(int numGathered, string item)
        {
            if (this.ItemsLost == null)
            {
                this.ItemsLost = new List<Item>();
            }

            AddItemsToList(this.ItemsLost, numGathered, item, this.LostItemsCounter);
        }
    }
}
