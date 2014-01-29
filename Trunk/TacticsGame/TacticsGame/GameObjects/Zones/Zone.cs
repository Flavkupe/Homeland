using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Map;
using Microsoft.Xna.Framework;

namespace TacticsGame.GameObjects.Zones
{
    /// <summary>
    /// A zone where things happen    
    /// </summary>
    [Serializable]
    public abstract class Zone : GameObject, IMultiTile
    {
        [NonSerialized]
        private List<Tile> tiles = null;

        private int tileHeight;

        private int tileWidth;

        protected Color? drawColor = null;

        // For drawing in debug
        private Rectangle? drawBounds = null;  

        public Zone(string zoneName, int width, int height)
            : base(zoneName, ResourceType.Other)
        {
            this.tileWidth = width;
            this.tileHeight = height;           
        }

        /// <summary>
        /// Sets the location of this zone. Must be called when loading zone back from a save as well.
        /// </summary>
        /// <param name="tile"></param>
        public virtual void SetTopLeftTile(Tile tile)
        {
            TileGrid grid = tile.Grid;
            this.tiles = grid.GetRangeOfTiles(tile.Coordinate.X, tile.Coordinate.X + this.tileWidth - 1, tile.Coordinate.Y, tile.Coordinate.Y + this.tileHeight - 1);

            this.drawBounds = new Rectangle(tile.AreaRectangle.Location.X, tile.AreaRectangle.Location.Y, grid.VisibleTileDimensions * this.tileWidth, grid.VisibleTileDimensions * this.tileHeight);
        }

        /// <summary>
        /// All tiles this zone entails
        /// </summary>
        public virtual List<Tile> CurrentTiles { get { return this.tiles; } }
        
        /// <summary>
        /// Width of zone
        /// </summary>
        public virtual int TileWidth
        {
            get { return tileWidth; }
            private set { tileWidth = value; }
        }

        /// <summary>
        /// Height of zone
        /// </summary>
        public virtual int TileHeight
        {
            get { return tileHeight; }
            private set { tileHeight = value; }
        }

        /// <summary>
        /// Draws the outline of the zone... potentially only for debug.
        /// </summary>        
        public override void Draw(GameTime gameTime)
        {
            if (this.drawBounds.HasValue) 
            {
                Color color = (this.drawColor ?? Color.Red) * 0.3f;
                
                Utilities.DrawRectangle(this.drawBounds.Value, color);
            }
        }
    }
}
