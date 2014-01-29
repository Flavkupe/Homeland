using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.Edicts;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Owners;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Zones;
using TacticsGame.Managers;
using TacticsGame.Map;
using TacticsGame.World;

namespace TacticsGame.Simulation
{
    public class ManagementSimulation
    {
        private UnitActionManager UnitActionManager { get { return GameManager.UnitActionManager; } }

        private List<UnitManagementActivity> managementActivities = new List<UnitManagementActivity>();

        private DailyActivityStats dailyActivityStats = new DailyActivityStats();      

        private TownState simulatedTown = null;

        public List<UnitManagementActivity> ManagementActivities
        {
            get { return this.managementActivities; }
            set { this.managementActivities = value; }
        }

        public DailyActivityStats DailyActivityStats
        {
            get { return dailyActivityStats; }
        }

        public bool DayIsOver { get { return GameManager.World.WorldTime.Hour >= GameWorld.DayEndHour; } }

        public TownState ActiveTown
        {
            get
            {
                if (this.simulatedTown == null)
                {
                    return GameManager.GameStateManager.World.CurrentTown;
                }

                return this.simulatedTown;
            }
        }

        protected List<DecisionMakingUnit> DecisionMakingUnits
        {
            get { return this.ActiveTown.DecisionMakingUnits; }
            set { this.ActiveTown.DecisionMakingUnits = value; }
        }

        /// <summary>
        /// Generates a new decision when needed
        /// </summary>
        /// <param name="activity">The decision activity object for the unit.</param>
        /// <param name="avoidShops">If true, unit should actively avoid shops</param>
        public void GenerateNewDecisionForUnit(UnitManagementActivity activity, UnitActivityUpdateStatus updateStatus)
        {
            Decision decision = this.UnitActionManager.DecisionEngine.MakeDecision(activity.Unit);
            int decisionDuration = this.UnitActionManager.DecisionEngine.DetermineDecisionDuration(activity.Unit, decision);
            activity.NewDecision(decision, decisionDuration);
            
            updateStatus.ShouldAnnounceActivityChange = true;            

            if (this.UnitActionManager.DecisionEngine.DecisionRequiresBuilding(decision))
            {
                activity.TargetBuilding = this.UnitActionManager.DecisionEngine.DetermineTargetBuilding(activity.Unit, decision, this.GetAllBuildings());

                if (activity.TargetBuilding == null)
                {
                    updateStatus.Announcements.Add(this.UnitActionManager.ResultTextManager.GetDecisionStringForUnableToFindProperBuilding(activity));
                    activity.Unit.PreviousDecision = decision;
                    activity.Unit.LastResult = new ActivityResult();
                    activity.Unit.LastResult.ActionSuccess = false;
                    this.GenerateNewDecisionForUnit(activity, updateStatus);
                    return;
                }
                else
                {
                    activity.TargetTile = activity.TargetBuilding.DoorTile.GetSouth();
                }
            }
            else if (this.UnitActionManager.DecisionEngine.DecisionRequiresExitZone(decision))
            {
                ExitZone zone = this.UnitActionManager.DecisionEngine.GetTargetZone(decision, this.ActiveTown.ExitZones);
                
                // TODO: no zone, or zone is blocked.
                Debug.Assert(zone != null, "Can't be null!");
                activity.TargetTile = zone.CurrentTiles.GetRandomItem(); 
            }

            this.InitiateTransition(activity);

            updateStatus.Announcements.Add(this.UnitActionManager.ResultTextManager.GetDecisionStringForActionStarted(activity));
        }

        private void InitiateTransition(UnitManagementActivity activity)
        {
            if (activity.IsOwner || activity.State == ActivityState.Idle)
            {
                return;
            }

            if (activity.State == ActivityState.PreparingToStartActivity || activity.State == ActivityState.PreparingToReturnFromActivity)
            {
                if (activity.TargetTile != null)
                {
                    activity.State = activity.State == ActivityState.PreparingToStartActivity ? ActivityState.GoingToActivity : ActivityState.ReturningFromActivity;
                    Debug.Assert(activity.Unit.CurrentTile != null);
                    List<Tile> path = this.ActiveTown.TileGrid.GetPathBetween(activity.Unit.CurrentTile, activity.TargetTile);
                    Debug.Assert(path != null);
                    activity.PathStack.Clear();
                    if (path == null)
                    {
                        activity.State = ActivityState.AwaitingNextActivity;
                    }
                    else
                    {
                        activity.PathStack.PushRangeReversed(path);
                    }                                         
                }
                else
                {
                    activity.State = activity.State == ActivityState.PreparingToStartActivity ? ActivityState.InActivity : ActivityState.AwaitingNextActivity;
                }
            }
        }

