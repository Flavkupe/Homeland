using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;

namespace TacticsGame.Abilities
{
    public class AbilityStats
    {
        private AbilityType type = AbilityType.Self;
        private AbilityProperty properties = AbilityProperty.None;
        List<AbilityEffect> abilityEffects = new List<AbilityEffect>();

        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public int Cooldown { get; set; }
        public int APCost { get; set; }
       
        public AbilityType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Gets the type of valid targets.
        /// </summary>
        public AbilityTargetType TargetType
        {
            get 
            {
                switch (this.Type)
                {                    
                    case AbilityType.Self:
                    case AbilityType.SelfRadialAll:                        
                    case AbilityType.SelfRadialEnemy:
                    case AbilityType.SelfRadialFriendly:
                        return AbilityTargetType.Self;
                    case AbilityType.TargetEnemy:                    
                        return AbilityTargetType.Enemy;                        
                    case AbilityType.TargetFriendly:                                                   
                        return AbilityTargetType.Ally;
                    case AbilityType.TargetRadialAll:
                    case AbilityType.TargetRadialFriendly:
                    case AbilityType.TargetRadialEnemy:
                        return AbilityTargetType.Any;                        
                    default:
                        return AbilityTargetType.Any;
                } 
            }
        }

        public AbilityProperty Properties
        {
            get { return this.properties; }
            set { this.properties = value; }
        }

        public List<AbilityEffect> AbilityEffects
        {
            get { return this.abilityEffects; }
            set { this.abilityEffects = value; }
        }

        public AbilityStats(int apCost = 1, int minRange = 1, int maxRange = 1, int cooldown = 0)
        {
            this.APCost = apCost;
            this.MinRange = minRange;
            this.MaxRange = maxRange;
            this.Cooldown = cooldown;
        }

        /// <summary>
        /// Whether this ability has the specific property, such as letting a unit attack after.
        /// </summary>
        public bool HasProperty(AbilityProperty property)
        {
            return this.Properties.HasFlag(property);
        }

        /// <summary>
        /// Sets the property to true.
        /// </summary>
        public void SetProperty(AbilityProperty property)
        {
            this.Properties |= property;
        }

        public AbilityStats Clone()
        {
            AbilityStats clone = (AbilityStats)this.MemberwiseClone();
            clone.AbilityEffects = new List<AbilityEffect>();
            this.AbilityEffects.ForEach(a => clone.AbilityEffects.Add(a.Clone()));

            return clone;
        }        
    }

    /// <summary>
    /// A special flagged enum of ability properties, such as whether or not a unit can attack after using 
    /// the ability.
    /// </summary>
    [Flags]    
    public enum AbilityProperty
    {
        None = 0,
        CanActAfter = 1,
        CanMoveAfter = 2,
        CanAttackAfter = 4,
        CanUseAbilityAfter = 8,
    }
}

