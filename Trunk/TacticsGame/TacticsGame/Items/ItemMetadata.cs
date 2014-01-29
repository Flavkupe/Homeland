using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Items
{
    [Flags]
    public enum ItemMetadata : long
    {
        None = 0,      
        SmithingIngredient = 1,
        CraftingIngredient = 2,
        ForageLoot = 4,
        MiningLoot = 8,
        HuntingLoot = 16,
        StoneMiningLoot = 32,
        WoodcuttingLoot = 64
    }
}
