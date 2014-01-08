using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Abilities;
using TacticsGame.GameObjects.Effects;
using TacticsGame.Map;
using Microsoft.Xna.Framework;

namespace TacticsGame.GameObjects
{
    public class AnimatedEffect : GameEntity, IExpire
    {
        private int? loops;

        private int currentLoop = 1;

        public AnimatedEffect(string effectName, Tile target,  ResourceType type = TacticsGame.ResourceType.VisualEffect, int? loops = null) 
            : base(effectName, type)
        {
            this.loops = loops;
            Rectangle targetRect = target.TileResident.DrawPosition;
            this.DrawPosition = target.AreaRectangle.Clone();
        }

        protected override void ReachedAnimationEnd()
        {
            this.currentLoop += 1;
            base.ReachedAnimationEnd();
        }

        protected override void ReachedAnimationStart()
        {
            this.currentLoop += 1;
            base.ReachedAnimationStart();
        }

        public virtual bool IsExpired { get { return this.loops.HasValue && this.currentLoop > this.loops; } }
    }
}
