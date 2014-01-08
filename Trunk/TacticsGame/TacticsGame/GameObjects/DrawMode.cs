using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.GameObjects
{
    public enum DrawMode
    {
        Normal = 0, // No change
        Waiting = 1, // Usually darkened gray
        Done = 2, // Usually pitch black
        Selected = 3, // Usually light green
        NotAllowed = 4, // Usually red

    }
}
