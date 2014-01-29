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
            Rectangle sourceRect = source.TileResident.Sprite.DrawPosition;
            this.DrawPosition = new Rectangle(sourceRect.Center.X, sourceRect.Center.Y, this.textureInfo.Width, this.textureInfo.Height);
            this.transitionSpeed = speed;
            this.OnInitiateTransitionToTarget(target.AreaRectangle.Center);                
        }

        protected override void OnReachedTransitionDestination()
        {
            this.reachedTarget = true;
            base.OnReachedTransitionDestination();
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
