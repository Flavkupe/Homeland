using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.AI.MaintenanceMode;

namespace TestUtility.Simulation
{
    public class BasicSimulationScene : SimulationScene
    {
        protected override void AnnounceTextToUI(string text)
        {
            this.SendFeedbackMessage(text);
        }

        protected override void ResetTurn()
        {
            //this.decisionActivities.Clear();
            //this.shopOwnerDecisionActivities.Clear();
            //this.decisionMakingUnits.ForEach(a => this.decisionActivities.Add(new DecisionUnitActivity(a, Decision.Idle, 0)));
            //this.shopOwnerDecisionActivities.ForEach(a => this.NewDecision(a));
            //this.decisionActivities.ForEach(a => this.NewDecision(a));
            //this.buildings.ForEach(a => a.RefreshAtStartOfTurn());            
            //this.decisionMakingUnits.ForEach(a => a.RefreshStatsForNewManagementModeTurn());

            this.RefreshUnitAndOwnerStatusForNewTurn();
            this.turnDone = false;
            
        }
    }
}
