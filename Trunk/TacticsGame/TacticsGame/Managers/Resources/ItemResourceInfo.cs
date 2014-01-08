using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Items
{
    public class ItemResourceInfo : GameResourceInfo
    {
        ItemStats stats = null;

        public ItemStats Stats
        {
            get { return stats; }
            set { stats = value; }
        }        
    }
}
