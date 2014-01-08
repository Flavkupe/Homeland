using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Abilities
{
    public class AbilityVisualEffectInfo
    {
        private int cylces = 1;

        private float scale = 1.0f;

        private AbilityTargetType targetType = AbilityTargetType.Self;

        private VisualEffectType visualEffectType = VisualEffectType.Animation;

        private string effectName = null;

        public string EffectName
        {
            get { return effectName; }
            set { effectName = value; }
        }

        public VisualEffectType VisualEffectType
        {
            get { return visualEffectType; }
            set { visualEffectType = value; }
        }

        public AbilityTargetType TargetType
        {
            get { return targetType; }
            set { targetType = value; }
        }
        
        public int Cylces
        {
            get { return cylces; }
            set { cylces = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
    }

    public enum VisualEffectType
    {
        LinearProjectile,
        Animation,
    }
}
