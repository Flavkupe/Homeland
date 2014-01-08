using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Preference
{
    public static class PreferenceValue
    {
        public static int NegativeHigh { get { return -50; } }
        public static int NegativeMed { get { return -20; } }
        public static int NegativeLow { get { return -10; } }
        public static int Neutral { get { return 0; } }
        public static int Low { get { return 10; } }
        public static int Medium { get { return 20; } }
        public static int High { get { return 50; } }
    }
}
