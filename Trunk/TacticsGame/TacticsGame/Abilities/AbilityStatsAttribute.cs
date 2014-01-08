using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Abilities
{
    public class AbilityStatsAttribute : Attribute
    {
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public int APCost { get; set; }
        public AbilityStatsAttribute(int apCost = 1, int minRange = 1, int maxRange = 1)
        {
            this.APCost = apCost;
            this.MinRange = minRange;
            this.MaxRange = maxRange;
        }
    }
}

