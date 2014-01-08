using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;

namespace TacticsGame.GameObjects
{
    public interface IHasInventory
    {
        Inventory Inventory { get; }
    }
}
