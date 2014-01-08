using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Items
{
    /// <summary>
    /// Different types of armors for different characters.
    /// </summary>
    public enum ArmorType
    {
        Light = 1,
        Medium = 2,
        Heavy = 4,
    }

    /// <summary>
    /// Different armor slots for different pieces.
    /// </summary>
    public enum ArmorSlot
    {
        Chest = 1,
        Feet = 2,
        Head = 4,
    }
}
