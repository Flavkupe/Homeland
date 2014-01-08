using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;

namespace TacticsGame.World.Caravans
{
    [Serializable]
    public class Caravan
    {
        private int currentDay = 0;

        private int arrivalDay = 1;
        
        private Inventory cargo = new Inventory();

        private Dictionary<string, ItemOrder> itemPrices = new Dictionary<string, ItemOrder>();        

        private ForeignTownInfo targetTown;

        public int ArrivalDay
        {
            get { return arrivalDay; }
            set { arrivalDay = value; }
        }

        public int CurrentDay
        {
            get { return currentDay; }
            set { currentDay = value; }
        }        

        public ForeignTownInfo TargetTown
        {
            get { return targetTown; }
            set { targetTown = value; }
        }        

        public Inventory Cargo
        {
            get { return cargo; }
            set { cargo = value; }
        }

        public void SetItemPrice(ItemOrder itemPrice)
        {
            this.itemPrices[itemPrice.ItemName] = itemPrice;
        }

        public ItemOrder GetItemPrice(string itemName)
        {
            return this.itemPrices.ContainsKey(itemName) ? this.itemPrices[itemName] : null;
        }

    }
}
