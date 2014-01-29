using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Visitors.Types
{
    [Serializable]
    public class BottleTrader : Merchant
    {
        public BottleTrader()
            : base("BottleTrader") 
        {
            this.Inventory.AddItems(ItemGenerationUtilities.GetBottleTraderAssortment(Utilities.GetRandomNumber(5,10)));

            this.Preferences.GovernancePreference.TaxTolerance = Utilities.GetRandomNumber(10, 30);

            this.Inventory.Money = Utilities.GetRandomNumber(200, 400);

            this.Preferences.ItemPreference.SetPreference(ItemType.Scrap, 20);
            this.Preferences.ItemPreference.SetPreference(ItemType.Misc, 20);
            this.Preferences.ItemPreference.SetPreference("Bottle", -50);
            this.Preferences.ItemPreference.SetPreference("Vial", -50);
        }
    }
}
