using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.UI
{
    public class Margin
    {
        public int Left;
        public int Right;
        public int Bottom;
        public int Top;
        public Margin(int left, int top, int bottom, int right)
        {
            this.Left = left;
            this.Top = top;
            this.Bottom = bottom;
            this.Right = right;
        }
    }
}
