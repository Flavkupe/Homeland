using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using System.Collections.ObjectModel;
using TacticsGame.Utility;
using System.Runtime.Serialization;

namespace TacticsGame.Items
{
    [Serializable]
    public class Inventory
    {
        public Inventory()
        {
            this.Money = 0;
            this.MaxRepetition = 0;
        }

        private CountingDictionary<Item> itemCounter = new CountingDictionary<Item>();            

        /// <summary>
        /// Money held
        /// </summary>
        public int Money { get; set;}

        /// <summary>
        /// Max of a single item this unit has.
        /// </summary>
        public int MaxRepetition { get; set; }

        public void AddItem(Item item)
        {
            // TODO: handle full items
            this.items.Add(item);
            this.itemCounter.AddItemAsString(item.ObjectName);
        }

        public void AddItems(IEnumerable<Item> items)
        {
            this.items.AddRange(items);
            items.ToList<Item>().ForEach(a => this.itemCounter.AddItemAsString(a.ObjectName));
        }

        public void RemoveItems(IEnumerable<Item> list)
        {
            this.items.RemoveAll(a => list.Contains(a));
            this.itemCounter.RemoveItems(list);
        }

        public void RemoveItem(Item item)
        {
            this.items.Remove(item);
            this.itemCounter.RemoveItem(item);
        }

        private List<Item> items = new List<Item>();

        public ReadOnlyCollection<Item> Items
        {
            get { return new ReadOnlyCollection<Item>(items); }
            set { this.items = new List<Item>(value); }
        }

        public int GetMaxItemCount()
        {
            return this.itemCounter.GetMaxValue();
        }

        /// <summary>
        /// Gets number of the item.
        /// </summary>
        /// <param name="item">Name of the item.</param>
        /// <returns>Number of the item.</returns>
        public int GetItemCount(string item)
        {
            return this.itemCounter.GetItemCount(item);
        }

        public int GetItemCount(ItemMetadata itemMetadata)
        {
            return this.items.Count(a => a.HasMetadata(itemMetadata));
        }

        public int GetItemCount(Item item)
        {
            return this.GetItemCount(item.ObjectName);
        }

        public Inventory Clone()
        {
            Inventory newInv = new Inventory();
            newInv.Money = this.Money;
            newInv.items = new List<Item>();
            newInv.itemCounter = new CountingDictionary<Item>();
            foreach (Item item in this.items)
            {
                newInv.AddItem(item);
            }

            return newInv;
        }

        /// <summary>
        /// Removes "numberToRemove" items with name "itemName"
        /// </summary>
        /// <param name="itemName">Name of item</param>
        /// <param name="numberToRemove">How many to remove.</param>
        public void RemoveItems(string itemName, int numberToRemove)
        {
            foreach (Item item in this.Items.ToList<Item>())
            {
                if (numberToRemove == 0)
                {
                    return;
                }                

                if (item.ObjectName == itemName)
                {
                    this.RemoveItem(item);
                    numberToRemove--;                   
                }
            }
        }

        public void LoadContent()
        {
            foreach (Item item in this.Items)
            {
                item.LoadContent();
            }
        }

        public bool HasItem(Item wantedItem)
        {
            return this.items.Any(item => item.ObjectName == wantedItem.ObjectName);
        }

        public bool HasItem(string wantedItem)
        {
            return this.items.Any(item => item.ObjectName == wantedItem);
        }
    }
}
