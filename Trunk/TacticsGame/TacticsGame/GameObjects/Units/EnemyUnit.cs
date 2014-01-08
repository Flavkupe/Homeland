using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.GameObjects.Units
{
    public class EnemyUnit : Unit
    {
        public EnemyUnit(string unitType)
            : base(unitType)
        {
            this.PlayerCanCommand = false;
            this.IsHostile = true;

            // TEMP
            this.BaseStats.HP = 10;
            this.BaseStats.ActionPoints = 3;
            this.CurrentStats.ActionPoints = 3;

            this.CurrentStats = this.BaseStats.Clone();
        }
    }
}
