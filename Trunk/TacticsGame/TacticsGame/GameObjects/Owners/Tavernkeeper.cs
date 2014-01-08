using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Buildings.Owners
{
    public class Tavernkeeper : Shopkeep
    {
        public Tavernkeeper()
        {
            this.ItemTypePreference = new Dictionary<ItemType, int>() { { ItemType.Food, 20 } };
        }

        public override bool OnlyBuysPreferredItemTypes { get { return true; } }
    }
}
