using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TacticsGame.Utility;
using TacticsGame.Abilities;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Managers;
using TacticsGame.Map;

namespace TacticsGame.GameObjects
{
    [Serializable]
    public class Sprite : GameObject, ISubSprite
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
        private bool transitioning = false;

        protected float baseTransitionSpeed = 2.0f;
        protected float transitionSpeed = 2.0f;

        public event EventHandler ReachedDestination;

        [NonSerialized]
        private Rectangle transitionTarget;

        [NonSerialized]
        private float transitionDirectionX;
        [NonSerialized]
        private float transitionDirectionY;
        [NonSerialized]
        private float XToYRatio;
        [NonSerialized]
        private float YToXRatio;

        [NonSerialized]
        private float distanceLeftToTravelX;
        [NonSerialized]
        private float distanceLeftToTravelY;

        [NonSerialized]
        private float realPosX;
        [NonSerialized]
        private float realPosY;

        [NonSerialized]
        private int animationCounter = 0;

        [NonSerialized]
        private DrawMode drawMode = 0;

        [NonSerialized]
        private Color? colorFilter = null;

        [NonSerialized]
        private bool facingLeft = true;        

        [NonSerialized]
        Rectangle currentTextureSource;

        [NonSerialized]
        protected TextureInfo textureInfo;
        
        [NonSerialized]
        private GameEntity owningEntity;

        public Sprite(string objectName)
            : this(objectName, ResourceType.GameObject)
        {            
        }

        public Sprite(string objectName, ResourceType type)
            : base(objectName, type)
        {
            // Having this here makes it easier to LoadContents later
            //this.DrawPosition = new Rectangle(0, 0, 0, 0);
        }       

        public GameEntity OwningEntity
        {
            get { return owningEntity; }
            set { owningEntity = value; }
        }

        public Color? ColorFilter
        {
            get { return colorFilter; }
            set { colorFilter = value; }
        }  

        /// <summary>
        /// Whether this entity is in the process of moving from one position to another.
        /// </summary>
        public bool IsTransitioning { get { return this.transitioning; } }

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

        public Rectangle CurrentTextureSource
        {
            get { return currentTextureSource; }
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

        public bool FacingLeft
        {
            get { return facingLeft; }
        }

        public TextureInfo TextureInfo
        {
            get { return textureInfo; }
            set { textureInfo = value; }
        }      

        private Rectangle GetInitialAnimationFrame()
        {
            return new Rectangle(this.textureInfo.StaticFrame, this.textureInfo.DefaultVertical, this.textureInfo.Width, this.textureInfo.Height);
        }


        public virtual void SetLocationTo(Rectangle destination)
        {
            this.DrawPosition = new Rectangle(destination.X, destination.Y, this.TextureInfo.Width, this.TextureInfo.Height);           
            this.transitioning = false;
        }

        /// <summary>
        /// Makes the unit go faster
        /// </summary>
        public void AccelerateTransitionSpeed()
        {
            this.transitionSpeed = 5.0f * this.baseTransitionSpeed;
        }

        /// <summary>
        /// Makes the unit move normally
        /// </summary>
        public void ResetTransitionSpeed()
        {
            this.transitionSpeed = this.baseTransitionSpeed;
        }

        public override void Draw(GameTime time)
        {
            Utilities.DrawTexture2D(this);

            this.SubSprite.Draw(time);
        }        

        /// <summary>
        /// Moves the animation forward by 1 frame.
        /// </summary>
        /// <param name="time"></param>
        protected virtual void Animate(GameTime time)
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

        /// <summary>
        /// Starts the movement of this entity between current location and target
        /// </summary>        
        public virtual void OnInitiateTransitionToTarget(Point target)
        {
            this.InitiateTransitionBetweenPoints(new Point(this.DrawPosition.X, this.DrawPosition.Y), target);
        }

        /// <summary>
        /// Changes the location of the entity to a new tile.
        /// </summary>
        /// <param name="newTile">The new tile the entity will go to.</param>
        /// <param name="replaceOwners">If this is true, then this entity will swap the owners of the new and old tiles.</param>
        public void InitiateTransitionBetweenTiles(Tile oldTile, Tile newTile, bool replaceOwners = true)
        {
            this.InitiateTransitionBetweenPoints(oldTile.AreaRectangle.Location, newTile.AreaRectangle.Location);
        }

        /// <summary>
        /// Starts the movement of this entity between two points.
        /// </summary>
        public virtual void InitiateTransitionBetweenPoints(Point source, Point target)
        {
            this.transitioning = true;
            this.transitionTarget = new Rectangle(target.X, target.Y, 0, 0);

            this.transitionDirectionX = source.X == target.X ? 0.0f : (source.X > target.X ? -1.0f : 1.0f);
            this.transitionDirectionY = source.Y == target.Y ? 0.0f : (source.Y > target.Y ? -1.0f : 1.0f);

            this.distanceLeftToTravelX = (float)Math.Abs(source.X - target.X);
            this.distanceLeftToTravelY = (float)Math.Abs(source.Y - target.Y);

            this.realPosX = source.X;
            this.realPosY = source.Y;

            if (this.DrawPosition.Y - target.Y != 0)
            {
                this.XToYRatio = this.distanceLeftToTravelX / this.distanceLeftToTravelY;
            }
            else
            {
                this.XToYRatio = 1.0f;
            }

            // HACK: to preserve speed
            this.YToXRatio = 1.0f;
            if (this.XToYRatio > 1.0f)
            {
                this.YToXRatio = 1.0f / this.XToYRatio;
                this.XToYRatio = 1.0f;
            }

            // Keep direction same if direction does not change
            if (this.transitionDirectionX < 0)
            {
                this.facingLeft = true;
            }
            if (this.transitionDirectionX > 0)
            {
                this.facingLeft = false;
            }    
        }

        public override void Update(GameTime time)
        {
            this.UpdateSprite(time);

            if (this.transitioning)
            {
                int gameSpeed = Utilities.GetSpeedMultiplier(GameManager.GameStateManager.GameSpeed);
                int width = this.DrawPosition.Width;
                int height = this.DrawPosition.Height;                                               

                if (this.distanceLeftToTravelX > 0)
                {
                    float distanceToCover = Math.Abs(this.realPosX - (float)this.transitionTarget.X);
                    float speed = Math.Min(this.XToYRatio * this.transitionSpeed, distanceToCover) * gameSpeed;
                    this.distanceLeftToTravelX -= speed;
                    this.realPosX = this.realPosX + speed * this.transitionDirectionX;
                }

                if (this.distanceLeftToTravelY > 0)
                {
                    float distanceToCover = Math.Abs(this.realPosY - (float)this.transitionTarget.Y);
                    float speed = Math.Min(this.transitionSpeed * YToXRatio, distanceToCover) * gameSpeed;
                    this.distanceLeftToTravelY -= speed;
                    this.realPosY = this.realPosY + speed * this.transitionDirectionY;
                }

                this.DrawPosition = new Rectangle((int)this.realPosX, (int)this.realPosY, width, height);

                // HACK: until i find a better way of doing this, looks like there is sometimes gonna be some extra digits
                if (this.distanceLeftToTravelY - 1 <= 0 && this.distanceLeftToTravelX - 1 <= 0)
                {
                    this.OnReachedTransitionDestination();

                    transitioning = false;
                }
            }
        }

        protected virtual void OnReachedTransitionDestination()
        {
            if (this.ReachedDestination != null)
            {
                this.ReachedDestination(this, EventArgs.Empty);
            }
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
