using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.Utility;

namespace TacticsGame.GameObjects.Visitors.Types
{
    /// <summary>
    /// Type of visitor that gives advice.
    /// </summary>
    [Serializable]    
    public class Traveller : Visitor
    {
        public Traveller()
            : base("Traveller")
        {
            this.Preferences.GovernancePreference.TaxTolerance = Utilities.GetRandomNumber(0, 300);
            this.Inventory.Money = Utilities.GetRandomNumber(0, 300);
        }

        public override bool IsTrader { get { return false; } }

        /// <summary>
        /// Generates a random message based on the type of traveller and stuff.
        /// </summary>
        /// <returns></returns>
        public string GenerateMessage()
        {
            return CommentGenerationUtilities.GenerateGenericVisitorComment();                       
        }
    }
}
