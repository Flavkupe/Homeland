using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TacticsGame.Map;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Utility;
using TacticsGame.Managers;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Buildings;

namespace TacticsGame.GameObjects
{
    /// <summary>
    /// An object in the game that has a location somewhere and is interacted with.
    /// This object is a unique object that has a unique ID, whose reference is relevant in the future.
    /// </summary>
    [Serializable]
    public abstract class GameEntity : AnimatedSprite
    {
        [NonSerialized]
        private bool transitioning = false;
        
        protected float baseTransitionSpeed = 2.0f;        
        protected float transitionSpeed = 2.0f;

        /// <summary>
        /// Unique identifier for this object
        /// </summary>
        private string id; 

        /// <summary>
        /// Whether this entity is in the process of moving from one position to another.
        /// </summary>
        public bool IsTransitioning { get { return this.transitioning; } }

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
        private Rectangle transitionTarget;

        [NonSerialized]
        private bool selected;
        [NonSerialized]
        private Tile currentTile;        

        protected GameEntity(string objectName, ResourceType type)
            : base(objectName, type)
        {
            this.id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Whether or not this is an entity the player can give direct commands to.
        /// </summary>
        public bool PlayerCanCommand { get; set; }

        /// <summary>
        /// Current tile inhabited by entity
        /// </summary>       
        public virtual Tile CurrentTile
        {
            get { return currentTile; }
            set { currentTile = value; }
        }

        /// <summary>
        /// A unique identifier for this GameEntity that can be used to tell this entity appart from others. 
        /// </summary>
        public string ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// Whether or not entity is selected
        /// </summary>
        public virtual bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }        

        /// <summary>
        /// Position relative to a grid system.
        /// </summary>
        public Point Coordinates { get; set; }

        /// <summary>
        /// Changes the location of the entity to a new tile. Note: does NOT account for tile costs in any way.
        /// </summary>
        /// <param name="newTile">The new tile the entity will go to.</param>
        public virtual void SetLocationTo(Tile newTile)
        {
            if (this.CurrentTile != null)
            {
                this.CurrentTile.TileResident = null;
            }

            this.Coordinates = newTile.Coordinate;
            this.DrawPosition = new Rectangle(newTile.AreaRectangle.X, newTile.AreaRectangle.Y, this.textureInfo.Width, this.textureInfo.Height);
            this.CurrentTile = newTile;
            this.transitioning = false;

            if (this is IMultiTile)
            {
                foreach (Tile tile in ((IMultiTile)this).CurrentTiles)
                {
                    tile.TileResident = this;
                }
            }
            else 
            {
                newTile.TileResident = this;
            }
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

        /// <summary>
        /// Changes the location of the entity to a new tile.
        /// </summary>
        /// <param name="newTile">The new tile the entity will go to.</param>
        /// <param name="replaceOwners">If this is true, then this entity will swap the owners of the new and old tiles.</param>
        private void InitiateTransitionBetweenTiles(Tile oldTile, Tile newTile, bool replaceOwners = true)
        {
            this.InitiateTransitionBetweenPoints(oldTile.AreaRectangle.Location, newTile.AreaRectangle.Location);
                        
            this.CurrentTile = newTile;

            if (replaceOwners)
            {
                oldTile.TileResident = null;
                newTile.TileResident = this;
            }
        }

        /// <summary>
        /// Initiates the transition of an entity to a new tile.
        /// </summary>
        /// <param name="newTile">The tile to transition to.</param>
        /// <param name="replaceOwners">If this is true, then this entity will swap the owners of the new and old tiles.</param>
        public virtual void InitiateTransitionTo(Tile newTile, bool replaceOwners = true)
        {            
            this.InitiateTransitionBetweenTiles(this.CurrentTile, newTile,replaceOwners);
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

            this.distanceLeftToTravelX = (float) Math.Abs(source.X - target.X);
            this.distanceLeftToTravelY = (float) Math.Abs(source.Y - target.Y);

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
        }

        /// <summary>
        /// Starts the movement of this entity between current location and target
        /// </summary>        
        public virtual void InitiateTransitionToTarget(Point target)
        {
            this.InitiateTransitionBetweenPoints(new Point(this.DrawPosition.X, this.DrawPosition.Y), target);
        }

        /// <summary>
        /// Called when the target reaches its destination
        /// </summary>
        /// <param name="transitionTarget"></param>
        protected virtual void ReachedTransitionDestination(Tile transitionTarget)
        {
        }

        /// <summary>
        /// Loads all the graphic-related shaz
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();                            
        }

        /// <summary>
        /// Based on existing units and buildings, this will load the proper references to them, when called. Meant to be overriden and called after serialization.
        /// </summary>
        /// <param name="units">List of units that this entity may have a refernece to (stored by id)</param>
        /// <param name="buildings">List of buildings that this entity may have a refernece to (stored by id)</param></param>
        public virtual void LoadReferencesFromLists(List<DecisionMakingUnit> units, List<Building> buildings)
        {
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            if (transitioning)
            {                
                int width = this.DrawPosition.Width;
                int height = this.DrawPosition.Height;

                if (this.distanceLeftToTravelX > 0)
                {
                    float distanceToCover = Math.Abs(this.realPosX - (float) this.transitionTarget.X);
                    float speed = Math.Min(this.XToYRatio * this.transitionSpeed, distanceToCover);
                    this.distanceLeftToTravelX -= speed;
                    this.realPosX = this.realPosX + speed * transitionDirectionX;                     
                }
                
                if (this.distanceLeftToTravelY > 0)
                {
                    float distanceToCover = Math.Abs(this.realPosY - (float) this.transitionTarget.Y);
                    float speed = Math.Min(this.transitionSpeed * YToXRatio, distanceToCover);
                    this.distanceLeftToTravelY -= speed;
                    this.realPosY = this.realPosY + speed * transitionDirectionY;
                }

                this.DrawPosition = new Rectangle((int)this.realPosX, (int)this.realPosY, width, height);

                // HACK: until i find a better way of doing this, looks like there is sometimes gonna be some extra digits
                if (this.distanceLeftToTravelY - 1 <= 0 && this.distanceLeftToTravelX - 1 <= 0)
                {
                    this.ReachedTransitionDestination(this.CurrentTile);
                    transitioning = false;
                }
            }
        }
    }
}
