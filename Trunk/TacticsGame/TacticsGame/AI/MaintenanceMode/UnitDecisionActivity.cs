using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using Microsoft.Xna.Framework;
using TacticsGame.GameObjects.Buildings;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using TacticsGame.GameObjects.Visitors;


namespace TacticsGame.AI.MaintenanceMode
{
    [Serializable]
    public class UnitDecisionActivity
    {                
        private float progress = 0.0f;

        private bool doneForTurn;

        [NonSerialized]
        private DecisionMakingUnit unit;

        private string unitId;

        private Decision decision;

        [NonSerialized]
        private Building targetBuilding;       

        private string targetBuildingId;

        [NonSerialized]
        private DecisionMakingUnit targetVisitor;

        private string targetVisitorId;

        private float expectedDuration;

        public UnitDecisionActivity(DecisionMakingUnit unit, Decision decision, float duration)
        {
            this.Unit = unit;
            this.Decision = decision;
            this.ExpectedDuration = duration;
            this.DoneForTurn = false;                 
        }

        /// <summary>
        /// Whether this unit is done until the start of the next round.
        /// </summary>       
        public bool DoneForTurn
        {
            get { return doneForTurn; }
            set { doneForTurn = value; }
        }

        /// <summary>
        /// Unit this activity applies to
        /// </summary>
        public DecisionMakingUnit Unit
        {
            get { return unit; }
            set 
            { 
                this.unit = value;
                this.unitId = value.ID;
            }
        }

        /// <summary>
        /// Building involved in current decision
        /// </summary>
        public Building TargetBuilding
        {
            get { return targetBuilding; }
            set 
            { 
                this.targetBuilding = value;
                this.targetBuildingId = value == null ? null : value.ID;                
            }
        }

        /// <summary>
        /// The visitor this unit may be targetting
        /// </summary>
        public DecisionMakingUnit TargetVisitor
        {
            get { return targetVisitor; }
            set 
            { 
                this.targetVisitor = value;
                this.targetVisitorId = value == null ? null : value.ID;
            }
        }

        /// <summary>
        /// Current decision
        /// </summary>
        public Decision Decision
        {
            get { return decision; }
            set { decision = value; }
        }

        /// <summary>
        /// How long activity is expected to last for
        /// </summary>
        public float ExpectedDuration
        {
            get { return expectedDuration; }
            private set { expectedDuration = value; }
        }

        /// <summary>
        /// Whether the action is complete
        /// </summary>
        public bool Complete { get { return this.progress >= this.ExpectedDuration; } }

        /// <summary>
        /// Gets the percent expected to complete the process.
        /// </summary>
        public float PercentComplete { get { return this.progress / this.ExpectedDuration; } }

        public virtual void Update(GameTime gameTime, int tickDuration = 1000) 
        {
            progress += gameTime == null ? tickDuration : ((float)gameTime.ElapsedGameTime.Milliseconds / (float)tickDuration);

            // TODO: progress based on unit?                         
        }

        /// <summary>
        /// Updates for a new activity for this unit.
        /// </summary>
        public void NewDecision(MaintenanceMode.Decision decision, int decisionDuration)
        {
            this.Decision = decision;
            this.ExpectedDuration = decisionDuration;
            this.progress = 0.0f;
        }

        /// <summary>
        /// Gets the icon associated with the activity
        /// </summary>
        /// <returns></returns>
        public virtual IconInfo GetActivityIcon()
        {
            switch (this.Decision)
            {
                case MaintenanceMode.Decision.GetLumber:
                    return TextureManager.Instance.GetIconInfo("Wood");
                case MaintenanceMode.Decision.MineStone:
                    return TextureManager.Instance.GetIconInfo("Stone");
                case MaintenanceMode.Decision.MineOre:
                    return TextureManager.Instance.GetIconInfo("IronOre");
                case MaintenanceMode.Decision.Hunt:
                    return TextureManager.Instance.GetIconInfo("BasicBow");
                case MaintenanceMode.Decision.Forage:
                    return TextureManager.Instance.GetIconInfo("HerbCluster");
                case MaintenanceMode.Decision.RestAtHome:
                    return TextureManager.Instance.GetTextureAsIconInfo("Building_Shop", ResourceType.GameObject);
                case MaintenanceMode.Decision.Buy:
                    return TextureManager.Instance.GetIconInfo("Buy");
                case MaintenanceMode.Decision.Sell:
                    return TextureManager.Instance.GetIconInfo("Sell");
                case MaintenanceMode.Decision.Idle:                    
                default:
                    return null;
            }                        
        }

        #region LoadReferences
        

        /// <summary>
        /// Given the proper lists, populates the references. For use when loading serialized data.
        /// </summary>
        /// <param name="units"></param>
        /// <param name="buildings"></param>
        public virtual void LoadReferencesFromLists(List<DecisionMakingUnit> units, List<Building> buildings)
        {
            if (this.unitId != null)
            {
                foreach (DecisionMakingUnit unit in units)
                {
                    if (unit.ID == this.unitId)
                    {
                        this.unit = unit;
                        break;
                    }
                }

                Debug.Assert(this.unit != null, "failed to load serialized activity data for a unit");
            }
            else
            {
                Debug.Assert(false, "this should not happen");
            }
           
            if (this.targetBuildingId != null)
            {
                foreach (Building building in buildings)
                {
                    if (building.ID == this.targetBuildingId)
                    {
                        this.targetBuilding = building;
                        break;
                    }
                }

                Debug.Assert(this.TargetBuilding != null, "failed to load serialized activity data for a building");
            }

            if (this.targetVisitorId != null)
            {
                foreach (Building building in buildings)
                {
                    if (building.Visitors != null)
                    {
                        foreach (Visitor visitor in building.Visitors)
                        {
                            if (visitor.ID == this.targetVisitorId)
                            {
                                this.targetVisitor = visitor;
                                break;
                            }
                        }
                    }
                }

                Debug.Assert(this.TargetVisitor != null, "failed to load serialized activity data for a visitor");
            }
        }

        #endregion
    }
}
