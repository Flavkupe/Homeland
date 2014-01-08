using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Abilities;
using TacticsGame.Map;
using Microsoft.Xna.Framework;

namespace TacticsGame.GameObjects.Effects
{
    public class ProjectileEffect : AnimatedEffect
    {
        public bool reachedTarget = false;

        public ProjectileEffect(string abilityName, Tile source, Tile target, int speed)
            : base(abilityName, target, ResourceType.VisualEffect)
        {
            this.CurrentTile = source;
            Rectangle sourceRect = source.TileResident.DrawPosition;
            this.DrawPosition = new Rectangle(sourceRect.Center.X, sourceRect.Center.Y, this.textureInfo.Width, this.textureInfo.Height);
            this.transitionSpeed = speed;
            this.InitiateTransitionToTarget(target.AreaRectangle.Center);                
        }

        protected override void ReachedTransitionDestination(Tile transitionTarget)
        {
            this.reachedTarget = true;
        }

        public override bool IsExpired
        {
            get
            {
                return this.reachedTarget;
            }
        }
    }
}
