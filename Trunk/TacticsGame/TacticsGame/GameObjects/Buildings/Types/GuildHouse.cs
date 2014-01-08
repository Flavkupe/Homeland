using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Managers;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Owners;
using TacticsGame.PlayerThings;

namespace TacticsGame.GameObjects.Buildings.Types
{    
    [Serializable]
    public class GuildHouse : Building
    {        
        public GuildHouse(int tileDimensions)
            : base("Building_Guildhouse", tileDimensions)
        {
            PlayerStateManager.Instance.Player.OwnedBuilding = this;
        }

        public GuildHouse()
            : base("Building_Guildhouse")
        {
            PlayerStateManager.Instance.Player.OwnedBuilding = this;
        }

        public List<ItemOrder> ItemOrders
        {
            get { return PlayerStateManager.Instance.ActiveTown.ItemOrders; }
            set { PlayerStateManager.Instance.ActiveTown.ItemOrders = value; }
        }        

        public override Inventory Stock
        {
            get { return PlayerStateManager.Instance.PlayerInventory; }
        }

        public override bool IsAutonomousBuilding { get { return false; } }

        public override bool IsBuildingWithStock { get { return true; } }

        public override bool IsBuildingWithOwner { get { return true; } }        

        public override bool IsBuildingWithUnits { get { return true; } }

        public override bool IsBuildingThatSellsThings { get { return false; } }

        public override bool CanDeployTroopsToBuilding { get { return true; } }

        public override int UnitCapacity { get { return 4; } }

        public override Owner Owner { get { return PlayerStateManager.Instance.Player; } }

        /// <summary>
        /// Whether this shop has orders that need to be filled.
        /// </summary>
        public override bool IsBuildingThatBuysThings
        {
            get 
            {
                foreach (ItemOrder order in this.ItemOrders)
                {
                    if (this.Stock.GetItemCount(order.ItemName) < order.Amount)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
