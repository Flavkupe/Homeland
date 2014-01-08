using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Preference
{
    [Serializable]
    public class GovernancePreference
    {
        /// <summary>
        /// Approximate amount of money the unit is willing to allow being taxed before they will enter town.
        /// </summary>
        private int taxTolerance = 50;

        /// <summary>
        /// How much tax the visitor is willing to tolerate in order to come to town. 0 means they tolerate NO taxes. 50 means they will only pay up to 50 in taxes.
        /// </summary>
        public int TaxTolerance
        {
            get { return taxTolerance; }
            set { taxTolerance = value; }
        }
    }
}
