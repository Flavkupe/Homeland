using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.World
{
    public class GameWorld
    {
        public const int DayEndHour = 22;
        public const int DayStartHour = 8;

        private List<ForeignTownInfo> foreignTowns = new List<ForeignTownInfo>();

        public List<ForeignTownInfo> ForeignTowns
        {
            get { return foreignTowns; }
            set { foreignTowns = value; }
        }

        private TownState currentTown = new TownState();

        public TownState CurrentTown
        {
            get { return this.currentTown; }
            set { this.currentTown = value; }
        }

        private DateTime worldTime;

        public DateTime WorldTime
        {
            get { return worldTime; }
            set { worldTime = value; }
        }
    }
}
