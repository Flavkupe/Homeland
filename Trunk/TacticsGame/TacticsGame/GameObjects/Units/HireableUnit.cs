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

            this.Preferences.ItemPreference.SetPreference(ItemType.Scrap, -50);
            this.Preferences.ItemPreference.SetPreference(ItemType.Resource, -30);
            this.Preferences.ItemPreference.SetPreference(ItemType.Commodity, -40);
            this.Preferences.ItemPreference.SetPreference(ItemType.Misc, -50);
            this.Preferences.ItemPreference.SetPreference(ItemType.Ingredient, -30);
            this.Preferences.ItemPreference.SetPreference(ItemType.Food, 5);
            //this.ItemTypePreference.Add(ItemType.Luxury, 20);
        }
    }
}
