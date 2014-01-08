using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Items
{
    public interface IDropLoot
    {
        IEnumerable<Item> GetLootDrop();
    }
}
