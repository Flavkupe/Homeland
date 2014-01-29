using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Buildings.Types;
using TacticsGame.GameObjects.Obstacles;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Units.Classes;
using TacticsGame.GameObjects.Zones;
using TacticsGame.Map;
using TacticsGame.PlayerThings;
using TacticsGame.Text;
using TacticsGame.UI;
using TacticsGame.Managers;
using TacticsGame.UI.Dialogs;
using TacticsGame.Simulation;

namespace TacticsGame.Scene
{
    [Serializable]
    public partial class ZoneManagementScene : ZoneScene, IDisposable
    {
        private enum State
        {       
            Idle,
            BuildingSelected,            
            MiscSelected,
            ShowingDialog,
            Fading,
            PlacingBuilding,
        }

        [NonSerialized]
        private State storedState = State.Idle;
        [NonSerialized]
        private State state = State.Idle;

        [NonSerialized]
        private bool pause = false;
        [NonSerialized]
        KeyboardState oldState;             

        [NonSerialized]
        private GuildHouse guildBuilding;        

        // Stuff for dragging buildings around        
        [NonSerialized]
        private IBuildable draggingBuilding = null;

        [NonSerialized]
        private List<Tile> tilesWeJustFiltered = new List<Tile>();
        [NonSerialized]
        private bool buildingCanBePlaced;

        [NonSerialized]
        private GameEntity selectedEntity = null;               

        private ManagementSimulation simulation = new ManagementSimulation();

        private delegate void PostDialogEventDelegate();
        private PostDialogEventDelegate postDialogEvent;

        public ZoneManagementScene()
            : base()
        {
            this.SetUIHandlers();

            this.openDialog = GameInitDialog.CreateDialog();
            this.CurrentState = State.ShowingDialog;
            this.postDialogEvent = this.InitializeScene;
        }

        protected List<DecisionMakingUnit> DecisionMakingUnits
        {
            get { return this.TownState.DecisionMakingUnits; }
            set { this.TownState.DecisionMakingUnits = value; }
        }

        public ManagementSimulation Simulation
        {
            get { return simulation; }
        }

        protected virtual void InitializeScene()
        {
            // TEMP
            Obstacle obst = new Obstacle("Rock");
            obst.SetLocationTo(Grid.GetTile(4, 4));
            this.Obstacles.Add(obst);

            Building bld = new Shop(Grid.VisibleTileDimensions);
            bld.SetLocationTo(Grid.GetTile(6, 6));
            this.Buildings.Add(bld);

            Building bld4 = new Blacksmith();
            bld4.SetLocationTo(Grid.GetTile(5, 10));
            this.Buildings.Add(bld4);

            Building bld3 = new Tavern();
            bld3.SetLocationTo(Grid.GetTile(15, 6));
            this.Buildings.Add(bld3);

            GuildHouse bld2 = new GuildHouse(Grid.VisibleTileDimensions);
            bld2.SetLocationTo(Grid.GetTile(12, 10));
            this.Buildings.Add(bld2);
            this.guildBuilding = bld2;

            Zone zone = new WoodlandZone(6,2);
            this.Zones.Add(zone);
            zone.SetTopLeftTile(this.Grid.GetTile(3, 0));

            Zone zone2 = new MountainZone(2, 6);
            this.Zones.Add(zone2);
            zone2.SetTopLeftTile(this.Grid.GetTile(0, 10));

            DecisionMakingUnit unit = new Ranger();
            unit.SetLocationTo(bld2.DoorTile.GetSouth(), false);
            this.DecisionMakingUnits.Add(unit);

            DecisionMakingUnit unit2 = new Footman();
            unit2.SetLocationTo(bld2.DoorTile.GetSouth(), false);
            this.DecisionMakingUnits.Add(unit2);

            DecisionMakingUnit unit3 = new Fool();
            unit3.SetLocationTo(bld2.DoorTile.GetSouth(), false);
            this.DecisionMakingUnits.Add(unit3);

            this.TownState.TownGuildhouse = this.guildBuilding;
            PlayerStateManager.Instance.ActiveTown = this.TownState;

            this.ResetTurn();
        }          

