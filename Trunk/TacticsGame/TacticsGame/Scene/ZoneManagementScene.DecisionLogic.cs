using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Owners;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;
using TacticsGame.Managers;
using TacticsGame.Text;

namespace TacticsGame.Scene
{
    public partial class ZoneManagementScene
    {
        /// <summary>
        /// Shows the results of the activity in the action feed (unit view) and on the world (when applicable). Does not show text.
        /// </summary>
        protected void ShowActivityResults(UnitDecisionActivity activity, ActivityResult results)
        {            
            if (activity.Unit is Owner)
            {
                this.ShowBuildingActionResultText(results, activity.Unit as Owner);
            }                      

            this.ShowActivityResultsOnActionFeed(activity, results);
        }

        /// <summary>
        /// Shows the results from an action on a building as floating text with icons.
        /// </summary>
        private void ShowBuildingActionResultText(ActivityResult results, Owner owner)
        {
            Building building = owner.OwnedBuilding;
            float x = building.DrawPosition.Left;
            float y = building.DrawPosition.Top;

            if (results.ItemsGained != null)
            {
                HashSet<string> used = new HashSet<string>();
                foreach (Item item in results.ItemsGained)
                {
                    if (!used.Contains(item.ObjectName))
                    {
                        used.Add(item.ObjectName);
                        string text = "+ " + results.GainedItemsCounter.GetItemCount(item);
                        float horizontalLoc = Utilities.GetRandomNumber((int)x, (int)(x + building.DrawPosition.Width)) - 50.0f;
                        FloatingText floatingText = new FloatingText(text, new Vector2(horizontalLoc, y), item.Icon, 2000, Color.DarkOrange);
                        this.floatingText.Add(floatingText);
                        y -= 30;
                    }
                }
            }

            if (results.ItemsLost != null)
            {
                HashSet<string> used = new HashSet<string>();
                foreach (Item item in results.ItemsLost)
                {
                    if (!used.Contains(item.ObjectName))
                    {
                        used.Add(item.ObjectName);
                        string text = "- " + results.LostItemsCounter.GetItemCount(item);
                        float horizontalLoc = Utilities.GetRandomNumber((int)x, (int)(x + building.DrawPosition.Width)) - 50.0f;
                        FloatingText floatingText = new FloatingText(text, new Vector2(horizontalLoc, y), item.Icon, 2000, Color.Maroon);
                        this.floatingText.Add(floatingText);
                        y -= 30;
                    }
                }
            }

            if (results.MoneyMade != null)
            {
                IconInfo icon = TextureManager.Instance.GetIconInfo("Coin");
                string text = "+ " + results.MoneyMade.Value;
                FloatingText floatingText = new FloatingText(text, new Vector2(x, y), icon);
                this.floatingText.Add(floatingText);
                y -= 30;
            }
            else if (results.MoneyLost != null)
            {
                IconInfo icon = TextureManager.Instance.GetIconInfo("Coin");
                string text = "- " + results.MoneyLost.Value;
                FloatingText floatingText = new FloatingText(text, new Vector2(x, y), icon);
                this.floatingText.Add(floatingText);
                y -= 30;
            }            
        }
        
        /// <summary>
        /// Generates a new decision when needed
        /// </summary>
        /// <param name="activity">The decision activity object for the unit.</param>
        /// <param name="avoidShops">If true, unit should actively avoid shops</param>
        private void GenerateNewDecisionForUnit(UnitDecisionActivity activity)
        {
            Decision decision = this.unitDecisionEngine.MakeDecision(activity.Unit);
            int decisionDuration = this.unitDecisionEngine.DetermineDecisionDuration(activity.Unit, decision);
            activity.NewDecision(decision, decisionDuration);
            
            this.UpdateActivityOnUI(activity);

            if (UnitDecisionUtils.DecisionRequiresBuilding(decision))
            {
                activity.TargetBuilding = this.unitDecisionEngine.DetermineTargetBuilding(activity.Unit, decision, this.Buildings);

                if (activity.TargetBuilding == null)
                {
                    this.AnnounceTextToUI(this.unitDecisionUtils.GetDecisionStringForUnableToFindProperBuilding(activity));
                    activity.Unit.PreviousDecision = decision;
                    activity.Unit.LastResult = new ActivityResult();
                    activity.Unit.LastResult.ActionSuccess = false;
                    this.GenerateNewDecisionForUnit(activity);
                    return;
                }
            }

            this.AnnounceTextToUI(this.unitDecisionUtils.GetDecisionStringForActionStarted(activity));
        }

        /// <summary>
        /// Generates a new decision when needed
        /// </summary>
        /// <param name="activity">The decision activity object for the unit.</param>
        /// <param name="avoidShops">If true, unit should actively avoid shops</param>
        private void GenerateNewDecisionForOwner(UnitDecisionActivity activity)
        {
            Debug.Assert(activity.Unit is Owner);
            Decision decision = this.unitDecisionEngine.MakeOwnerDecision(activity.Unit as Owner);
            activity.NewDecision(decision, 1);
            if (UnitDecisionUtils.DecisionRequiresVisitor(decision))
            {
                DecisionMakingUnit visitor = this.unitDecisionEngine.DetermineTargetVisitor(activity.Unit, decision, (activity.Unit as Owner).OwnedBuilding.Visitors);
                activity.TargetVisitor = visitor;
                if (visitor == null)
                {
                    // Did not find a visitor... for now, assume end of turn                    
                    activity.NewDecision(Decision.Idle, 1);
                    activity.DoneForTurn = true;
                }
                else
                {
                    this.AnnounceTextToUI(this.unitDecisionUtils.GetDecisionStringForActionStarted(activity));
                }
            }                        
        }

