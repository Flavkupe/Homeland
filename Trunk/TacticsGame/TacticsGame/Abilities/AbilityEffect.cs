using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;

namespace TacticsGame.Abilities
{
    /// <summary>
    /// Used for holding info about an ability's effect. An ability can have multiple of these.
    /// </summary>
    public class AbilityEffect
    {
        private AbilityDirectEffect directEffects = AbilityDirectEffect.None;

        /// <summary>
        /// Info about an ability's effects.
        /// </summary>
        /// <param name="targetType">The target that is affected by this particular effect.</param>
        public AbilityEffect(AbilityTargetType targetType)
        {
             this.TargetType = targetType;             
        }
    
        /// <summary>
        /// Any status effects this ability might have. Can be null.
        /// </summary>
        public UnitStatusEffectInfo StatusEffect { get; set; }

        public int HPModification { get; set; }
        public int APModification { get; set; }
        public int MoraleModification { get; set; }

        /// <summary>
        /// An effect that deals directly with cooldowns.
        /// </summary>
        public CooldownModificationEffect CooldownEffect { get; set; }        

        /// <summary>
        /// Any direct effects this activity may also have. Can be None.
        /// </summary>
        public AbilityDirectEffect DirectEffects
        {
            get { return this.directEffects; }
            set { this.directEffects = value; }
        }

        /// <summary>
        /// The type of target this particular effect affects.
        /// </summary>
        public AbilityTargetType TargetType { get; set; }

        /// <summary>
        /// Clones this class
        /// </summary>
        /// <returns></returns>
        public AbilityEffect Clone()
        {
            AbilityEffect clone = (AbilityEffect)this.MemberwiseClone();
            clone.StatusEffect = this.StatusEffect == null ? null : this.StatusEffect.Clone();
            clone.CooldownEffect = this.CooldownEffect == null ? null : this.CooldownEffect.Clone();
            return clone;
        }
    }

    [Flags]
    public enum AbilityDirectEffect
    {
        None = 0,
        SkipRestOfTurn = 1, // Effect of "Muddle"
    }

    /// <summary>
    /// A type of effect that does work on ability cooldowns.
    /// </summary>
    public class CooldownModificationEffect
    {
        /// <summary>
        /// If Modification is null, the ability cooldown will go down to 0, enabling the ability. Else it will lower/raise by the Modification value. 1 will raise cooldown by 1.
        /// </summary>
        public int? Modification { get; set; }

        public TargetAbilityCooldown Target { get; set; }

        public CooldownEffectCondition Condition { get; set; }

        /// <summary>
        /// Usually null. Only relevant if Target's type is "Specific"... then it'd be the specific ability being affected.
        /// </summary>
        public string SpecificAbilityName { get; set; }

        public CooldownModificationEffect Clone()
        {
            return (CooldownModificationEffect)this.MemberwiseClone();
        }

        public enum TargetAbilityCooldown
        {
            All,
            RandomDisabled,
            RandomEnabled,
            AllSpells,
            Specific,
            Select,
        }

        public enum CooldownEffectCondition
        {
            None,
            OnKill,
        }
    }
}
