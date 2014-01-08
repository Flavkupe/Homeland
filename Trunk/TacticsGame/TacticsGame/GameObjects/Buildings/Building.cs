using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Map;
using Microsoft.Xna.Framework;
using TacticsGame.Items;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Owners;
using TacticsGame.GameObjects.Visitors;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.GameObjects.Buildings
{
    [Serializable]
    public abstract class Building : GameEntity, IMultiTile, IHasInventory
    {
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        private Tile currentTile;
        private List<Tile> currentTiles;
        private List<DecisionMakingUnit> visitors;

        public Building(string spriteName, int tileDimensions = 32)
            : base(spriteName, ResourceType.GameObject)
        {
            this.TileHeight = this.textureInfo.Height / tileDimensions;
            this.TileWidth = this.textureInfo.Width / tileDimensions;

            if (this.IsBuildingWithVisitors)
            {
                this.Visitors = new List<DecisionMakingUnit>();
            }
        }
        
        /// <summary>
        /// The central tile
        /// </summary>
        public override Tile CurrentTile 
        {
            get { return this.currentTile; }
            set
            {
                this.currentTile = value;
                
                List<Tile> currentTiles = new List<Tile>();
                for (int i = 0; i < this.TileWidth; ++i)
                {
                    for (int j = 0; j < this.TileHeight; ++j)
                    {
                        currentTiles.AddIfNotNull(value.Grid.GetTile(this.currentTile.Coordinate.X + i, this.currentTile.Coordinate.Y + j));
                    }
                }

                this.currentTiles = currentTiles;
            }
        }

        /// <summary>
        /// Gets the CurrentTiles. These are set when CurrentTile is set. Current tile is the top-left of these tiles.
        /// </summary>
        public List<Tile> CurrentTiles
        {
            get
            {
                return this.currentTiles;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime time)
        {
            if (this.Selected)
            {
                this.colorFilter = Color.GreenYellow;
            }
            else if (this.CannotBeBuilt)
            {
                this.colorFilter = Color.DarkRed;
            }
            else
            {
                this.colorFilter = null;
            }
            
            base.Draw(time);
        }       

        public virtual IconInfo Icon { get { return this.GetEntityIcon(); } }

        public Inventory Inventory { get { return this.Stock; } }

        public abstract Inventory Stock { get; }

        public virtual bool IsBuildingWithStock { get { return true; } }

        public virtual bool IsBuildingWithOwner { get { return true; } }

        public virtual bool IsBuildingWithUnits { get { return false; } }

        /// <summary>
        /// A building whose owner will behave on his own accord
        /// </summary>
        public virtual bool IsAutonomousBuilding { get { return true; } }

        public virtual bool IsBuildingWithVisitors { get { return true; } }

        public abstract bool IsBuildingThatSellsThings { get; }

        public abstract bool IsBuildingThatBuysThings { get; }

        public virtual bool CanDeployTroopsToBuilding { get { return false; } }

        /// <summary>
        /// How many units this building can support.
        /// </summary>
        public virtual int UnitCapacity { get { return 0; } }

        public abstract Owner Owner { get; }        

        public List<DecisionMakingUnit> Visitors
        {
            get { return visitors; }
            set { visitors = value; }
        }

        /// <summary>
        /// For tinting
        /// </summary>
        public bool CannotBeBuilt { get; set; }

        public virtual void RefreshAtStartOfTurn()
        {
        }

        /// <summary>
        /// Loads up all the references the owner may have had before getting serialized, such as the owner's reference to this building. 
        /// </summary>
        public override void LoadReferencesFromLists(List<Units.DecisionMakingUnit> units, List<Building> buildings)
        {
            base.LoadReferencesFromLists(units, buildings);

            if (this.Owner != null)
            {
                this.Owner.LoadReferencesFromLists(units, buildings);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            if (this.Owner != null)
            {
                this.Owner.LoadContent();
            }

            if (this.Visitors != null) 
            {
                foreach (DecisionMakingUnit visitor in this.Visitors)
                {
                    visitor.LoadContent();
                }
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            if (this.Owner != null)
            {
                this.Owner.Update(time);
            }
        }
    }
}
