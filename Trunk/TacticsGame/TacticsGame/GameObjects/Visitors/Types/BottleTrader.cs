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

            this.ItemTypePreference.Add(ItemType.Scrap, 20);
            this.ItemTypePreference.Add(ItemType.Misc, 20);
            this.ItemPreference.Add("Bottle", -50);
            this.ItemPreference.Add("Vial", -50);
        }
    }
}
