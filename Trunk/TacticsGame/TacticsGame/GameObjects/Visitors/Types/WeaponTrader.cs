using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility;
using TacticsGame.Managers;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Visitors.Types
{
    [Serializable]
    public class WeaponTrader : Merchant
    {
        public WeaponTrader()
            : base("WeaponTrader") 
        {
            List<Item> weapons = ItemManager.Instance.GenerateWeaponList(Rarity.Uncommon | Rarity.Scrap | Rarity.Common, null).GetRandomPick(Utilities.GetRandomNumber(3, 6));
            List<Item> armor = ItemManager.Instance.GenerateArmorList(Rarity.Uncommon | Rarity.Scrap | Rarity.Common, null, null).GetRandomPick(Utilities.GetRandomNumber(5, 8));
           
            this.Inventory.AddItems(weapons);
            this.Inventory.AddItems(armor);

            this.Preferences.GovernancePreference.TaxTolerance = Utilities.GetRandomNumber(30, 40);
        }


    }
}