        /// <summary>
        /// Generates a new decision when needed
        /// </summary>
        /// <param name="activity">The decision activity object for the unit.</param>
        /// <param name="avoidShops">If true, unit should actively avoid shops</param>
        public void GenerateNewDecisionForOwner(UnitManagementActivity activity, UnitActivityUpdateStatus updateStatus)
        {    
            // TODO: do not use Owner to make decisions
            Debug.Assert(activity.Unit is Owner);
            Decision decision = this.UnitActionManager.DecisionEngine.MakeOwnerDecision(activity.Unit as Owner);
            activity.NewDecision(decision, 1);
            if (this.UnitActionManager.DecisionEngine.DecisionRequiresVisitor(decision))
            {
                DecisionMakingUnit visitor = this.UnitActionManager.DecisionEngine.DetermineTargetVisitor(activity.Unit, decision, (activity.Unit as Owner).OwnedBuilding.Visitors);
                activity.TargetVisitor = visitor;
                if (visitor == null)
                {
                    // Did not find a visitor... for now, assume end of turn                    
                    activity.NewDecision(Decision.Idle, 1);
                    activity.DoneForTurn = true;
                }
                else
                {
                    updateStatus.Announcements.Add(this.UnitActionManager.ResultTextManager.GetDecisionStringForActionStarted(activity));
                }
            }
        }

        /// <summary>
        /// Makes a unit finish up, and checks if all other units are done.
        /// </summary>
        /// <param name="activity"></param>
        private void ProcessUnitDoneForTurn(UnitManagementActivity activity, UnitActivityUpdateStatus status)
        {
            string announcement = this.UnitActionManager.ResultTextManager.GetDecisionStringForUnitDone(activity.Unit);
            status.Announcements.Add(announcement);

            activity.Decision = Decision.RestAtHome;
            activity.DoneForTurn = true;
            activity.NewDecision(Decision.Idle, 0);                       
        }


        /// <summary>
        /// Updates status of active decisions and handles letting idle units make decisions 
        /// </summary>
        public UnitActivityUpdateStatus ProcessUnitActivity(UnitManagementActivity activity, GameTime gameTime)
        {
            UnitActivityUpdateStatus activityStatus = new UnitActivityUpdateStatus();
            activityStatus.Activity = activity;

            activity.Update(gameTime);

            if (activity.State == ActivityState.Idle)
            {
                // Nothing
            }
            else if (activity.State == ActivityState.DoneWithActivity)
            {
                // Done with some activity
                ActivityResult results = this.UnitActionManager.ResultEngine.GetActionResult(activity);

                activityStatus.Results = results;

                string announcement = this.UnitActionManager.ResultTextManager.GetDecisionStringForActionComplete(activity, results);
                if (announcement != null)
                {
                    activityStatus.Announcements.Add(announcement);
                }
                    
                activity.Unit.PreviousDecision = activity.Decision;
                activity.NewDecision(Decision.Idle, this.GetIdleTime());

                activity.Unit.CurrentStats.ActionPoints -= results.ActionPointCost;
                Debug.Assert(activity.Unit.CurrentStats.ActionPoints >= 0);

                activityStatus.ShouldAnnounceActivityChange = true;
            }
            else if (activity.State == ActivityState.AwaitingNextActivity)
            {                
                // Done idling                
                if (activity.Unit is Owner)
                {
                    this.GenerateNewDecisionForOwner(activity, activityStatus);
                }
                else
                {
                    this.GenerateNewDecisionForUnit(activity, activityStatus);
                }                            
            }
            else if (activity.State == ActivityState.PreparingToReturnFromActivity)
            {
                // TEMP                
                activity.TargetTile = this.ActiveTown.TownGuildhouse.DoorTile.GetSouth();
                this.InitiateTransition(activity);
            }

            this.DailyActivityStats.Update(activityStatus);
            return activityStatus;
        }

