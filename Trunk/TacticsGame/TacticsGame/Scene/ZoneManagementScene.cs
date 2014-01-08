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
        private const int baseActivityTickDuration = 1000;
        [NonSerialized]
        private const int hurryActivityTickDuration = 100;
        [NonSerialized]
        private int activityTickDuration = baseActivityTickDuration;

        [NonSerialized]
        private UnitDecisionEngine unitDecisionEngine = new UnitDecisionEngine();
        [NonSerialized]
        private UnitDecisionResultEngine unitDecisionResultEngine = new UnitDecisionResultEngine();
        [NonSerialized]
        private UnitDecisionUtils unitDecisionUtils = new UnitDecisionUtils();

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

        private bool allUnitsDone = false;

        protected List<UnitDecisionActivity> decisionActivities = new List<UnitDecisionActivity>();
        protected List<UnitDecisionActivity> shopOwnerDecisionActivities = new List<UnitDecisionActivity>();

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
            get { return this.townState.DecisionMakingUnits; }
            set { this.townState.DecisionMakingUnits = value; }
        }

        protected virtual void InitializeScene()
        {
            // TEMP
            Obstacle obst = new Obstacle("Rock");
            obst.SetLocationTo(grid.GetTile(4, 4));
            this.obstacles.Add(obst);

            Building bld = new Shop(grid.VisibleTileDimensions);
            bld.SetLocationTo(grid.GetTile(6, 6));
            this.Buildings.Add(bld);

            Building bld4 = new Blacksmith();
            bld4.SetLocationTo(grid.GetTile(5, 10));
            this.Buildings.Add(bld4);

            Building bld3 = new Tavern();
            bld3.SetLocationTo(grid.GetTile(15, 6));
            this.Buildings.Add(bld3);

            GuildHouse bld2 = new GuildHouse(grid.VisibleTileDimensions);
            bld2.SetLocationTo(grid.GetTile(12, 10));
            this.Buildings.Add(bld2);
            this.guildBuilding = bld2;

            Zone zone = new WoodlandZone(6,2);
            this.zones.Add(zone);
            zone.SetTopLeftTile(this.grid.GetTile(3, 0));

            Zone zone2 = new MountainZone(2, 6);
            this.zones.Add(zone2);
            zone2.SetTopLeftTile(this.grid.GetTile(0, 10));

            DecisionMakingUnit unit = new Ranger();
            this.DecisionMakingUnits.Add(unit);

            DecisionMakingUnit unit2 = new Footman();
            this.DecisionMakingUnits.Add(unit2);

            DecisionMakingUnit unit3 = new Fool();
            this.DecisionMakingUnits.Add(unit3);

            this.townState.TownGuildhouse = this.guildBuilding;
            PlayerStateManager.Instance.ActiveTown = this.townState;

            this.ResetTurn();
        }

        ////////////////////
        #region UI Overrides        

        /// <summary>
        /// Override if there is no action feed.
        /// </summary>
        protected virtual void ShowActivityResultsOnActionFeed(UnitDecisionActivity activity, ActivityResult results)
        {
            this.actionFeed.UpdateResultOnActivity(activity, results);
        }

        /// <summary>
        /// Override if no UI is present.
        /// </summary>
        protected virtual void UpdateActivityOnUI(UnitDecisionActivity activity)
        {
            this.actionFeed.UpdateActivity(activity);
        }

        /// <summary>
        /// Uses present UI to announce text.
        /// </summary>
        /// <param name="text"></param>
        protected virtual void AnnounceTextToUI(string text)
        {
            this.actionFeed.AddToFeed(text);
        }

        /// <summary>
        /// Puts the activity in the UI for updating
        /// </summary>
        /// <param name="activity"></param>
        protected virtual void AddActivityToUIFeed(UnitDecisionActivity activity)
        {
            this.actionFeed.AddToFeed(activity);
        }

        #endregion
        ////////////////////

        /// <summary>
        /// Clears all handlers for when we are ready to dispose of this scene.
        /// </summary>
        protected override void ClearUIHandlers()
        {
            this.commandPane.CommandButtonClicked -= this.HandleCommandPaneCommand;
            this.commandPane.ContinueButtonClicked -= this.HandleContinueButtonClicked;
            this.commandPane.BuildButtonClicked -= this.HandleBuildBuildingIconClicked;        
            InterfaceManager.Instance.DialogClosed -= this.HandleDialogClosed;
            this.actionFeed.ActivityIconClicked -= this.HandleUnitIconClicked;
        }

        /// <summary>
        /// Creates the UI handlers for this scene.
        /// </summary>
        protected override void SetUIHandlers()
        {
            this.commandPane.CommandButtonClicked += this.HandleCommandPaneCommand;
            this.commandPane.ContinueButtonClicked += this.HandleContinueButtonClicked;
            this.commandPane.BuildButtonClicked += this.HandleBuildBuildingIconClicked;
            InterfaceManager.Instance.DialogClosed += this.HandleDialogClosed;
            this.actionFeed.ActivityIconClicked += this.HandleUnitIconClicked;
        }

        void HandleCommandPaneCommand(object sender, CommandPaneCommandEventArgs e)
        {
            switch (e.Command)
            {
                case Commands.RequestsClicked:
                    this.HandleRequestsButtonClicked();
                    break;
                case Commands.CaravanClicked:
                    this.HandleCaravanButtonClicked();
                    break;
                case Commands.VisitorsClicked:
                    this.HandleVisitorsButtonClicked();
                    break;
                case Commands.ShowStockClicked:                    
                    this.HandleShowStockClicked();
                    break;
                case Commands.UnitsButtonClicked:
                    this.HandleUnitButtonClicked();
                    break;
                case Commands.SellClicked:
                    this.HandleSellButtonClicked();
                    break;
                case Commands.EdictsClicked:
                    this.HandleEdictsButtonClicked();
                    break;
                case Commands.FinancesClicked:
                    this.HandleFinanceButtonClicked();
                    break;
            }   
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

        /// <summary>
        /// Some tile in the grid was selected that was not previously selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void HandleClickedOnATile(object sender, TileGrid.TileClickedEventArgs e)
        {
            Tile selection = e.ClickedTile;

            if (this.CurrentState == State.PlacingBuilding)
            {
                this.PlaceBuildingOnTile(selection);
                return;
            }
            else
            {
                if (selection.TileResident != null)
                {
                    this.SelectedEntity = selection.TileResident;

                    if (selection.TileResident is Building)
                    {
                        this.CurrentState = State.BuildingSelected;
                    }
                    else
                    {
                        this.CurrentState = State.MiscSelected;
                    }
                }
                else
                {
                    this.ClearSelection();
                }
            }
        }                               

        public override void LoadContent()
        {                      
            // Load heavy graphical and common-resource content
            base.LoadContent();
            this.grid.LoadContent();
            this.DecisionMakingUnits.ForEach(a => a.LoadContent());            
            this.Units.ForEach(a => a.LoadContent());
            this.obstacles.ForEach(a => a.LoadContent());
            this.Buildings.ForEach(a => a.LoadContent());

            // Set references of units to corresponding tiles
            this.DecisionMakingUnits.ForEach(a => a.SetLocationTo(this.grid.GetTile(a.Coordinates.X, a.Coordinates.Y)));
            this.Units.ForEach(a => a.SetLocationTo(this.grid.GetTile(a.Coordinates.X, a.Coordinates.Y)));            
            this.obstacles.ForEach(a => a.SetLocationTo(this.grid.GetTile(a.Coordinates.X, a.Coordinates.Y)));
            this.Buildings.ForEach(a => a.SetLocationTo(this.grid.GetTile(a.Coordinates.X, a.Coordinates.Y)));

            // Create new lists for non-serialized lists and objects
            this.visualEffects = new List<AnimatedEffect>();
            this.floatingText = new List<FloatingText>();
            this.fade = new Fade();
            this.unitDecisionEngine = new UnitDecisionEngine();        
            this.unitDecisionResultEngine = new UnitDecisionResultEngine();        
            this.unitDecisionUtils = new UnitDecisionUtils();
            this.tilesWeJustFiltered = new List<Tile>();

            // Random initializations
            this.activityTickDuration = baseActivityTickDuration;

            // Find the guild building
            foreach (Building building in this.Buildings)
            {
                if (building is GuildHouse)
                {                    
                    this.guildBuilding = building as GuildHouse;
                    break;
                }
            }

            this.townState.TownGuildhouse = this.guildBuilding;
            PlayerStateManager.Instance.ActiveTown = this.townState;

            // Set references of objects to the deserialized versions of those objects by comparing IDs
            this.decisionActivities.ForEach(a => a.LoadReferencesFromLists(this.DecisionMakingUnits, this.Buildings));
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

            this.shopOwnerDecisionActivities.ForEach(a => a.LoadReferencesFromLists(owners, this.Buildings));

            // UI stuff 
            this.SetUIHandlers();
            this.ClearSelection();
            this.actionFeed.ClearContents();
            this.actionFeed.ClearUnitView();                        
            foreach (UnitDecisionActivity activity in this.decisionActivities)
            {
                // Put the loaded activity items into the action feed
                this.actionFeed.AddToFeed(activity);
            }            
        }

        protected override void HandleMouseAndKeyboardInputs()
        {
            this.HandleScrollInputs();

            if (this.commandPane.MouseIsOverControl())
            {
                // Let the UI deal with it
                return;
            }

            if (this.CurrentState == State.ShowingDialog)
            {
                // Don't interrupt dialog.
                return;
            }

            KeyboardState keystate = Keyboard.GetState();

            if (keystate.IsKeyUp(Keys.P) && this.oldState != null && this.oldState.IsKeyDown(Keys.P))
            {
                this.pause = !this.pause;
            }
            else if (keystate.IsKeyUp(Keys.Q) && this.oldState != null && this.oldState.IsKeyDown(Keys.Q))
            {
                this.QuickSave();
            }
            else if (keystate.IsKeyUp(Keys.L) && this.oldState != null && this.oldState.IsKeyDown(Keys.L))
            {
                this.QuickLoad();
            }
                                                               
            base.HandleMouseAndKeyboardInputs();      

            if (this.RMBReleased)
            {
                this.ClearSelection();
            }

            this.oldState = Keyboard.GetState();
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

                this.ProcessUnitAndOwnerDecisions(gameTime);
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

            //foreach (DecisionMakingUnit unit in this.decisionMakingUnits)
            //{
            //    unit.Draw(gameTime);
            //}

            if (this.CurrentState == State.Fading) 
            {
                this.fade.Draw(gameTime);
            }

            Utilities.DrawDebugText(this.state.ToString(), new Vector2(10, 60));

            if (pause)
            {
                Utilities.DrawDebugText("Paused", new Vector2(10, 80));
            }

            Utilities.DrawDebugText("Zoom: " + GameStateManager.Instance.ZoomLevel, new Vector2(10, 80));
        }

        public override void  Dispose()
        {
 	        this.ClearUIHandlers();            
        }
    }
}
