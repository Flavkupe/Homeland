using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TacticsGame.Map;

namespace TacticsGame.GameObjects.Buildings
{
    public interface IBuildable : IMultiTile
    {        
        int MoneyCost { get; }

        List<ObjectValuePair<string>> ResourceCost { get; }                

        IconInfo Icon { get; }

        Rectangle DrawPosition { get; set; }

        void SetLocationTo(Tile tile);

        Point Coordinates { get; set; }

        Tile CurrentTile { get; set; }

        /// <summary>
        /// For tinting
        /// </summary>
        bool CannotBeBuilt { get; set; }

        /// <summary>
        /// For display
        /// </summary>
        string DisplayName { get; }

        bool BuildAgain { get; }
    }
}
