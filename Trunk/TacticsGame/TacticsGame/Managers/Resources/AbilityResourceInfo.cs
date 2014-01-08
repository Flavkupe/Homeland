using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Abilities;

namespace TacticsGame.Managers
{
    public class AbilityResourceInfo : GameResourceInfo
    {              
        public AbilityResourceInfo()
        {
            this.Stats = new AbilityStats();
            this.VisualEffects = new List<AbilityVisualEffectInfo>();
        }

        public AbilityStats Stats { get; set; }

        public List<AbilityVisualEffectInfo> VisualEffects { get; set; }                 
    }
}
