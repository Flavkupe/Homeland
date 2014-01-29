using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Visitors;
using Microsoft.Xna.Framework;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame;
using TacticsGame.Scene;

namespace TestUtility.Simulation
{
    public class SimulationScene : ZoneManagementScene
    {
        public event EventHandler TurnDone;

        public event EventHandler<EventArgsEx<string>> SendMessage;

        protected bool turnDone = false;

        protected UneducatedDecisionEngine decisionEngine = null;
        protected UneducatedResultEngine decisionResultEngine = null;
        protected UnitDecisionUtils decisionUtils = null;        

        protected List<Visitor> visitors = new List<Visitor>();

        ////////////////////////////////
        #region Methods to override away        

        protected override int GetIdleTime()
        {
            return 1;
        }

        protected override int GetActivityDurationTime()
        {
            return 1;
        }

        protected override void UpdateCommandPaneText(string text)
        {            
        }

        protected override void AddActivityToUIFeed(UnitDecisionActivity activity)
        {            
        }

        protected override void ClearUIHandlers()
        {            
        }

        protected override void SetUIHandlers()
        {
        }

        protected override void ShowActivityResultsOnActionFeed(UnitDecisionActivity activity, ActivityResult results)
        {            
        }

        protected override void UpdateActivityOnUI(UnitDecisionActivity activity)
        {            
        }

        protected override void HandleMouseAndKeyboardInputs()
        {            
        }

        protected override void HandleClickedOnATile(object sender, TacticsGame.Map.TileGrid.TileClickedEventArgs e)
        {            
        }

        public override void DrawUI(GameTime gameTime)
        {            
        }

        protected override void HandleScrollInputs()
        {            
        }

        protected override void InitializeUI()
        {            
        }

        protected override void InitializeScene()
        {            
        }        

        #endregion
        ////////////////////////////////

        public SimulationScene()
        {
            this.LoadDecisionEngines();
        }

        protected virtual void LoadDecisionEngines()
        {
            decisionEngine = new UneducatedDecisionEngine();
            decisionUtils = new UnitDecisionUtils();
            decisionResultEngine = new UneducatedResultEngine();
        }

        /// <summary>
        /// Send a message to whomever is listening.
        /// </summary>
        protected virtual void SendFeedbackMessage(string message)
        {
            if (this.SendMessage != null)
            {
                this.SendMessage(this, new EventArgsEx<string>(message));
            }
        }

        public virtual void SetupScene() 
        {
            this.DecisionMakingUnits.Clear();
            this.Buildings.Clear();
            this.visitors.Clear();
            this.decisionActivities.Clear();

            this.DecisionMakingUnits.AddRange(SimulationObjects.Instance.GetEntities<DecisionMakingUnit>("DecisionMakingUnit"));
            this.visitors.AddRange(SimulationObjects.Instance.GetEntities<Visitor>("Visitor"));
            this.Buildings.AddRange(SimulationObjects.Instance.GetEntities<Building>("Building"));
            


            this.ResetTurn();
        }

        public override void Update(GameTime time) 
        {
            this.DecisionMakingUnits.ForEach(a => a.Update(null));
            this.Buildings.ForEach(a => a.Update(null));
            this.visitors.ForEach(a => a.Update(null));

            if (this.decisionActivities.All(a => a.DoneForTurn))
            {
                this.FinishedTurn();
            }

            this.ProcessUnitAndOwnerDecisions(time);
        }

        private void FinishedTurn()
        {
            this.turnDone = true;

            if (this.TurnDone != null)
            {
                this.TurnDone(this, new EventArgs());
            }
        }

        /// <summary>
        /// Runs a step of the simulation... returns false when turn is done (waiting for reaction) and true if still running.
        /// </summary>
        /// <returns></returns>
        public bool RunStep()
        {
            if (this.turnDone)
            {                
                return false;
            }

            this.Update(null);
            return true;
        }

        public virtual void UpdateDecisionMaking() { }

        protected override void ResetTurn() {}

        public void NewTurn() { this.ResetTurn(); }

        public virtual void NewDecision(UnitDecisionActivity activity, bool avoidShops = false) { }
    }
}
