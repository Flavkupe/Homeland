using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.EntityMetadata;
using TacticsGame.Items;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Buildings;

namespace TacticsGame.GameObjects.Owners
{
    /// <summary>
    /// Class for a building owner
    /// </summary>
    [Serializable] 
    public abstract class Owner : DecisionMakingUnit, IMakeDecisions
    {
        [NonSerialized]
        private Building ownedBuilding = null;

        private string ownedBuildingID = null;

        public Owner(string ownerType)
            : base(ownerType)
        {
            this.DisplayName = Utilities.GenerateRandomName(ownerType);
            this.BaseStats.ActionPoints = 6; // By default, owners get 6 AP per turn but buy/sell actions cost more for them
        }       

        protected override void InitializeEquipment()
        {
        }

        /// <summary>
        /// The building this guy owns.
        /// </summary>
        public Building OwnedBuilding
        {
            get { return ownedBuilding; }
            set 
            { 
                ownedBuilding = value;
                ownedBuildingID = value.ID;
            }
        }

        public override bool IsShopOwner { get { return true; } }

        public override bool UpgradesEquipment { get { return false; } }

        public override void RefreshStatsForNewManagementModeTurn()
        {
            this.CurrentStats.ActionPoints = this.BaseStats.ActionPoints;
        }

        /// <summary>
        /// Finds the building this referenced before serialization and references it, using ID. To be used after deserializing.
        /// </summary>
        public override void LoadReferencesFromLists(List<DecisionMakingUnit> units, List<Building> buildings)
        {
            base.LoadReferencesFromLists(units, buildings);

            foreach (Building building in buildings)
            {
                if (building.ID == this.ownedBuildingID)
                {
                    this.ownedBuilding = building; // found his building! how cute!
                    break;
                }
            }
        }
    }
}
