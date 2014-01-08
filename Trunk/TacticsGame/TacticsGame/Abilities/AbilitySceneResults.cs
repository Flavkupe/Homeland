using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Text;
using TacticsGame.GameObjects.Effects;
using TacticsGame.GameObjects;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.Abilities
{
    public class AbilitySceneResults
    {
        private List<FloatingText> floatingText = new List<FloatingText>();
        public List<FloatingText> FloatingText
        {
            get { return floatingText; }
            set { floatingText = value; }
        }

        private List<ProjectileEffect> projectiles = new List<ProjectileEffect>();
        public List<ProjectileEffect> Projectiles
        {
            get { return projectiles; }
            set { projectiles = value; }
        }

        private List<AnimatedEffect> animations = new List<AnimatedEffect>();
        public List<AnimatedEffect> Animations
        {
            get { return animations; }
            set { animations = value; }
        }

        private List<Unit> killedUnits = new List<Unit>();
        public List<Unit> KilledUnits
        {
            get { return killedUnits; }
            set { killedUnits = value; }
        }

        public void Accumulate(AbilitySceneResults otherEffects)
        {
            if (otherEffects.Animations != null) { this.Animations.AddRange(otherEffects.Animations); }
            if (otherEffects.FloatingText != null) { this.FloatingText.AddRange(otherEffects.FloatingText); }
            if (otherEffects.Projectiles != null) { this.Projectiles.AddRange(otherEffects.Projectiles); }
            if (otherEffects.KilledUnits != null) { this.KilledUnits.AddRange(otherEffects.KilledUnits); }
        }
    }
}
