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
    public abstract class GameEntity : GameObject
    {
        /// <summary>
        /// Unique identifier for this object
        /// </summary>
        private string id; 

        private Sprite sprite;

        [NonSerialized]
        private bool selected;

        [NonSerialized]
        private Tile currentTile;

        protected GameEntity(string objectName, ResourceType type)
            : base(objectName, type)
        {
            this.id = Guid.NewGuid().ToString();
            this.Sprite = new Sprite(objectName, type);
            this.Sprite.ReachedDestination += this.Sprite_ReachedDestination;
        }

        public Sprite Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public Rectangle DrawPosition 
        {
            get { return this.Sprite.DrawPosition; }
            set { this.Sprite.DrawPosition = value; }
        }

        /// <summary>
        /// Whether or not this is an entity the player can give direct commands to.
        /// </summary>
        public bool PlayerCanCommand { get; set; }

        /// <summary>
        /// Position relative to a grid system.
        /// </summary>
        public Point Coordinates { get; set; }

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
            this.Sprite.Update(time);
        }

        /// <summary>
        /// Changes the location of the entity to a new tile. Note: does NOT account for tile costs in any way.
        /// </summary>
        /// <param name="newTile">The new tile the entity will go to.</param>
        public virtual void SetLocationTo(Tile newTile, bool setTileResident = true)
        {
            if (this.CurrentTile != null)
            {
                this.CurrentTile.TileResident = null;
            }

            this.Coordinates = newTile.Coordinate;            
            this.CurrentTile = newTile;

            if (setTileResident)
            {
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

            this.Sprite.SetLocationTo(newTile.AreaRectangle);
        }

        /// <summary>
        /// Changes the location of the entity to a new tile.
        /// </summary>
        /// <param name="newTile">The new tile the entity will go to.</param>
        /// <param name="replaceOwners">If this is true, then this entity will swap the owners of the new and old tiles.</param>
        private void InitiateTransitionBetweenTiles(Tile oldTile, Tile newTile, bool replaceOwners = true)
        {
            this.Sprite.InitiateTransitionBetweenTiles(oldTile, newTile);

            this.CurrentTile = newTile;

            if (replaceOwners)
            {
                oldTile.TileResident = null;
                newTile.TileResident = this;
            }
        }

        /// <summary>
        /// Called when the target reaches its destination
        /// </summary>
        /// <param name="transitionTarget"></param>
        protected virtual void OnReachedTransitionDestination(Tile transitionTarget)
        {
        }

        private void Sprite_ReachedDestination(object sender, EventArgs e)
        {
            this.OnReachedTransitionDestination(this.CurrentTile);
        }

        /// <summary>
        /// Initiates the transition of an entity to a new tile.
        /// </summary>
        /// <param name="newTile">The tile to transition to.</param>
        /// <param name="replaceOwners">If this is true, then this entity will swap the owners of the new and old tiles.</param>
        public virtual void InitiateTransitionTo(Tile newTile, bool replaceOwners = true)
        {
            this.InitiateTransitionBetweenTiles(this.CurrentTile, newTile, replaceOwners);
        }

        public IconInfo GetEntityIcon()
        {
            return this.Sprite.GetEntityIcon();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this.Sprite.Draw(gameTime);
        }

        public bool IsTransitioning { get { return this.Sprite.IsTransitioning; } }
    }
}
