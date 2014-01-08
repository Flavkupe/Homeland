using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TacticsGame.GameObjects.Zones
{
    [Serializable]
    public class WoodlandZone : ExitZone
    {
        public WoodlandZone(int width, int height)
            : base("woods", width, height)
        {
            this.drawColor = Color.ForestGreen;
        }

        public override bool LeadsToHunt { get { return true; } }

        public override bool LeadsToWood { get { return true; } }
    }
}
