using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Units
{
    [Serializable]
    public abstract class HireableUnit : DecisionMakingUnit, ICanBeHired
    {
        public HireableUnit(string unitType)
            : base(unitType)
        {
            this.Preferences.GovernancePreference.TaxTolerance = Utilities.GetRandomNumber(0, 50);
        }

        /// <summary>
        /// Should never be random.
        /// </summary>
        public virtual int GetHireCost()
        {
            return 100;
        }

        /// <summary>
        /// Whether this unit cares about equipment at all. Hireable units do because they can improve their gear.
        /// </summary>
        public override bool UpgradesEquipment { get { return true; } }

        public override bool IsTrader { get { return false; } }

        protected override void InitializePreferences()
        {
            base.InitializePreferences();

            this.ItemTypePreference.Add(ItemType.Scrap, -50);
            this.ItemTypePreference.Add(ItemType.Resource, -30);
            this.ItemTypePreference.Add(ItemType.Commodity, -40);
            this.ItemTypePreference.Add(ItemType.Misc, -50);
            this.ItemTypePreference.Add(ItemType.Ingredient, -30);
            this.ItemTypePreference.Add(ItemType.Food, 5);
            //this.ItemTypePreference.Add(ItemType.Luxury, 20);
        }
    }
}
