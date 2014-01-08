using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Map;

namespace TacticsGame.Interfaces
{
    public interface IModifyNeighbors
    {
        void PlacedOnTile(Tile tilePlacedOn);

        void AdjustToNewNeighbors();
    }
}
