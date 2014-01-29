using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;

namespace TacticsGame.AI.MaintenanceMode.Engines
{
    public class SimplifiedPreferenceEngine : EducatedPreferenceEngine
    {
        public override int MakeAppraisal(GameObjects.Units.IMakeDecisions unit, Items.Item item, int boost = 0)
        {
            // In the simplified model, all units know the cost by default
            return item.Stats.Cost;
        }

        public override int ImproveBid(GameObjects.Units.IMakeDecisions unit, Items.Item item, int currentBid)
        {
            // Lower bid by 10 percent each time bid is refused
            int newBid = currentBid - (int)((double)currentBid / 10.0d);
            return newBid > this.GetMinimumSellValue(unit, item) ? newBid : 0;            
        }

        /// <summary>
        /// Minimum value the unit is willing to sell the item for
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public override int GetMinimumSellValue(GameObjects.Units.IMakeDecisions unit, Item item)
        {
            return item.Stats.Cost / 4;
        }

        public override int ExpectedSellValue(DecisionMakingUnit a, Item item)
        {
            // In simplified model, unit knows exact result of bidding early on
            return this.MakeBid(a, item, true);                
        }           
    }
}
