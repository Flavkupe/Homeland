using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.World
{
    public class GameWorld
    {
        private List<ForeignTownInfo> foreignTowns = new List<ForeignTownInfo>();

        public List<ForeignTownInfo> ForeignTowns
        {
            get { return foreignTowns; }
            set { foreignTowns = value; }
        }


    }
}
