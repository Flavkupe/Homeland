using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;

namespace TacticsGame.AI.MaintenanceMode
{
    public partial class UnitDecisionUtils
    {
        public string GetStringForUnitTaxed(UnitDecisionActivity activity, int moneyLost)
        {
            return this.GetStringForUnitTaxed(activity.Unit, moneyLost);
        }

        public string GetStringForUnitTaxed(Unit unit, int moneyLost)
        {
            if (moneyLost > 0)
            {
                return string.Format("{0} was taxed {1} moneys for the day.", unit.DisplayName, moneyLost);
            }
            else
            {
                return string.Format("{0} could not afford the tax.", unit.DisplayName);
            }
        }

        public string GetDecisionStringForActionStarted(UnitDecisionActivity activity)
        {
            Decision decision = activity.Decision;
            Unit unit = activity.Unit;

            if (decision == Decision.GetLumber)
            {
                return string.Format("{0} went to gather wood.", unit.DisplayName);
            }
            else if (decision == Decision.MineStone)
            {
                return string.Format("{0} went to the stone quarry.", unit.DisplayName);
            }
            else if (decision == Decision.Sell)
            {
                return activity.TargetBuilding != null ? string.Format("{0} went to sell something at {1}.", unit.DisplayName, activity.TargetBuilding.DisplayName) :
                                                         string.Format("{0} is selling to {1}.", unit.DisplayName, activity.TargetVisitor.DisplayName);
            }
            else if (decision == Decision.Buy)
            {
                return activity.TargetBuilding != null ? string.Format("{0} went to buy something at {1}.", unit.DisplayName, activity.TargetBuilding.DisplayName) :
                                                         string.Format("{0} is buying from {1}.", unit.DisplayName, activity.TargetVisitor.DisplayName); 
            }
            else
            {
                return string.Format("{0} went to rest at home.", unit.DisplayName);
            }
        }

        public string GetDecisionStringForUnableToFindProperBuilding(UnitDecisionActivity activity)
        {
            return string.Format("{0} was unable to find a suitable store!", activity.Unit.DisplayName);
        }       

        public string GetDecisionStringForActionComplete(UnitDecisionActivity activity, ActivityResult results)
        {
            return GetDecisionStringForActionComplete(activity.Unit, activity.Decision, results);
        }

        public string GetDecisionStringForActionComplete(Unit unit, Decision decision, ActivityResult results)
        {
            if (decision == Decision.GetLumber || decision == Decision.MineStone || decision == Decision.MineGems)
            {
                if (results.ItemsGained != null && results.ItemsGained.Count > 0)
                {
                    return string.Format("{0} returned with {1}.", unit.DisplayName, GetStringForListOfItems(results.ItemsGained));
                }
                else
                {
                    return string.Format("{0} returned empty-handed.", unit.DisplayName);
                }
            }
            else if (decision == Decision.Buy)
            {
                string target = results.TransactionTarget.DisplayName;
                if (results.ItemsGained != null)
                {                    
                    return string.Format("{0} purchased {1} from {2} for {3} moneys.", unit.DisplayName, GetStringForListOfItems(results.ItemsGained), target, results.MoneyLost);
                }
                else
                {
                    return string.Format("{0} did not buy anything from {1}.", unit.DisplayName, target);
                }
            }
            else if (decision == Decision.Sell)
            {
                string target = results.TransactionTarget.DisplayName;
                if (results.ItemsLost != null)
                {
                    return string.Format("{0} sold {1} to {2} for {3} moneys.", unit.DisplayName, GetStringForListOfItems(results.ItemsLost), target, results.MoneyMade);
                }
                else
                {
                    return string.Format("{0} did not sell anything to {1}.", unit.DisplayName, target);
                }
            }
            else if (decision == Decision.Craft)
            {
                return string.Format("{0} crafted {1} using {2}.", unit.DisplayName, GetStringForListOfItems(results.ItemsGained), GetStringForListOfItems(results.ItemsLost));
            }
            else
            {
                return string.Format("{0} rested at home.", unit.DisplayName);
            }
        }        

        public string GetDecisionStringForUnitDone(Unit unit)
        {
            return string.Format("{0} is done for the day.", unit.DisplayName);
        }

        private string GetStringForListOfItems(List<Item> items)
        {
            StringBuilder final = new StringBuilder();

            Dictionary<string, int> itemCount = new Dictionary<string, int>();
            foreach (Item item in items)
            {
                if (itemCount.ContainsKey(item.ObjectName))
                {
                    itemCount[item.ObjectName]++;
                }
                else
                {
                    itemCount[item.ObjectName] = 1;
                }
            }

            int current = 0;
            foreach (string key in itemCount.Keys)
            {
                final.Append(key + " x " + itemCount[key]);
                if (current < itemCount.Keys.Count - 1)
                {
                    final.Append(", ");
                }
            }

            return final.ToString();
        }
    }
}
