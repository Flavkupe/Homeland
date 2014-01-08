using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Map;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.AI
{
    public class SimpleEnemyDecisionEngine : CombatAIDecisionEngine
    {
        /// <summary>
        /// For this simple version, go towards the closest unit and attack.
        /// </summary>
        /// <param name="currentUnit"></param>
        /// <param name="grid"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public override Tile DecideMovementTarget(Unit currentUnit, TileGrid grid, List<Unit> units)
        {            
            List<Tile> currentPath = null;

            foreach (Unit unit in units)
            {
                if (!unit.IsHostile)
                {
                    List<Tile> testPath = grid.GetPathBetween(currentUnit.CurrentTile, unit.CurrentTile, false, false);
                    if (currentPath == null || (testPath != null && testPath.Count < currentPath.Count))
                    {
                        currentPath = testPath;
                    }
                }
            }

            Tile targetTile = currentPath == null || currentPath.Count <= 1 ? null : currentPath[currentPath.Count - 2];
            return targetTile;
        }

        /// <summary>
        /// For this simple version, attack first unit it's close to.
        /// </summary>
        /// <param name="currentUnit"></param>
        /// <param name="grid"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public override Tile DecideAttackTarget(Unit currentUnit, TileGrid grid, List<Unit> units)
        {
            if (currentUnit.CurrentStats.ActionPoints < currentUnit.CurrentStats.BaseAttackAP)
            {
                return null;
            }

            HashSet<Tile> attackRange = grid.GetTileRadius(currentUnit.CurrentTile, currentUnit.GetAttackMaxRange(), true, currentUnit.GetAttackMinRange());
            foreach (Tile tile in attackRange)
            {
                if (tile.TileResident is Unit && !((Unit)tile.TileResident).IsHostile)
                {
                    return tile;
                }
            }

            return null;
        }

        /// <summary>
        /// In this simple version, returns whether or not the unit is able to afford a basic attack.
        /// </summary>
        /// <param name="currentUnit"></param>
        /// <returns></returns>
        public override bool CanPerformAnAttack(Unit currentUnit)
        {
            return currentUnit.CurrentStats.ActionPoints >= currentUnit.CurrentStats.BaseAttackAP;
        }
    }
}
