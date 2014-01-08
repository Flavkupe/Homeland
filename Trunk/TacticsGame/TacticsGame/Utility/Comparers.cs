using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.Utility
{
    public class Comparers         
    {
        public class UnitReadinessComparer : IComparer<Unit>
        {
            public int Compare(Unit x, Unit y)
            {
                if (x.ID == y.ID)
                {
                    return 0; // same unit
                }

                int relativeAP = y.CurrentStats.ActionPoints.CompareTo(x.CurrentStats.ActionPoints);
                if (relativeAP != 0)
                {
                    return relativeAP;
                }
                else
                {
                    int relative = y.CurrentStats.Cunning.CompareTo(x.CurrentStats.Cunning);
                    return relative == 0 ? -1 : relative; // doesn't seem to handle 0 case the way we want.
                }
            }

            /// <summary>
            /// Returns whether sourceUnit is more ready than targetUnit (or tied in readyness).
            /// </summary>
            public bool UnitIsMoreReadyThan(Unit sourceUnit, Unit targetUnit)
            {
                return this.Compare(sourceUnit, targetUnit) <= 0;
            }
        }
    }
}
