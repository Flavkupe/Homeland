using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.PlayerThings
{
    [Serializable]
    public class GameStatus
    {
        private int currentDay = 0;

        public int CurrentDay
        {
            get { return currentDay; }
            set { currentDay = value; }
        }
    }
}
