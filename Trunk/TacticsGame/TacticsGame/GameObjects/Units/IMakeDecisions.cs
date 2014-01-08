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

        UnitStats CurrentStats { get; set; }        

        Equipment Equipment { get; set; }

        List<Recipe> KnownRecipes { get; }

        void RefreshStatsForNewManagementModeTurn();

        int PriceMarkupRange { get; set; }

        ActivityResult LastResult { get; set; }

        bool UpgradesEquipment { get; }

        bool WantsToUpgradeWeapon { get; }

        bool WantsToUpgradeArmor { get; }

        void AcquireItem(Item item, AcquiredItemSource source);

        void AcquireItems(int number, string itemName, AcquiredItemSource source);

        void AcquireItems(IEnumerable<string> items, AcquiredItemSource source);

        /// <summary>
        /// Whether only items in ItemTypePreference are bought. If it's null, all items are bought. If empty, none are bought.
        /// </summary>
        bool OnlyBuysPreferredItemTypes { get; }

        bool IsTrader { get; }

        Preferences Preferences { get; }

        Dictionary<string, int> TransactionHistory { get; }

        /// <summary>
        /// Rating this unit gives to willingness to buy certain item types.
        /// </summary>
        Dictionary<ItemType, int> ItemTypePreference { get; }

        /// <summary>
        /// Rating this unit gives to willingness to buy specific items.
        /// </summary>
        Dictionary<string, int> ItemPreference { get; }
    }
}
