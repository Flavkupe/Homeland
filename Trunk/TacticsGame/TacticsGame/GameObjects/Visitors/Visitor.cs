using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.EntityMetadata;
using TacticsGame.GameObjects.Visitors.Types;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.Managers;

namespace TacticsGame.GameObjects.Visitors
{
    [Serializable]
    public abstract class Visitor : DecisionMakingUnit, IMakeDecisions
    {
        public Visitor(string visitorType)
            : base(visitorType)
        {
            this.DisplayName = Utilities.GenerateRandomName(visitorType);
        }

        public override void RefreshStatsForNewManagementModeTurn()
        {
        }       

        public virtual bool SellsItems { get { return false; } }
        public virtual bool BuysItems { get { return false; } }

        protected override void InitializeEquipment()
        {
            this.Preferences.ItemPreference.OnlyBuysPreferredItemTypes = true;
        }

        public static Visitor CreateRandomVisitor()
        {
            Type[] types = new Type[] { typeof(Merchant), typeof(Traveller), typeof(ResourceTrader), typeof(JunkCollector), typeof(WeaponTrader) };
            Type selection = types.GetRandomItem();
            return Activator.CreateInstance(selection) as Visitor;
        }        

        public override void LoadContent()
        {
            base.LoadContent();
            this.inventory.LoadContent();
        }

        public override bool WantsToUpgradeWeapon
        {
            get { return false; }
        }

        public override bool WantsToUpgradeArmor
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether or not the unit is a shopowner. A shopowner is a unit that owns inventory in a building.
        /// </summary>
        public virtual bool IsVisitor { get { return true; } }
    }
}
