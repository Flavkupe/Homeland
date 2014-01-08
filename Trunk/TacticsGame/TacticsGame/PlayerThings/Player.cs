using System;
using System.Collections.Generic;
using TacticsGame.Items;
using TacticsGame.Managers;
using System.Linq;
using TacticsGame.GameObjects.Owners;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.PlayerThings
{
    [Serializable]
    public class Player : Owner
    {
        public Player()
            : this("Fool", "Player")
        {            
        }        

        public Player(string playerClass, string playerName)
            : base(playerClass)
        {
            this.DisplayName = playerName;

            this.inventory.Money = 1000;
            this.inventory.AddItem(new Item("Wood"));
            this.inventory.AddItem(new Item("Wood"));
            this.inventory.AddItem(new Item("Wood"));
            this.inventory.AddItem(new Item("Stone"));
            this.inventory.AddItem(new Item("Stone"));
            this.inventory.AddItem(new Item("Stone"));
        }

        public override Dictionary<string, int> ItemPreference
        {
            get
            {
                List<ItemOrder> orders = PlayerStateManager.Instance.ActiveTown.ItemOrders;

                // TODO: cache or optimize or something... maybe
                Dictionary<string, int> preference = new Dictionary<string,int>();                
                foreach (ItemOrder order in orders)
                {
                    Item item = new Item(order.ItemName);                    
                    int itemPref = 10 + order.Offer - item.Stats.Cost;
                    preference[item.ObjectName] = itemPref;
                }

                return preference;
            }
            protected set
            {
                base.ItemPreference = value;
            }
        }

        /// <summary>
        /// Get the item and remove it from the player's Orders
        /// </summary>
        /// <param name="item"></param>
        public override void AcquireItem(Item item, AcquiredItemSource source)
        {
            this.Inventory.AddItem(item);

            if (source == AcquiredItemSource.ItemWasSoldTo)
            {
                List<ItemOrder> orders = PlayerStateManager.Instance.ActiveTown.ItemOrders;
                if (orders.Count > 0)
                {
                    ItemOrder order = orders.First(a => a.ItemName == item.ObjectName);
                    if (order != null)
                    {
                        order.Amount--;
                        if (order.Amount <= 0)
                        {
                            orders.Remove(order);
                        }
                    }
                }
            }
        }

        public override bool IsPlayer { get { return true; } }

        public override void LoadContent()
        {
            base.LoadContent();            
            this.pictureFrame = TextureManager.Instance.GetTextureInfo("Frame_Fool", ResourceType.MiscObject);
        }

        protected override void InitializeAbilities()
        {
            base.InitializeAbilities();
        }

        protected override void InitializeStats()
        {
        }
    }
}
