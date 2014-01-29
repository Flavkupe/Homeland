using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Interfaces;
using TacticsGame.Map;
using Microsoft.Xna.Framework;

namespace TacticsGame.GameObjects.Structures
{
    [Serializable]
    public class NeighborDependentStructure : Structure, IModifyNeighbors, IMultiTile
    {
        private Rectangle subRectangle;
        private Rectangle? subRectangle2 = null;
        private Rectangle? drawPosition2 = null;

        public NeighborDependentStructure(string structureName)
            : base(structureName)
        {
            this.subRectangle = new Rectangle(0, 0, 32, 32);
        }

        public override void SetLocationTo(Tile newTile, bool setTileResident = true)
        {
            base.SetLocationTo(newTile, setTileResident);

            this.PlacedOnTile(newTile);
        }

        public void PlacedOnTile(Tile tilePlacedOn)
        {
            foreach (Tile tile in tilePlacedOn.Grid.GetTilesAdjacent(tilePlacedOn, true))
            {
                if (this.NeighborIsAffected(tile))
                {
                    (tile.TileResident as IModifyNeighbors).AdjustToNewNeighbors();
                }
            }

            this.AdjustToNewNeighbors();
        }

        ////////////////////////
        # region Ugly Calculations =/    

        public void AdjustToNewNeighbors()
        {
            bool north = this.NeighborIsAffected(this.CurrentTile.GetNorth());
            bool south = this.NeighborIsAffected(this.CurrentTile.GetSouth());
            bool east = this.NeighborIsAffected(this.CurrentTile.GetEast());
            bool west = this.NeighborIsAffected(this.CurrentTile.GetWest());

            this.subRectangle2 = null;
            this.drawPosition2 = null;

            if (!north && !south && !east && !west)
            {
                // the one on the corner (icon)
                this.subRectangle = new Rectangle(0, 0, 32, 32);
            }
            else if (north && south && east && west)
            {
                // center of big one
                this.subRectangle = new Rectangle(16, 48, 32, 32);
            }
            else if (north && !south && east && !west)
            {
                // bottom left
                this.subRectangle = new Rectangle(0, 64, 32, 32);
            }
            else if (north && !south && !east && west)
            {
                // bottom right
                this.subRectangle = new Rectangle(32, 64, 32, 32);
            }
            else if (!north && south && east && !west)
            {
                // top left
                this.subRectangle = new Rectangle(0, 32, 32, 32);
            }
            else if (!north && south && !east && west)
            {
                // top right
                this.subRectangle = new Rectangle(32, 32, 32, 32);
            }
            else if (!north && south && east && west)
            {
                // top middle 
                this.subRectangle = new Rectangle(16, 32, 32, 32);
            }
            else if (north && !south && east && west)
            {
                // bottom middle
                this.subRectangle = new Rectangle(16, 64, 32, 32);
            }
            else if (north && south && east && !west)
            {
                // middle left
                this.subRectangle = new Rectangle(0, 48, 32, 32);
            }
            else if (north && south && !east && west)
            {
                // middle right
                this.subRectangle = new Rectangle(32, 48, 32, 32);
            }
            else if (north && south && !east && !west)
            {
                // north and south... parts of the left and right
                this.subRectangle = new Rectangle(0, 48, 16, 32);
                this.subRectangle2 = new Rectangle(48, 48, 16, 32);
            }
            else if (!north && !south && east && west)
            {
                // east and west... parts of the top and bottom
                this.subRectangle = new Rectangle(16, 32, 32, 16);
                this.subRectangle2 = new Rectangle(16, 80, 32, 16);
            }
            else if (!north && !south && east && !west)
            {
                // only east... parts of the topleft and bottomleft
                this.subRectangle = new Rectangle(0, 32, 32, 16);
                this.subRectangle2 = new Rectangle(0, 80, 32, 16);
            }
            else if (!north && !south && !east && west)
            {
                // only west... parts of the topright and bottomright
                this.subRectangle = new Rectangle(32, 32, 32, 16);
                this.subRectangle2 = new Rectangle(32, 80, 32, 16);
            }
            else if (!north && south && !east && !west)
            {
                // only south... parts of the topright and topleft
                this.subRectangle = new Rectangle(0, 32, 16, 32);
                this.subRectangle2 = new Rectangle(48, 32, 16, 32);
            }
            else if (north && !south && !east && !west)
            {
                // only north... parts of the bottomright and bottomleft
                this.subRectangle = new Rectangle(0, 64, 16, 32);
                this.subRectangle2 = new Rectangle(48, 64, 16, 32);
            }

            this.DrawPosition = new Rectangle(this.DrawPosition.X, this.DrawPosition.Y, this.subRectangle.Width, this.subRectangle.Height);
            if (this.subRectangle2 != null)
            {
                this.drawPosition2 = new Rectangle(this.DrawPosition.X + (32 - this.subRectangle.Width), this.DrawPosition.Y + (32 - this.subRectangle.Height), this.subRectangle2.Value.Width, this.subRectangle2.Value.Height);
            }
        }

        #endregion        
        ////////////////////////

        private bool NeighborIsAffected(Tile neighborTile)
        {
            return neighborTile != null && neighborTile.TileResident != null && neighborTile.TileResident is IModifyNeighbors && neighborTile.TileResident.ObjectName == this.ObjectName;
        }

        /// <summary>
        /// By default structures are 1x1 but can be made bigger
        /// </summary>
        public virtual List<Tile> CurrentTiles
        {
            get { return new List<Tile>() { this.CurrentTile }; }
        }

        /// <summary>
        /// By default 1x1
        /// </summary>
        public virtual int TileWidth
        {
            get { return 1; }
        }

        /// <summary>
        /// By default 1x1
        /// </summary>
        public virtual int TileHeight
        {
            get { return 1; }
        }

        public override void Draw(GameTime time)
        {
            Utilities.DrawTexture2D(this.Sprite.TextureInfo.Texture, this.DrawPosition, this.subRectangle, this.CannotBeBuilt ? (Color?)Color.Red : null);

            if (this.drawPosition2 != null)
            {
                Utilities.DrawTexture2D(this.Sprite.TextureInfo.Texture, this.drawPosition2.Value, this.subRectangle2.Value, this.CannotBeBuilt ? (Color?)Color.Red : null);
            }
        }
    }
}