        /// <summary>
        /// When the turn is done. Do all end-of-turn things.
        /// </summary>
        private void ProcessTurnDone()
        {
            List<UnitDecisionActivity> unitActivities = new List<UnitDecisionActivity>(this.decisionActivities);
            unitActivities.AddRange(this.shopOwnerDecisionActivities);

            foreach (UnitDecisionActivity activity in unitActivities)
            {
                // Tax
                if (PlayerStateManager.Instance.ActiveTown.DailyTaxes > 0)
                {
                    DecisionMakingUnit unit = activity.Unit;
                    int tax = (int)((float)unit.Inventory.Money * ((float)PlayerStateManager.Instance.ActiveTown.DailyTaxes / 100.0f));
                    int moneyLost = Math.Min(unit.Inventory.Money, tax);
                    unit.Inventory.Money = unit.Inventory.Money - moneyLost;
                    ActivityResult result = new ActivityResult();
                    result.MoneyLost = moneyLost == 0 ? null : (int?)moneyLost;
                    this.ShowActivityResults(activity, result);
                    this.AnnounceTextToUI(this.unitDecisionUtils.GetStringForUnitTaxed(activity, moneyLost));
                    PlayerStateManager.Instance.PlayerInventory.Money += moneyLost;
                }
            }
        }

        /// <summary>
        /// Makes a unit finish up, and checks if all other units are done.
        /// </summary>
        /// <param name="activity"></param>
        private void ProcessUnitDoneForTurn(UnitDecisionActivity activity)
        {
            this.AnnounceTextToUI(this.unitDecisionUtils.GetDecisionStringForUnitDone(activity.Unit));
            activity.Decision = Decision.RestAtHome;
            activity.DoneForTurn = true;
            activity.NewDecision(Decision.Idle, 0);
            this.UpdateActivityOnUI(activity);

            if (!this.allUnitsDone)
            {
                bool unitsDone = true;
                foreach (UnitDecisionActivity activityToCheck in this.decisionActivities)
                {
                    if (!activityToCheck.DoneForTurn)
                    {
                        unitsDone = false;
                        break;
                    }
                }

                if (unitsDone)
                {
                    this.allUnitsDone = true;
                    UpdateCommandPaneText("Next Day");
                    this.AnnounceTextToUI("All units are done for the day.");
                }
            }
        }

        protected virtual void UpdateCommandPaneText(string text)
        {
            this.commandPane.ContinueButton.Text = text;
        }

        /// <summary>
        /// Prepares units for a new turn
        /// </summary>
        protected void RefreshUnitAndOwnerStatusForNewTurn()
        {
            this.decisionActivities.Clear();

            foreach (DecisionMakingUnit unit in this.DecisionMakingUnits)
            {
                unit.RefreshStatsForNewManagementModeTurn();
                UnitDecisionActivity activity = new UnitDecisionActivity(unit, Decision.Idle, this.GetIdleTime());
                this.decisionActivities.Add(activity);
                this.AddActivityToUIFeed(activity);
            }

            this.shopOwnerDecisionActivities.Clear();

            foreach (Building building in this.Buildings) 
            {
                if (building.IsBuildingWithOwner && building.IsAutonomousBuilding && building.Owner != null)
                {
                    building.Owner.RefreshStatsForNewManagementModeTurn();
                    UnitDecisionActivity activity = new UnitDecisionActivity(building.Owner, Decision.Idle, this.GetActivityDurationTime());
                    this.shopOwnerDecisionActivities.Add(activity);
                }
            }
        }

        protected virtual int GetActivityDurationTime()
        {
            return Utilities.GetRandomNumber(10, 20);
        }

        protected virtual int GetIdleTime()
        {
            return Utilities.GetRandomNumber(1, 3);
        }

        /// <summary>
        /// Updates status of active decisions and handles letting idle units make decisions 
        /// </summary>
        protected void ProcessUnitAndOwnerDecisions(GameTime gameTime)
        {
            List<UnitDecisionActivity> allActivities = new List<UnitDecisionActivity>(this.decisionActivities);
            allActivities.AddRange(this.shopOwnerDecisionActivities);

            foreach (UnitDecisionActivity activity in allActivities)
            {
                if (activity.DoneForTurn)
                {
                    continue;
                }

                activity.Update(gameTime, this.activityTickDuration);

                if (activity.Complete)
                {
                    if (activity.Decision == Decision.Idle)
                    {
                        // Done idling
                        if (activity.Unit.CanMakeDecision)
                        {
                            if (activity.Unit is Owner)
                            {
                                this.GenerateNewDecisionForOwner(activity);
                            }
                            else
                            {
                                this.GenerateNewDecisionForUnit(activity);
                            }
                        }
                        else
                        {
                            this.ProcessUnitDoneForTurn(activity);
                        }
                    }
                    else
                    {
                        // Done with some activity
                        ActivityResult results = this.unitDecisionResultEngine.GetActionResult(activity);
                                                                          
                        this.AnnounceTextToUI(this.unitDecisionUtils.GetDecisionStringForActionComplete(activity, results));
                        activity.Unit.PreviousDecision = activity.Decision;
                        this.ShowActivityResults(activity, results);
                        if (activity.Unit.CanMakeDecision && !results.UnitIsDone)
                        {                            
                            activity.NewDecision(Decision.Idle, this.GetIdleTime());
                            this.UpdateActivityOnUI(activity);
                        }
                        else
                        {
                            this.ProcessUnitDoneForTurn(activity);
                        }
                    }
                }
            }
        }
    }
}
