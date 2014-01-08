using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;
using TacticsGame.GameObjects.Units;
using TacticsGame.PlayerThings;
using TacticsGame.Utility.Classes;
using TacticsGame.World;

namespace TacticsGame.Managers
{
    /// <summary>
    /// Singleton to manage player stuff like total money and player inventory.
    /// </summary>
    public class PlayerStateManager : Singleton<PlayerStateManager>
    {
        private PlayerInfo playerInfo = null;        

        private PlayerStateManager()
        {
            this.playerInfo = new PlayerInfo();            
        }

        public TownState ActiveTown { get; set;}

        /// <summary>
        /// Loads the stuff for player.
        /// </summary>
        public void LoadPlayerContents()
        {
            playerInfo.Avatar.LoadContent();
        }

        /// <summary>
        /// Reference to the player's character.
        /// </summary>
        public Player Player
        {
            get { return playerInfo.Avatar; }
            set { playerInfo.Avatar = value; }
        }

        public Inventory PlayerInventory
        {
            get { return this.Player.Inventory; }
            private set { this.Player.Inventory = value; }
        }        
    }
}
