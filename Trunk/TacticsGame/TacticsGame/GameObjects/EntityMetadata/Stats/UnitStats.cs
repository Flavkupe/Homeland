using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.EntityMetadata.Stats
{
    /// <summary>
    /// Class to keep track of shit like hit points, stength, all that stuff.
    /// </summary>
    public class UnitStats
    {
        public int ActionPoints { get; set; }

        public int HP { get; set; }

        public int BaseAttack { get; set; }

        public int BaseAttackAP { get; set; }

        /// <summary>
        /// Attack range of the basic attack
        /// </summary>
        public int BaseAttackRange { get; set; }

        /// <summary>
        /// Min attack range, for ranged attacks.
        /// </summary>
        public int BaseAttackMinRange { get; set; }

        /// <summary>
        /// Create a clone of this instance and return it.
        /// </summary>
        /// <returns>A clone of this instance.</returns>
        public UnitStats Clone()
        {
            UnitStats clone = new UnitStats();
            clone.ActionPoints = this.ActionPoints;            
            clone.HP = this.HP;
            clone.BaseAttack = this.BaseAttack;
            clone.BaseAttackAP = this.BaseAttackAP;
            clone.BaseAttackRange = this.BaseAttackRange;
            clone.BaseAttackMinRange = this.BaseAttackMinRange;
            return clone;
        }
    }    
}