        /// <summary>
        /// Prepares units for a new turn
        /// </summary>
        public void RefreshUnitAndOwnerStatusForNewTurn()
        {
            this.managementActivities.Clear();
            this.dailyActivityStats = new DailyActivityStats();            

            foreach (DecisionMakingUnit unit in this.DecisionMakingUnits)
            {
                unit.RefreshStatsForNewManagementModeTurn();
                UnitManagementActivity activity = new UnitManagementActivity(unit, Decision.Idle, this.GetIdleTime());
                this.managementActivities.Add(activity);
            }           

            foreach (Building building in this.ActiveTown.Buildings)
            {
                if (building.IsBuildingWithOwner && building.IsAutonomousBuilding && building.Owner != null)
                {
                    building.Owner.RefreshStatsForNewManagementModeTurn();
                    UnitManagementActivity activity = new UnitManagementActivity(building.Owner, Decision.Idle, this.GetActivityDurationTime());
                    activity.IsOwner = true;
                    this.managementActivities.Add(activity);
                }
            }
        }

        private List<Building> GetAllBuildings()
        {
            return this.ActiveTown.Buildings;
        }

        protected virtual int GetActivityDurationTime()
        {
            return Utilities.GetRandomNumber(10, 20);
        }

        protected virtual int GetIdleTime()
        {
            return Utilities.GetRandomNumber(1, 3);
        }

        public UnitActivityUpdateStatus TaxUnit(UnitManagementActivity activity)
        {
            UnitActivityUpdateStatus updateStatus = new UnitActivityUpdateStatus(activity);           

            // Tax
            if (this.ActiveTown.DailyTaxes > 0)
            {
                DecisionMakingUnit unit = activity.Unit;
                int tax = (int)((float)unit.Inventory.Money * ((float)this.ActiveTown.DailyTaxes / 100.0f));
                int moneyLost = Math.Min(unit.Inventory.Money, tax);
                unit.Inventory.Money = unit.Inventory.Money - moneyLost;
                ActivityResult result = new ActivityResult();
                result.MoneyLost = moneyLost == 0 ? null : (int?)moneyLost;              
                updateStatus.Results = result;
                updateStatus.ChangeInPlayerMoney = moneyLost;
                updateStatus.Announcements.Add(this.UnitActionManager.ResultTextManager.GetStringForUnitTaxed(activity, moneyLost));
                
                this.DailyActivityStats.DailyTaxesCollected += moneyLost;
            }

            return updateStatus;
        }

        /// <summary>
        /// Taxes each visitor.
        /// </summary>
        /// <param name="building"></param>
        public UnitActivityUpdateStatus TaxVisitors(Building building)
        {
            UnitActivityUpdateStatus activityUpdate = new UnitActivityUpdateStatus();
            int taxes = this.ActiveTown.VisitorTaxes;
            bool mercantilismEnabled = this.ActiveTown.EdictIsActive(EdictType.Mercantilism);

            if (taxes > 0)
            {
                int totalTaxes = 0;
                foreach (DecisionMakingUnit visitor in building.Visitors)
                {
                    if (mercantilismEnabled && visitor.IsTrader)
                    {
                        // Under mercantilism, traders are tax exempt. 
                        continue;
                    }

                    visitor.Inventory.Money -= taxes;                    
                    totalTaxes += taxes;                    
                    activityUpdate.Announcements.Add(this.UnitActionManager.ResultTextManager.GetStringForUnitTaxed(visitor, taxes));                                       
                }

                activityUpdate.ChangeInPlayerMoney = totalTaxes;
                this.DailyActivityStats.VisitorTaxesCollected += totalTaxes;
            }

            return activityUpdate;
        }

        public void ResetWorldTimeForNewTurn()
        {
            GameManager.World.WorldTime.Date.AddDays(1);
            GameManager.World.WorldTime.Date.Add(new TimeSpan(GameWorld.DayStartHour, 0, 0));
        }

        public void UpdateWorldTime(GameTime gameTime)
        {            
            // Each second is 5 minutes of game time. Fast speed accelerates that by 5x.
            DateTime time = GameManager.World.WorldTime;            
            int gameMilliseconds = gameTime.ElapsedGameTime.Milliseconds * Utilities.GetSpeedMultiplier(GameManager.GameStateManager.GameSpeed) * 300;
            time = time.AddMilliseconds(gameMilliseconds);
            if (time.Hour > GameWorld.DayEndHour)
            {
                // Time stops at 22
                time = time.Date.Add(new TimeSpan(GameWorld.DayEndHour, 0, 0));
            }

            GameManager.World.WorldTime = time;
        }
    }
}
