using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.Utility;

namespace TacticsGame.GameObjects.Units.Enemy.Types
{
    public class Bandit : EnemyUnit, IDropLoot
    {
        public Bandit()
            : base("Bandit")
        {            
        }

        protected override void InitializeStats()
        {
            this.BaseStats.GenerateRandomStats(10, 15);
            this.BaseStats.ActionPoints = 5;
            this.BaseStats.BaseAttack = 4;
            this.BaseStats.BaseAttackAP = 3;
            this.BaseStats.HP = 15;
        }        
    }
}
