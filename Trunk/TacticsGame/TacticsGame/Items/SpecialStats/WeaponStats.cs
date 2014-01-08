using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Abilities;

namespace TacticsGame.Items.SpecialStats
{    
    public class WeaponStats : ItemStats
    {
        public WeaponStats()
        {
            this.Attack = 1;
            this.RangeMax = 1;
            this.RangeMin = 1;
            this.APCost = 1;
        }

        public WeaponType WeaponType { get; set; }

        public int Attack { get; set; }

        public int RangeMin { get; set; }

        public int RangeMax { get; set; }

        public int APCost { get; set; }

        public List<AbilityVisualEffectInfo> VisualEffects { get; set; }     
    }
}
