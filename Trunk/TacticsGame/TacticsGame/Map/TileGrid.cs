using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Utility;
using System.Diagnostics;
using TacticsGame.GameObjects.Units;
using TacticsGame.Abilities;

namespace TacticsGame.Map
{
    /// <summary>
    /// A datastructure used to hold tiles
    /// </summary>
    [Serializable]
    public class TileGrid
    {                
        private Tile[,] tiles;

        [NonSerialized]
        private HashSet<Tile> movementRadius = null;

        [NonSerialized]
        private HashSet<Tile> targetsRadius = null;

        [NonSerialized]
        private Point? selectedTileCoords;

        [NonSerialized]
        private Tile selectedTile;

        [NonSerialized]
        private EventHandler<TileClickedEventArgs> tileClicked;

        private int baseTileDimensions = 32;

        /// <summary>
        /// Event for when the mouse clicks on a tile.
        /// </summary>
        public event EventHandler<TileClickedEventArgs> TileClicked
        {
            add
            {
                if (tileClicked == null || !tileClicked.GetInvocationList().Contains(value))
                {
                    tileClicked += value;
                }
            }
            remove
            {
                tileClicked -= value;
            }
        }

        public class TileClickedEventArgs : EventArgs 
        {
            public Tile ClickedTile { get; private set; }
            public bool SelectionChanged { get; private set; }
            public TileClickedEventArgs(Tile newTile, bool selectionChanged)
            {
                this.ClickedTile = newTile;
                this.SelectionChanged = selectionChanged;
            }
        }

        /// <summary>
        /// Get coords of the selected tile (tile coords, not pixels). Null for not selected
        /// </summary>
        public Point? SelectedTileCoords
        {
            get { return this.selectedTileCoords; }
        }

        /// <summary>
        /// How many tiles accross (Y axis)
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// How many tiles accross (X axis)
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// How many pixels wide the level is
        /// </summary>
        public int LevelPixelWidth { get { return this.Width * this.baseTileDimensions; } }

        /// <summary>
        /// How many pixels high the level is
        /// </summary>
        public int LevelPixelHeight { get { return this.Height * this.baseTileDimensions; } }

        /// <summary>
        /// Gets the actual selected tile item. Null for not selected.
        /// </summary>
        public Tile SelectedTile
        {
            get { return selectedTile; }
        }

        /// <summary>
        /// The dimensions of each tile. Most likely 32x32. All tiles all square
        /// </summary>
        public int VisibleTileDimensions 
        {
            get
            {
                float zoom = GameStateManager.Instance.ZoomLevel;
                return zoom == 1.0f ? this.baseTileDimensions : (int)((float)this.baseTileDimensions * zoom);
            }
        } 

        public HashSet<Tile> MovementRadius { get { return this.movementRadius; } }

        /// <summary>
        /// Create a grid for drawing tiles
        /// </summary>
        /// <param name="width">The map accross (X axis)</param>
        /// <param name="length">The map accross (Y axis)</param>
        /// <param name="dimensions">The tile dimensions in pixels. All tiles are square.</param>
        public TileGrid(int width, int length, int dimensions) 
        {            
            this.Width = width;
            this.Height = length;
            
            this.baseTileDimensions = dimensions;

            this.tiles = new Tile[width, length];
        }

        /// <summary>
        /// Handles the mouse being released somewhere on the grid.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        public void MouseReleased(int mouseX, int mouseY)
        {
            this.selectedTile = null;

            float zoom = GameStateManager.Instance.ZoomLevel;
            int tileDimensions = this.VisibleTileDimensions;
            
            int coordX = mouseX / tileDimensions;
            int coordY = mouseY / tileDimensions;                      

            if (coordX >= 0 && coordX < this.Width && coordY >= 0 && coordY < this.Height)
            {                
                Point coords = new Point(coordX, coordY);
                bool tileSelectionChanged = (selectedTileCoords == null || !coords.Equals(this.selectedTileCoords));
                
                this.selectedTileCoords = coords;

                Debug.Assert(this.tiles[coordX, coordY] != null, "Null tile!!");

                this.selectedTile = this.tiles[coordX, coordY];

                if (this.tileClicked != null)
                {
                    this.tileClicked(this, new TileClickedEventArgs(this.SelectedTile, tileSelectionChanged));
                }                
            }
        }

