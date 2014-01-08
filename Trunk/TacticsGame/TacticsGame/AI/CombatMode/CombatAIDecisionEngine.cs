using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.Map;
using TacticsGame.GameObjects.Obstacles;

namespace TacticsGame.AI
{
    public abstract class CombatAIDecisionEngine
    {
        public abstract Tile DecideMovementTarget(Unit currentUnit, TileGrid grid, List<Unit> units);

        public abstract Tile DecideAttackTarget(Unit currentUnit, TileGrid grid, List<Unit> units);

        public abstract bool CanPerformAnAttack(Unit currentUnit);
    }
}
