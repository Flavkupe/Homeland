using System;
namespace TacticsGame.AI.MaintenanceMode
{
    public interface IActionResultEngine
    {
        ActivityResult GetActionResult(UnitManagementActivity activity);
        ActivityResult GetOwnerActionResult(UnitManagementActivity activity);
        ActivityResult GetUnitActionResult(UnitManagementActivity activity);
    }
}
