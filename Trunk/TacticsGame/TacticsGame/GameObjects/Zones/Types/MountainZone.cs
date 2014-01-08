using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TacticsGame.GameObjects.Zones
{
    [Serializable]
    public class MountainZone : ExitZone
    {
        public MountainZone(int width, int height)
            : base("mountain", width, height)
        {
            this.drawColor = Color.BurlyWood;
        }

        public override bool LeadsToStone { get { return true; } }
    }
}
