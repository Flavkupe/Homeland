using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Map;

namespace TacticsGame.GameObjects
{
    public interface IMultiTile
    {
        List<Tile> CurrentTiles { get; }

        int TileWidth { get; }
        
        int TileHeight { get; }
    }
}
