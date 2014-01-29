using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Visitors.Types
{
    [Serializable]
    public class JunkCollector : Merchant
    {
        public JunkCollector()
            : base("JunkCollector") 
        {
            this.Inventory.AddItems(ItemGenerationUtilities.GetJunkAssortment(Utilities.GetRandomNumber(3,6)));
            this.Inventory.AddItems(ItemGenerationUtilities.GetResourceAssortment(Utilities.GetRandomNumber(0, 2)));
            this.Inventory.AddItems(ItemGenerationUtilities.GetBottleTraderAssortment(Utilities.GetRandomNumber(0, 2)));

            this.BaseStats.Mental = Utilities.GetRandomNumber(0, 20);
            this.Inventory.Money = Utilities.GetRandomNumber(200, 400);

            this.Preferences.ItemPreference.SetPreference(ItemType.Scrap, 200);
            this.Preferences.ItemPreference.SetPreference(ItemType.Resource, -50);
            this.Preferences.ItemPreference.SetPreference(ItemType.Misc, 50);

            this.Preferences.GovernancePreference.TaxTolerance = Utilities.GetRandomNumber(10, 30);
        }
    }
}
