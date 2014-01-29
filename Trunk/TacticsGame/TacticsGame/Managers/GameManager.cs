using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.World;

namespace TacticsGame.Managers
{
    public class GameManager
    {
        private static UnitActionManager unitActionManager = new UnitActionManager();

        public static GameResourceManager GameResourceManager { get { return TacticsGame.Managers.GameResourceManager.Instance; } }
        public static GameSerializer GameSerializer { get { return TacticsGame.Managers.GameSerializer.Instance; } }
        public static GameStateManager GameStateManager { get { return TacticsGame.Managers.GameStateManager.Instance; } }
        public static InterfaceManager InterfaceManager { get { return TacticsGame.Managers.InterfaceManager.Instance; } }
        public static ItemManager ItemManager { get { return TacticsGame.Managers.ItemManager.Instance; } }
        public static PlayerStateManager PlayerStateManager { get { return TacticsGame.Managers.PlayerStateManager.Instance; } }
        public static TextureManager TextureManager { get { return TacticsGame.Managers.TextureManager.Instance; } }

        public static GameWorld World { get { return GameStateManager.World; } }

        public static UnitActionManager UnitActionManager
        {
            get { return GameManager.unitActionManager; }
        }
    }
}
