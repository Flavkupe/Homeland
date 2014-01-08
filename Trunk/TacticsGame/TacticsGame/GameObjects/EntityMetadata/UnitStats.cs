using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;

namespace TacticsGame.EntityMetadata
{
    /// <summary>
    /// Class to keep track of shit like hit points, stength, all that stuff.
    /// </summary>
    [Serializable]
    public class UnitStats
    {
        private int hp;
        private int baseAttack;
        private int baseAttackAP;
        private int physical;
        private int mental;
        private int cunning;
        private int actionPoints;
        private int morale;
        private int loyalty;
        private int baseAttackRange;
        private int baseAttackMinRange;

        private WeaponType weaponTypePreference;

        public List<UnitTrait> Traits = new List<UnitTrait>();

        public UnitStats()
        {
            this.Morale = 100;
            this.Loyalty = 80;
            this.ActionPoints = 6;
            this.HP = 50;
            this.BaseAttack = 1;
            this.BaseAttackAP = 3;
            this.BaseAttackRange = 1;
            this.BaseAttackMinRange = 1;

            // TEMP
            int numTraits = Enum.GetValues(typeof(UnitTrait)).Length;
            for (int i = 0; i < 4; ++i)
            {
                int num = Utilities.GetRandomNumber(0, numTraits - 1);
                if (!this.Traits.Contains((UnitTrait)num))
                {
                    this.Traits.Add((UnitTrait)num);
                }
                else
                {
                    i--;
                    continue;
                }
            }

            this.Skills = new UnitSkills();
        }

        public UnitSkills Skills { get; private set; }                

        public int ActionPoints
        {
            get { return actionPoints; }
            set { actionPoints = value; }
        }

        public int HP
        {
            get { return hp; }
            set { hp = value; }
        }
        
        public int BaseAttack
        {
            get { return baseAttack; }
            set { baseAttack = value; }
        }
        
        public int BaseAttackAP
        {
            get { return baseAttackAP; }
            set { baseAttackAP = value; }
        }        

        public int Physical
        {
            get { return physical; }
            set { physical = value; }
        }
        
        public int Mental
        {
            get { return mental; }
            set { mental = value; }
        }        

        public int Cunning
        {
            get { return cunning; }
            set { cunning = value; }
        }

        public int Morale
        {
            get { return morale; }
            set { morale = value; }
        }        

        public int Loyalty
        {
            get { return loyalty; }
            set { loyalty = value; }
        }        

        public WeaponType WeaponTypePreference
        {
            get { return weaponTypePreference; }
            set { weaponTypePreference = value; }
        }

        /// <summary>
        /// Attack range of the basic attack
        /// </summary>        
        public int BaseAttackRange
        {
            get { return baseAttackRange; }
            set { baseAttackRange = value; }
        }

        /// <summary>
        /// Min attack range, for ranged attacks.
        /// </summary>        
        public int BaseAttackMinRange
        {
            get { return baseAttackMinRange; }
            set { baseAttackMinRange = value; }
        }

        /// <summary>
        /// Create a clone of this instance and return it.
        /// </summary>
        /// <returns>A clone of this instance.</returns>
        public UnitStats Clone()
        {
            return (UnitStats)this.MemberwiseClone();
        }

        public void GenerateRandomStats(int min, int max)
        {
            this.Physical = Utilities.GetRandomNumber(min, max);
            this.Mental = Utilities.GetRandomNumber(min, max);
            this.Cunning = Utilities.GetRandomNumber(min, max);
        }

        /// <summary>
        /// Makes a luck roll based on the unit stats. It's a number from 0 to 100
        /// boosted by certain stats.
        /// </summary>
        /// <returns></returns>
        public int LuckRoll()
        {
            int roll = Utilities.GetRandomNumber(0, 100);
            return roll.GetClampedValue(0, 100);
        }

        /// <summary>
        /// Makes a skill roll based on the unit stats, based on a specific skill.
        /// </summary>
        /// <returns></returns>
        public int SkillRoll(UnitSkills.SkillType skill)
        {
            int roll = this.Skills.GetSkillLevel(skill) * 5;
            return roll;
        }
    }    
}

