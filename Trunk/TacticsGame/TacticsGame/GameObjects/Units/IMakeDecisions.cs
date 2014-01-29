using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.EntityMetadata;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;
using TacticsGame.Crafting;
using TacticsGame.Preference;

namespace TacticsGame.GameObjects.Units
{
    public interface IMakeDecisions : IHasInventory
    {
        string DisplayName { get; }

        bool IsMakingADecision { get; set; }

        bool CanMakeDecision { get; }

        Decision PreviousDecision { get; set; }

        UnitStats BaseStats { get; set; }

        UnitStats CurrentStats { get; set; }        

        Equipment Equipment { get; set; }

        List<Recipe> KnownRecipes { get; }

        void RefreshStatsForNewManagementModeTurn();

        ActivityResult LastResult { get; set; }

        bool UpgradesEquipment { get; }

        bool WantsToUpgradeWeapon { get; }

        bool WantsToUpgradeArmor { get; }

        void AcquireItem(Item item, AcquiredItemSource source);

        void AcquireItems(int number, string itemName, AcquiredItemSource source);

        void AcquireItems(IEnumerable<string> items, AcquiredItemSource source);

        bool IsTrader { get; }

        Preferences Preferences { get; }

        Dictionary<string, int> TransactionHistory { get; }
    }
}
