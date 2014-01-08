using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.AI.MaintenanceMode;

namespace TacticsGame.Items
{
    public interface IGenerateLoot
    {
        IEnumerable<string> GetActivityLoot(IMakeDecisions unit, Decision activity, int modifier = 0);

        IEnumerable<Item> GetCombatLoot(IDropLoot enemy);
    }
}
