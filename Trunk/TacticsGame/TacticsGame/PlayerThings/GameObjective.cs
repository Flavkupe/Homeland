using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.PlayerThings
{
    [Serializable]
    public class GameObjective
    {
        private int? targetDays;

        private int? targetGold;

        public int? TargetGold
        {
            get { return targetGold; }
            set { targetGold = value; }
        }

        public int? TargetDays
        {
            get { return targetDays; }
            set { targetDays = value; }
        }
    }

    public enum ObjectiveStatus
    {
        None,
        Failed,
        Succeeded,
    }
}
