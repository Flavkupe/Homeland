using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.GameObjects;

namespace TacticsGame.Map
{
    /// <summary>
    /// The base tile class that you can draw and stuff.
    /// </summary>
    [Serializable]
    public abstract class Tile : GameObject
    {
        [NonSerialized]
        private GameEntity tileResident;

        [NonSerialized]
        private List<GameEntity> passableContents;

        [NonSerialized]
        protected Texture2D texture;

        [NonSerialized]
        private TileGrid grid = null;

        protected Tile(string tileType, TileGrid grid)
            : base(tileType, ResourceType.Tile)
        {
            this.Grid = grid;
        }

        /// <summary>
        /// Reference to the grid this belongs to.
        /// </summary>        
        public TileGrid Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        public TileDrawFilter? Filter {get; set; }        

        public Point Coordinate { get; protected set; }

        public Rectangle AreaRectangle { get; protected set; } 

        public int Dimensions { get; protected set; }

        public abstract string TileDisplayName { get; }        

        /// <summary>
        /// Cost of traversing this tile.
        /// </summary>
        public virtual int Cost { get { return 1; } }

        public bool IsTraversable
        {
            get { return this.TileResident == null; }
        }        

        public Texture2D Texture
        {
            get { return this.texture; }
        }

        public GameEntity TileResident
        {
            get { return tileResident; }
            set { tileResident = value; }
        }

        public List<GameEntity> PassableContents
        {
            get { return passableContents; }
            private set { passableContents = value; }
        }
        
        ////////////////
        #region GetRelativeTiles

        public Tile GetNorth()
        {
            return this.Grid.GetTile(this.Coordinate.X, this.Coordinate.Y - 1);
        }

        public Tile GetEast()
        {
            return this.Grid.GetTile(this.Coordinate.X + 1, this.Coordinate.Y);
        }

        public Tile GetWest()
        {
            return this.Grid.GetTile(this.Coordinate.X - 1, this.Coordinate.Y);
        }

        public Tile GetSouth()
        {
            return this.Grid.GetTile(this.Coordinate.X, this.Coordinate.Y + 1);
        }

        #endregion
        ////////////////

        /// <summary>
        /// Adds entity to the list of passable objects, creating the list if necessary.
        /// </summary>
        /// <param name="entity"></param>
        public void AddPassableObject(GameEntity entity)
        {
            if (this.PassableContents == null)
            {
                this.PassableContents = new List<GameEntity>();
            }

            this.PassableContents.Add(entity);
            entity.Sprite.DrawPosition = this.AreaRectangle;
            entity.CurrentTile = this;
        }

        /// <summary>
        /// Removes entity from the passable contents on the tile, nulling the list if necessary.
        /// </summary>
        /// <param name="entity"></param>
        public void RemovePassableContent(GameEntity entity)
        {
            if (this.PassableContents != null && this.PassableContents.Contains(entity))
            {
                this.PassableContents.Remove(entity);

                if (this.PassableContents.Count == 0)
                {
                    this.PassableContents = null;
                }
            }
        }

        public override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {            
        }

        public abstract void Draw(GameTime gameTime, TileDrawFilter filter);        

        public enum TileDrawFilter
        {
            None,
            TileMovement,
            AttackRange,
            MovementAndAttackRange,

            CanPlaceBuilding,
            CannotPlaceBuilding,
        }
    }
}
