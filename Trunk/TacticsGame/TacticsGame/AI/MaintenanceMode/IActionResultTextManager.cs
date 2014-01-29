using System;
namespace TacticsGame.AI.MaintenanceMode
{
    public interface IActionResultTextManager
    {
        string GetDecisionStringForActionComplete(TacticsGame.AI.MaintenanceMode.UnitManagementActivity activity, TacticsGame.AI.MaintenanceMode.ActivityResult results);
        string GetDecisionStringForActionComplete(TacticsGame.GameObjects.Units.Unit unit, TacticsGame.AI.MaintenanceMode.Decision decision, TacticsGame.AI.MaintenanceMode.ActivityResult results);
        string GetDecisionStringForActionStarted(TacticsGame.AI.MaintenanceMode.UnitManagementActivity activity);
        string GetDecisionStringForUnableToFindProperBuilding(TacticsGame.AI.MaintenanceMode.UnitManagementActivity activity);
        string GetDecisionStringForUnitDone(TacticsGame.GameObjects.Units.Unit unit);
        string GetStringForUnitTaxed(TacticsGame.AI.MaintenanceMode.UnitManagementActivity activity, int moneyLost);
        string GetStringForUnitTaxed(TacticsGame.GameObjects.Units.Unit unit, int moneyLost);
    }
}