        private GameEntity SelectedEntity
        {
            get { return this.selectedEntity; }
            set
            {
                if (value == null)
                {
                    if (this.selectedEntity != null)
                    {
                        this.selectedEntity.Selected = false;
                        this.selectedEntity = null;
                    }
                }
                else
                {
                    if (this.selectedEntity != value && this.selectedEntity != null)
                    {
                        this.selectedEntity.Selected = false;
                    }

                    this.selectedEntity = value;
                    this.selectedEntity.Selected = true;
                }
            }
        }

        private State CurrentState
        {
            get { return this.state; }
            set
            {
                this.storedState = this.state;
                if (this.state != value)
                {
                    if (this.state == State.ShowingDialog)
                    {
                        this.HandleShowingDialogStateChanged();
                    }

                    if (value == State.BuildingSelected)
                    {
                        this.HandleStateChangedToBuildingSelected();
                    }
                    else if (value == State.MiscSelected)
                    {
                        this.HandleStateChangedToMiscSelection();
                    }
                    else if (value == State.Idle)
                    {
                        this.ShowIdleMenuButtons();
                    }                    
                }
                else
                {
                    if (value == State.BuildingSelected)
                    {
                        this.RefreshBuildingSelection();
                    }
                    else if (value == State.MiscSelected)
                    {
                        this.RefreshMiscSelection();
                    }
                    else if (value == State.Idle)
                    {
                        this.ShowIdleMenuButtons();
                    }
                }

                this.state = value;
            }
        }                  

        public override void LoadContent()
        {                      
            // Load heavy graphical and common-resource content
            base.LoadContent();
            this.Grid.LoadContent();
            this.DecisionMakingUnits.ForEach(a => a.LoadContent());            
            this.Units.ForEach(a => a.LoadContent());
            this.Obstacles.ForEach(a => a.LoadContent());
            this.Buildings.ForEach(a => a.LoadContent());

            // Set references of units to corresponding tiles
            this.DecisionMakingUnits.ForEach(a => a.SetLocationTo(this.Grid.GetTile(a.Coordinates.X, a.Coordinates.Y), false));
            this.Units.ForEach(a => a.SetLocationTo(this.Grid.GetTile(a.Coordinates.X, a.Coordinates.Y), false));            
            this.Obstacles.ForEach(a => a.SetLocationTo(this.Grid.GetTile(a.Coordinates.X, a.Coordinates.Y)));
            this.Buildings.ForEach(a => a.SetLocationTo(this.Grid.GetTile(a.Coordinates.X, a.Coordinates.Y)));

            // Create new lists for non-serialized lists and objects
            this.visualEffects = new List<AnimatedEffect>();
            this.floatingText = new List<FloatingText>();
            this.fade = new Fade();
            this.tilesWeJustFiltered = new List<Tile>();

            // Find the guild building
            foreach (Building building in this.Buildings)
            {
                if (building is GuildHouse)
                {                    
                    this.guildBuilding = building as GuildHouse;
                    break;
                }
            }

            this.TownState.TownGuildhouse = this.guildBuilding;
            PlayerStateManager.Instance.ActiveTown = this.TownState;

            // Set references of objects to the deserialized versions of those objects by comparing IDs
            this.Simulation.ManagementActivities.ForEach(a => a.LoadReferencesFromLists(this.DecisionMakingUnits, this.Buildings));
            this.Buildings.ForEach(a => a.LoadReferencesFromLists(this.DecisionMakingUnits, this.Buildings));

            // Make a list of owners
            List<DecisionMakingUnit> owners = new List<DecisionMakingUnit>();
            foreach (Building building in this.Buildings)
            {
                if (building.Owner != null) 
                {
                    owners.Add(building.Owner);
                }
            }           

            // TODO: i might have broken this...
            this.Simulation.ManagementActivities.Where(a => a.IsOwner).ToList().ForEach(a => a.LoadReferencesFromLists(owners, this.Buildings));

            // UI stuff 
            this.SetUIHandlers();
            this.ClearSelection();
            this.actionFeed.ClearContents();
            this.actionFeed.ClearUnitView();                        
            foreach (UnitManagementActivity activity in this.Simulation.ManagementActivities.Where(a => !a.IsOwner))
            {
                // Put the loaded activity items into the action feed
                this.AddActivityToUIFeed(activity);
            }            
        }        

