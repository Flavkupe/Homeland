using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;

namespace TacticsGame.Items.SpecialStats
{    
    public class ArmorStats : ItemStats
    {
        public ArmorStats()
        {
            ArmorType = ArmorType.Medium;
            ArmorSlot = EquipmentSlot.Chest;
            Defense = 0;
        }

        public ArmorType ArmorType { get; set; }

        public EquipmentSlot ArmorSlot { get; set; }

        public int Defense { get; set; }
    }
}
