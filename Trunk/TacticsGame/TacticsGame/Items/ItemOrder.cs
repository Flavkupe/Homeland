using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Items
{
    [Serializable]
    public class ItemOrder
    {
        private string itemName;

        private int amount;

        private int offer;

        private ItemOrderType orderType;

        public ItemOrder(string itemType, int amount, int offer, ItemOrderType orderType)
        {
            this.itemName = itemType;
            this.amount = amount;
            this.offer = offer;
            this.orderType = orderType;
        }

        /// <summary>
        /// Gets amount of items for order.
        /// </summary>
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        /// <summary>
        /// Gets type of item for order.
        /// </summary>
        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public ItemOrderType OrderType
        {
            get { return orderType; }
            set { orderType = value; }
        }

        /// <summary>
        /// Offer per item. 
        /// </summary>
        public int Offer
        {
            get { return offer; }
            set { offer = value; }
        }

        public enum ItemOrderType
        {
            Buying,
            Selling
        }

        public void SetTo(ItemOrder item)
        {
            this.Offer = item.Offer;
            this.ItemName = item.ItemName;
            this.OrderType = item.OrderType;
            this.Amount = item.Amount;
        }
    }
}