        public override void QuickLoad()
        {
            if (this.CurrentState == State.Idle)
            {
                base.QuickLoad();
            }
        }

        public override void QuickSave()
        {
            if (this.CurrentState == State.Idle)
            {
                base.QuickSave();
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (this.CurrentState == State.Fading)
            {
                this.fade.Update(gameTime);

                if (this.fade.IsDone)
                {
                    this.CurrentState = State.Idle;
                    this.PrepareForNextDay();
                    return;
                }                               
            }

            // Most things don't update while fading, but the action feed does.
            if (this.CurrentState != State.Fading)
            {
                if (this.CurrentState == State.ShowingDialog)
                {
                    return;
                }

                // Check which keys and mouse buttons have been pressed     
                this.UpdateMouseInput();

                // Deals with key presses
                this.HandleMouseAndKeyboardInputs();

                this.HandleScrolling();

                if (this.CurrentState == State.PlacingBuilding)
                {
                    this.RefreshBuildSelectedBuildingState();
                }

                foreach (FloatingText text in this.floatingText.ToList<FloatingText>())
                {
                    text.Update(gameTime);
                    if (text.IsExpired)
                    {
                        this.floatingText.Remove(text);
                    }
                }

                if (pause)
                {
                    return;
                }

                // Everything that happens only when the game is unpaused goes below

                this.DaylightFilter.Update(gameTime);
                this.Simulation.UpdateWorldTime(gameTime);

                // Simulate all entities
                List<UnitManagementActivity> allActivities = new List<UnitManagementActivity>(this.Simulation.ManagementActivities);

                foreach (UnitManagementActivity activity in allActivities)
                {
                    UnitActivityUpdateStatus activityUpdate = this.Simulation.ProcessUnitActivity(activity, gameTime);
                    this.ProcessUnitActivityUpdate(activityUpdate);
                }

                if (this.Simulation.DayIsOver)
                {
                    this.UpdateCommandPaneText("Next Day");
                    this.AnnounceTextToUI("All units are done for the day.");
                }
            }

            this.actionFeed.Update(gameTime);
        }    

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.draggingBuilding != null)
            {                
                (this.draggingBuilding as GameEntity).Draw(gameTime);
            }

            foreach (UnitManagementActivity activity in this.Simulation.ManagementActivities)
            {
                if (activity.State == ActivityState.GoingToActivity || activity.State == ActivityState.ReturningFromActivity)
                {
                    activity.Unit.Draw(gameTime);
                    activity.Unit.Sprite.UpdateAnimation(gameTime);
                }
            }

            if (this.CurrentState == State.Fading) 
            {
                this.fade.Draw(gameTime);
            }

            this.DaylightFilter.Draw(gameTime);

            Utilities.DrawDebugText(this.state.ToString(), new Vector2(10, 60));

            Utilities.DrawDebugText("Zoom: " + GameStateManager.Instance.ZoomLevel, new Vector2(10, 80));

            Utilities.DrawDebugText(GameManager.World.WorldTime.ToShortTimeString(), new Vector2(10, 100));

            if (pause)
            {
                Utilities.DrawDebugText("Paused", new Vector2(10, 120));
            }
        }

        public override void  Dispose()
        {
 	        this.ClearUIHandlers();            
        }
    }
}
