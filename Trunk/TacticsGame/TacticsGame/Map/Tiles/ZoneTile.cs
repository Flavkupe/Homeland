using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Utility;
using Microsoft.Xna.Framework;
using TacticsGame.Attributes;
using TacticsGame.GameObjects;
using TacticsGame.Managers;

namespace TacticsGame.Map
{
    /// <summary>
    /// A 32x32 tile that has a graphic and coordinate
    /// </summary>
    [Serializable]
    public class ZoneTile : Tile
    {
        public ZoneTile(string type, Point coords, Point drawPosition, int dimensions, TileGrid grid) 
            : base(type, grid)
        {
            this.Coordinate = coords;
            this.Dimensions = dimensions;
            this.AreaRectangle = new Rectangle(drawPosition.X, drawPosition.Y, dimensions, dimensions);
        }

        public override string TileDisplayName
        {
            get { return this.DisplayName; }
        }

        /// <summary>
        /// Draws each item in the passable content list, in order.
        /// </summary>
        /// <param name="gameTime"></param>
        private void DrawPassableContent(GameTime gameTime)
        {
            if (this.PassableContents != null)
            {
                foreach (GameEntity entity in this.PassableContents)
                {
                    entity.Draw(gameTime);
                }
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            GameResourceInfo info = GameResourceManager.Instance.GetResourceByResourceType(this.ObjectName, ResourceType.Tile);
            this.DisplayName = info.DisplayName;
            this.texture = info.TextureInfo.Texture;
        }

        public override void Draw(GameTime gameTime, Tile.TileDrawFilter filter)
        {
            switch (filter) 
            {
                case TileDrawFilter.None:
                    Utilities.DrawTexture2D(texture, AreaRectangle, null, true);
                    break;
                case TileDrawFilter.TileMovement:
                    Utilities.DrawTexture2D(texture, AreaRectangle, Color.LightBlue, true);
                    break;
                case TileDrawFilter.AttackRange:
                    Utilities.DrawTexture2D(texture, AreaRectangle, Color.DarkRed, true);
                    break;
                case TileDrawFilter.MovementAndAttackRange:
                    Utilities.DrawTexture2D(texture, AreaRectangle, Color.LightPink, true);
                    break;
                case TileDrawFilter.CannotPlaceBuilding:
                    Utilities.DrawTexture2D(texture, AreaRectangle, Color.Red, true);
                    break;
                case TileDrawFilter.CanPlaceBuilding:
                    Utilities.DrawTexture2D(texture, AreaRectangle, Color.LightBlue, true);
                    break;
            }

            this.DrawPassableContent(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            this.Draw(gameTime, TileDrawFilter.None);            
        }   
    }
}
