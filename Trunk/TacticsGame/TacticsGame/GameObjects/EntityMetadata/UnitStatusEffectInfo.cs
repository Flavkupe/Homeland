using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.GameObjects.EntityMetadata
{
    /// <summary>
    /// Contains info about a UnitStatusEffect
    /// </summary>
    [Serializable]
    public class UnitStatusEffectInfo
    {
        private UnitStatusEffect effect;
        private int modifier = 0;
        private int duration = 1;

        /// <summary>
        /// The effect
        /// </summary>
        public UnitStatusEffect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        /// <summary>
        /// Duration (cooldown) on effect
        /// </summary>
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        /// <summary>
        /// Modifier for whichever things are wanted, like damage.
        /// </summary>
        public int Modifier
        {
            get { return modifier; }
            set { modifier = value; }
        }

        public UnitStatusEffectInfo(UnitStatusEffect effect, int duration = 1, int modifier = 0)
        {
            this.effect = effect;
            this.Duration = duration;
            this.Modifier = modifier;
        }

        public UnitStatusEffectInfo Clone()
        {
            return (UnitStatusEffectInfo)this.MemberwiseClone();
        }
    }
}
