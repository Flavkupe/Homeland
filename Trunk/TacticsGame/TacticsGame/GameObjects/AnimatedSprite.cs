using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TacticsGame.Utility;
using TacticsGame.Abilities;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Managers;

namespace TacticsGame.GameObjects
{
    [Serializable]
    public class AnimatedSprite : GameObject, ISubSprite
    {
        private ISubSprite SubSprite = new SubSpriteCollection();

        private Rectangle drawPosition;

        [NonSerialized]
        private bool animated;

        [NonSerialized]
        int currentFrame = 1;
        [NonSerialized]
        bool reverse = false;

        [NonSerialized]
        private int animationCounter = 0;

        [NonSerialized]
        private DrawMode drawMode = 0;

        [NonSerialized]
        protected Color? colorFilter = null;  

        [NonSerialized]
        Rectangle currentTextureSource;

        [NonSerialized]
        protected TextureInfo textureInfo;       

        public AnimatedSprite(string objectName)
            : this(objectName, ResourceType.GameObject)
        {            
        }

        public AnimatedSprite(string objectName, ResourceType type)
            : base(objectName, type)
        {
            // Having this here makes it easier to LoadContents later
            //this.DrawPosition = new Rectangle(0, 0, 0, 0);
        }               

        /// <summary>
        /// Actual position on screen, in pixels.
        /// </summary>
        public Rectangle DrawPosition
        {
            get { return drawPosition; }
            set 
            {
                this.drawPosition = value;
                this.SetDrawPosition(value.X, value.Y);
            }
        }

        /// <summary>
        /// Scale of this object for drawing
        /// </summary>
        public float? Scale { get; set; }        

        public DrawMode CurrentDrawMode
        {
            get { return drawMode; }
            set { drawMode = value; }
        }

        /// <summary>
        /// Whether it's animated.
        /// </summary>
        public bool Animated
        {
            get { return this.animated; }
            set
            {
                if (this.animated != value)
                {
                    this.currentTextureSource = this.GetInitialAnimationFrame();
                }

                this.animated = value;
            }
        }

        private Rectangle GetInitialAnimationFrame()
        {
            return new Rectangle(this.textureInfo.StaticFrame, this.textureInfo.DefaultVertical, this.textureInfo.Width, this.textureInfo.Height);
        }

        public override void Draw(GameTime time)
        {
            if (this.Scale.HasValue)
            {
                Utilities.DrawTexture2D(textureInfo.Texture, this.DrawPosition, this.currentTextureSource, colorFilter, 0.0f, this.Scale.Value);
            }
            else
            {
                Utilities.DrawTexture2D(textureInfo.Texture, this.DrawPosition, this.currentTextureSource, colorFilter);
            }

            this.SubSprite.Draw(time);
        }        

        /// <summary>
        /// Moves the animation forward by 1 frame.
        /// </summary>
        /// <param name="time"></param>
        public virtual void Animate(GameTime time)
        {
            if (this.textureInfo.IsAnimated && this.textureInfo.HorizontalFrames > 1) 
            {
                if (this.reverse)
                {
                    this.currentFrame--;
                }
                else
                {
                    this.currentFrame++;
                }
                
                if (this.currentFrame > this.textureInfo.HorizontalFrames)
                {
                    this.ReachedAnimationEnd();
                    this.reverse = true;
                    this.currentFrame -= 2;
                }
                else if (this.currentFrame < 1)
                {
                    this.ReachedAnimationStart();
                    this.reverse = false;
                    this.currentFrame += 2;
                }

                this.currentTextureSource = new Rectangle((this.currentFrame - 1) * this.textureInfo.Width, 0, this.textureInfo.Width, this.textureInfo.Height);
            }                       
        }

        /// <summary>
        /// Gets icon represented by this.
        /// </summary>
        /// <returns></returns>
        public virtual IconInfo GetEntityIcon()
        {
            return this.textureInfo.Icon;
        }

        public override void LoadContent()
        {
            GameResourceInfo info = GameResourceManager.Instance.GetResourceByResourceType(this.ObjectName, this.ResourceType);
            this.textureInfo = info.TextureInfo;
            this.drawPosition = new Rectangle(this.drawPosition.X, this.drawPosition.Y, this.textureInfo.Width, this.textureInfo.Height);
            this.currentTextureSource = this.GetInitialAnimationFrame();
            this.animated = this.textureInfo.IsAnimated;

            if (this.DisplayName == null || this.DisplayName == this.ObjectName)
            {
                this.DisplayName = string.IsNullOrWhiteSpace(info.DisplayName) ? this.ObjectName : info.DisplayName;
            }
        }

        public override void Update(GameTime time)
        {
            this.UpdateSprite(time);
        }

        public void UpdateAnimation(GameTime time)
        {
            if (this.Animated)
            {
                this.animationCounter += time.ElapsedGameTime.Milliseconds;

                if (this.animationCounter > this.textureInfo.AnimationRate)
                {
                    this.animationCounter = 0;
                    this.Animate(time);
                }
            }

            this.UpdateSprite(time);
        }

        /// <summary>
        /// If it reaches past the last frame
        /// </summary>
        protected virtual void ReachedAnimationEnd() 
        {
        }

        /// <summary>
        /// If, going in reverse, it loops past the start again
        /// </summary>
        protected virtual void ReachedAnimationStart()
        {
        }

        public void AddSubSprite(ISubSprite subsprite)
        {
            this.SubSprite.AddSubSprite(subsprite);
        }

        public void UpdateSprite(GameTime time)
        {                        
            this.SubSprite.UpdateSprite(time);
        }

        public void SetDrawPosition(int x, int y)
        {
            int diffX = x - this.drawPosition.X;
            int diffY = y - this.drawPosition.Y;
            this.drawPosition = this.DrawPosition.CloneAndRelocate(x, y);
            this.SubSprite.ShiftDrawPosition(diffX, diffY);
        }

        public void ShiftDrawPosition(int x, int y)
        {
            this.drawPosition = this.DrawPosition.CloneAndOffset(x, y);
            this.SubSprite.ShiftDrawPosition(x, y);
        }
    }
}
