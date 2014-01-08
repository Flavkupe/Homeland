using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.GameObjects.Zones
{
    [Serializable]
    public abstract class ExitZone : Zone
    {
        public ExitZone(string zoneType, int width, int height)
            : base(zoneType, width, height)
        {
        }

        public virtual bool LeadsToWood { get { return false; } }
        public virtual bool LeadsToStone { get { return false; } }
        public virtual bool LeadsToHunt { get { return false; } } 
    }
}
