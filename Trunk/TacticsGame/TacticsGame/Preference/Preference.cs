using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Preference
{
    [Serializable]
    public class Preferences
    {
        private ItemPreference itemPreference = new ItemPreference();

        public ItemPreference ItemPreference
        {
            get { return itemPreference; }
            set { itemPreference = value; }
        }

        private GovernancePreference governancePreference = new GovernancePreference();

        public GovernancePreference GovernancePreference
        {
            get { return governancePreference; }
            set { governancePreference = value; }
        }
    }
}
