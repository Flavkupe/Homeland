using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Preference;

namespace TacticsGame.World
{
    public class ForeignTownInfo
    {        
        private string townDisplayName;

        private Preferences preferences = new Preferences();

        public ForeignTownInfo(string displayName)
        {
            this.townDisplayName = displayName;            
        }

        public string TownDisplayName
        {
            get { return townDisplayName; }
            set { townDisplayName = value; }
        }        

        public Preferences Preferences
        {
            get { return preferences; }
            set { preferences = value; }
        }
    }
}
