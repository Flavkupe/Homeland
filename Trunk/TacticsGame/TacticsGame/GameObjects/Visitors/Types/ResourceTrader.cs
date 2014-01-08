using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility;

namespace TacticsGame.GameObjects.Visitors.Types
{
    [Serializable]
    public class ResourceTrader : Merchant
    {
        public ResourceTrader()
            : base("Shopkeep") 
        {
            this.Inventory.AddItems(ItemGenerationUtilities.GetResourceAssortment(5));

            this.Preferences.GovernancePreference.TaxTolerance = Utilities.GetRandomNumber(30, 40);
        }


    }
}
