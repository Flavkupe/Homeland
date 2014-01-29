using System;
using System.Collections.Generic;
using TacticsGame.GameObjects.Zones;
namespace TacticsGame.AI.MaintenanceMode
{
    public interface IDecisionEngine
    {
        int DetermineDecisionDuration(TacticsGame.GameObjects.Units.IMakeDecisions unit, Decision decision);
        TacticsGame.GameObjects.Buildings.Building DetermineTargetBuilding(TacticsGame.GameObjects.Units.IMakeDecisions unit, Decision decision, System.Collections.Generic.List<TacticsGame.GameObjects.Buildings.Building> list);
        TacticsGame.GameObjects.Units.DecisionMakingUnit DetermineTargetVisitor(TacticsGame.GameObjects.Units.IMakeDecisions unit, Decision decision, System.Collections.Generic.List<TacticsGame.GameObjects.Units.DecisionMakingUnit> list);
        Decision MakeDecision(TacticsGame.GameObjects.Units.IMakeDecisions unit);
        Decision MakeOwnerDecision(TacticsGame.GameObjects.Owners.Owner owner);

        bool DecisionRequiresBuilding(Decision decision);

        bool DecisionRequiresVisitor(Decision decision);

        bool DecisionRequiresExitZone(Decision decision);

        ExitZone GetTargetZone(Decision decision, IEnumerable<ExitZone> zones);
    }
}