        /// <summary>
        /// Gets the tile at coord (x,y)
        /// </summary>
        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || y >= this.Height || x >= this.Width)
            {
                return null;
            }

            return this.tiles[x, y];            
        }

        public Tile GetTile(int x, int y, bool getNonTraversable)
        {
            Tile tile = this.GetTile(x, y);

            if (tile != null && !tile.IsTraversable && !getNonTraversable)
            {
                return null;
            }

            return tile;
        }

        /// <summary>
        /// Loads the grid contents.
        /// </summary>
        public void LoadTiles()
        {          
            //TEMP: eventually this should be done by reading from a script or file
            for (int x = 0; x < this.Width; ++x)
            {
                for (int y = 0; y < this.Height; ++y)
                {
                    ZoneTile tile = new ZoneTile("Grass", new Point(x, y), new Point(x * this.VisibleTileDimensions, y * this.VisibleTileDimensions), this.VisibleTileDimensions, this);
                    this.tiles[x, y] = tile;
                }
            }
        }

        public void SetDisplayedMovementRadius(Unit unit)
        {
            this.SetDisplayedMovementRadius(unit.CurrentTile, unit.CurrentStats.ActionPoints);
        }

        public void SetDisplayedMovementRadius(Tile center, int radius)
        {
            this.movementRadius = this.GetTileRadius(center, radius);
        }

        public void ClearMovementRadius()
        {
            this.movementRadius = null;
        }

        public void ClearTargetRadius()
        {
            this.targetsRadius = null;
        }

        public bool TileIsInMovementRange(Tile target)
        {
            return movementRadius != null && this.movementRadius.Contains(target);
        }

        public bool TileIsInTargetRange(Tile target)
        {
            return this.targetsRadius != null && this.targetsRadius.Contains(target);
        }

        public void SetDisplayedAbilityRadius(Unit centerUnit, AbilityInfo usedAbility)
        {
            this.targetsRadius = this.GetTileRadius(centerUnit.CurrentTile, usedAbility.Stats.MaxRange, true, usedAbility.Stats.MinRange);
        }

        public void SetDisplayedAttackRadius(Unit centerUnit)
        {
            this.targetsRadius = this.GetTileRadius(centerUnit.CurrentTile, centerUnit.GetAttackMaxRange(), true, centerUnit.GetAttackMinRange());
        }

        public void ClearCustomDrawFilters()
        {
            foreach (Tile tile in this.tiles)
            {
                tile.Filter = null;
            }
        }

        /// <summary>
        /// Gets closest tile using manhattan distance.
        /// </summary>
        /// <param name="tilesAvailable">List of tiles we are interested in moving to.</param>
        /// <param name="target">Tile we want to go to.</param>
        /// <param name="preferredTile">Tile we have preference for. For example, starting tile.</param>
        /// <returns>Tile from tilesAvailable closest to target</returns>
        public Tile GetClosestTileTo(ICollection<Tile> tilesAvailable, Tile target, Tile preferredTile = null)
        {
            int minDistance = preferredTile == null ? Int32.MaxValue : this.GetManhattanDistance(preferredTile, target);
            Tile minTile = preferredTile;            

            foreach (Tile tile in tilesAvailable)
            {
                int newDistance = this.GetManhattanDistance(tile, target);
                if (minTile == null || newDistance < minDistance)
                {
                    minDistance = newDistance;
                    minTile = tile;
                }
            }

            return minTile;
        }

        private int GetManhattanDistance(Tile tile1, Tile tile2)
        {
            return Math.Abs(tile1.Coordinate.X - tile2.Coordinate.X) + Math.Abs(tile1.Coordinate.Y - tile2.Coordinate.Y);
        }

        /// <summary>
        /// Given radius, gets a set containing the tiles reachable within that radius.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<Tile> GetTileRadius(Tile center, int radius, bool ignoreTraverability = false, int minRadius = 1)
        {
            HashSet<Tile> tiles = new HashSet<Tile>();
            
            if (radius == 0)
            {
                return tiles;
            }

            HashSet<Tile> closedList = new HashSet<Tile>();
            Stack<Tile> currentTiles = new Stack<Tile>();
            Stack<Tile> nextTiles = new Stack<Tile>();            

            currentTiles.PushRange<Tile>(this.GetTilesAdjacent(center, ignoreTraverability));
            closedList.AddRangeIfNotExists<Tile>(currentTiles);

            if (minRadius < 2)
            {
                tiles.AddRangeIfNotExists<Tile>(currentTiles);                
            }

            if (minRadius == 0)
            {
                tiles.AddIfNotExists(center);
            }

            for (int i = 1; i < radius; ++i) 
            {
                while (currentTiles.Count > 0)
                {
                    Tile topTile = currentTiles.Pop();
                    List<Tile> adjacent = this.GetTilesAdjacent(topTile, ignoreTraverability);                    

                    foreach (Tile tile in adjacent)
                    {
                        if (!closedList.Contains(tile))
                        {
                            nextTiles.Push(tile);
                            closedList.Add(tile);

                            if (i >= minRadius - 1)
                            {
                                tiles.Add(tile);
                            }
                        }
                    }
                }
                
                currentTiles = new Stack<Tile>(nextTiles);
                nextTiles.Clear();
            }

            if (minRadius > 0)
            {
                tiles.Remove(center);
            }

            return tiles;
        }

        /// <summary>
        /// Gets the adjacent tiles.
        /// </summary>
        /// <param name="center">Tile to start from.</param>
        /// <param name="getNonTraversable">If this is false, then it gets tiles that are IsTraversable as well.</param>
        /// <returns></returns>
        public List<Tile> GetTilesAdjacent(Tile center, bool getNonTraversable = false)
        {            
            List<Tile> list = new List<Tile>();
            list.AddIfNotNull<Tile>(this.GetTile(center.Coordinate.X - 1, center.Coordinate.Y, getNonTraversable));
            list.AddIfNotNull<Tile>(this.GetTile(center.Coordinate.X + 1, center.Coordinate.Y, getNonTraversable));
            list.AddIfNotNull<Tile>(this.GetTile(center.Coordinate.X, center.Coordinate.Y - 1, getNonTraversable));
            list.AddIfNotNull<Tile>(this.GetTile(center.Coordinate.X, center.Coordinate.Y + 1, getNonTraversable));
            return list;
        }

        #region A*        

        /// <summary>
        /// Uses A* to find the path between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Tile> GetPathBetween(Tile start, Tile end, bool includeFirst = false, bool dontCountIfLastIsNonTraversable = true)
        {
            if (end == null || (!end.IsTraversable && dontCountIfLastIsNonTraversable))
            {
                return null;
            }

            HashSet<NodeInfo> closedList = new HashSet<NodeInfo>();
            HashSet<NodeInfo> openList = new HashSet<NodeInfo>();

            openList.Add(new NodeInfo(0, start, null));

            bool found = false;
            NodeInfo current = null;
            while (openList.Count > 0) 
            {
                current = this.GetBestNode(openList);
                openList.Remove(current);
                closedList.Add(current);

                if (current.Tile == end)
                {
                    found = true;
                    break;
                }

                foreach(Tile tile in this.GetTilesAdjacent(current.Tile, true)) 
                {
                    if ((!tile.IsTraversable || this.GetTileFromList(closedList, tile) != null))
                    {
                        // If the tile is the end tile, then either it's nontraversable and we have already returned null, or we want to get its value either way.
                        if (tile != end)
                        {
                            continue;
                        }
                    }

                    NodeInfo existingTile = this.GetTileFromList(openList, tile);
                    if (existingTile == null)
                    {
                        openList.Add(new NodeInfo(this.GetFCost(current, end), tile, current));
                    }
                    else if (existingTile.Cost > current.Cost)
                    {
                        existingTile.Parent = current;
                        existingTile.Cost = this.GetFCost(current, end);
                    }                    
                }
            }

            if (!found || current == null)
            {
                return null;
            }

            List<Tile> path = new List<Tile>();

            while (current.Parent != null)
            {
                // TODO: this is dangerous...
                path.Add(current.Tile);
                current = current.Parent;
            }

            if (includeFirst)
            {
                path.Add(start);
            }

            path.Reverse();
            return path;
        }

        private int GetFCost(NodeInfo currentNode, Tile targetTile)
        {
            int G = currentNode.Cost + currentNode.Tile.Cost;
            int H = Math.Abs(targetTile.Coordinate.X - currentNode.Tile.Coordinate.X) + Math.Abs(targetTile.Coordinate.Y - currentNode.Tile.Coordinate.Y);
            return G + H;
        }

        private NodeInfo GetTileFromList(HashSet<NodeInfo> list, Tile tile)
        {
            foreach (NodeInfo node in list)
            {
                if (node.Tile == tile) 
                {
                    return node; 
                }
            }

            return null;
        }

        private NodeInfo GetBestNode(HashSet<NodeInfo> list)
        {
            NodeInfo best = null;

            foreach (NodeInfo node in list)
            {
                if (best == null || node.Cost < best.Cost)
                {
                    best = node;
                }
            }

            return best;
        }

        private class NodeInfo
        {
            public int Cost { get; set; }
            public Tile Tile { get; set; }
            public NodeInfo Parent { get; set; } 
            public NodeInfo(int cost, Tile tile, NodeInfo parent)
            {
                this.Cost = cost;
                this.Tile = tile;
                this.Parent = parent;
            }
        }

        #endregion

        public virtual void LoadContent()
        {
            foreach (Tile tile in this.tiles)
            {
                tile.LoadContent();
                tile.Grid = this;
            }
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Draws all the tiles from the grid.
        /// </summary>
        /// <param name="time"></param>
        public virtual void Draw(GameTime time)
        {            
            Rectangle camera = GameStateManager.Instance.CameraView;
            float zoom = GameStateManager.Instance.ZoomLevel;

            int tileDimensions = this.VisibleTileDimensions;            
            int cameraX = camera.X;
            int cameraY = camera.Y;
            
            int startingX = Math.Max(0, cameraX / tileDimensions);
            int startingY = Math.Max(0, cameraY / tileDimensions);
            int endingX = Math.Min(this.Width - 1, (cameraX + camera.Width) / tileDimensions);
            int endingY = Math.Min(this.Height - 1, (cameraY + camera.Height) / tileDimensions);

            for (int x = startingX; x <= endingX; ++x)
            {
                for (int y = startingY; y <= endingY; ++y)
                {
                    Tile tile = this.tiles[x, y];

                    // First draw the custom filters. Then draw the standard ones.
                    if (tile.Filter != null)
                    {
                        tile.Draw(time, tile.Filter.Value);
                    }
                    else
                    {
                        if (this.targetsRadius != null && this.movementRadius != null && this.targetsRadius.Contains(tile) && this.movementRadius.Contains(tile))
                        {
                            tile.Draw(time, Tile.TileDrawFilter.MovementAndAttackRange);
                        }
                        else if (this.targetsRadius != null && this.targetsRadius.Contains(tile))
                        {
                            tile.Draw(time, Tile.TileDrawFilter.AttackRange);
                        }
                        else if (this.movementRadius != null && this.movementRadius.Contains(tile))
                        {
                            tile.Draw(time, Tile.TileDrawFilter.TileMovement);
                        }
                        else
                        {
                            tile.Draw(time, Tile.TileDrawFilter.None);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a range of tiles given the specified coordinate bounds.
        /// </summary>
        /// <param name="tileXMin">Leftmost tile coords</param>
        /// <param name="tileXMax">Rightmost tile coords</param>
        /// <param name="tileYMin">Topmost tile coords</param>
        /// <param name="tileYMax">Bottommost tile coords</param>
        /// <param name="onlyGetEmptyTiles">If true, only counts tile in range if empty.</param>
        /// <returns></returns>
        public List<Tile> GetRangeOfTiles(int tileXMin, int tileXMax, int tileYMin, int tileYMax, bool onlyGetEmptyTiles = false)
        {
            List<Tile> range = new List<Tile>();
            for (int x = tileXMin; x < tileXMax; ++x)
            {
                if (x >= 0 && x < this.Width)
                {
                    for (int y = tileYMin; y < tileYMax; ++y)
                    {
                        if (y >= 0 && y < this.Height)
                        {
                            Tile tile = this.tiles[x,y];
                            if (!onlyGetEmptyTiles || (onlyGetEmptyTiles && tile.IsTraversable))
                            {
                                range.Add(tile);
                            }
                        }
                    }
                }
            }

            return range;
        }
    }
}

