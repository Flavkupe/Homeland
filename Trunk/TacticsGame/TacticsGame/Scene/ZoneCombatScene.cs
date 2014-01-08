using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using TacticsGame.Abilities;
using TacticsGame.AI;
using TacticsGame.GameObjects;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Units.Classes;
using TacticsGame.GameObjects.Units.Enemy.Types;
using TacticsGame.Map;
using TacticsGame.Text;
using TacticsGame.UI;
using TacticsGame.Utility;
using Wintellect.PowerCollections;
using TacticsGame.UI.Groups;
using TacticsGame.AI.CombatMode;

namespace TacticsGame.Scene
{
    public partial class ZoneCombatScene : ZoneScene
    {
        private enum State
        {
            Idle,
            UnitSelected,
            UnitDestinationSpecified,
            UnitMoving,
            AwaitingAbilityTargetSelection,
            AwaitingAbilityAnimation,
            EnemyTurnReadyForNewMove,
            EnemyUnitMoving,
            EnemyUnitReadyToAttack,
            ShowingDialog,
            Fading,
        }


        private List<Unit> readyList = new List<Unit>();
        private OrderedSet<Unit> waitingList = new OrderedSet<Unit>(new Comparers.UnitReadinessComparer());
        private List<Unit> doneList = new List<Unit>();

        private State state = State.Idle;
        private Random randomNum = new Random();
        private List<Unit> enemyUnitsToMove = new List<Unit>();
        private CombatAIDecisionEngine enemyDecisionEngine = new SimpleEnemyDecisionEngine();
        private int pauseUnitActions = 0;
        private Unit activeEnemy = null;
        private List<Unit> deadEnemies = new List<Unit>();
        private AbilityInfo activeAbility = null;

        private State storedState;

        protected TileSelectionDialog tileSelectionDialog;               
        protected Tile targetTile = null;
        private GameEntity selectedEntity = null;

        protected GameEntity SelectedEntity
        {
            get { return selectedEntity; }
            set 
            {
                if (this.selectedEntity != value)
                {
                    if (this.selectedEntity != null)
                    {
                        this.selectedEntity.Selected = false;
                    }

                    this.selectedEntity = value;

                    if (this.selectedEntity != null)
                    {
                        this.selectedEntity.Selected = true;
                    }
                }                
            }
        }

        TextureInfo footsteps;
        TextureInfo X;
        TextureInfo cursorTexture;
        TextureInfo enemyCursorTexture;

        public ZoneCombatScene()
            : base()
        {
            this.LoadContent();
            this.SetUIHandlers();

            //TEMP
            //Unit unit1 = new Ranger();
            //unit1.SetLocationTo(this.grid.GetTile(5, 5));
            //this.Units.Add(unit1);

            //Unit unit2 = new Fool();
            //unit2.SetLocationTo(this.grid.GetTile(10, 10));
            //this.Units.Add(unit2);

            //Unit unit4 = new Footman();
            //unit4.SetLocationTo(this.grid.GetTile(10, 5));
            //this.Units.Add(unit4);

            //Unit unit5 = new Footman();
            //unit5.SetLocationTo(this.grid.GetTile(10, 15));
            //this.Units.Add(unit5);

            //Unit unit6 = new Footman();
            //unit6.SetLocationTo(this.grid.GetTile(15, 15));
            //this.Units.Add(unit6);

            //Unit unit3 = new Bandit();
            //unit3.SetLocationTo(this.grid.GetTile(2, 10));
            //this.Units.Add(unit3);

            //Unit unit7 = new Wolf();
            //unit7.SetLocationTo(this.grid.GetTile(2, 13));
            //this.Units.Add(unit7);

            //// TEMP
            //this.AdvanceToNextTurn();
        }

        /// <summary>
        /// The current state.
        /// </summary>
        private State CurrentState
        {
            get { return state; }

            set
            {                
                if (state != value)
                {
                    this.StateChanged(state, value);
                }
                else
                {
                    this.RefreshState(value);
                }

                state = value;
            }
        }

        /// <summary>
        /// Called when the state is set to the same value as it was before.
        /// </summary>
        private void RefreshState(State state)
        {
            if (state == State.UnitSelected)
            {
                this.RefreshSelectedUnit();
            }
        }

        /// <summary>
        /// Called when the state changes from one state to another
        /// </summary>
        /// <param name="newState"></param>
        private void StateChanged(State oldState, State newState)
        {
            if (newState == State.UnitSelected)
            {
                this.StateChangedToUnitSelected();
            }
            else if (newState == State.AwaitingAbilityTargetSelection)
            {
                this.StateChangedToAwaitingAbilityTargetSelection();
            }
            else if (newState == State.AwaitingAbilityAnimation)
            {
                this.StateChangedToAwaitingAbilityAnimation();
            }
            else if (newState == State.EnemyTurnReadyForNewMove)
            {
                this.StateChangedToEnemyTurnReadyForNewMove();
            }
            else if (newState == State.EnemyUnitReadyToAttack)
            {
                this.StateChangedToEnemyTurnReadyToAttack();
            }
        }        

        /// <summary>
        /// Clears all handlers for when we are ready to dispose of this scene.
        /// </summary>
        protected override void ClearUIHandlers()
        {
            this.commandPane.AbilityClicked -= this.HandleAbilityClicked;
            this.commandPane.ContinueButtonClicked -= this.HandleContinueClicked;            
            InterfaceManager.Instance.DialogClosed -= this.HandleDialogClosed;
            this.actionFeed.UnitIconClicked -= this.HandleActionFeedUnitIconClicked;
        }

        /// <summary>
        /// Creates the UI handlers for this scene.
        /// </summary>
        protected override void SetUIHandlers()
        {
            this.commandPane.AbilityClicked += this.HandleAbilityClicked;
            this.commandPane.ContinueButtonClicked += this.HandleContinueClicked;            
            InterfaceManager.Instance.DialogClosed += this.HandleDialogClosed;
            this.actionFeed.UnitIconClicked += this.HandleActionFeedUnitIconClicked;
        }            

        /// <summary>
        /// Do the updating for specific mouse inputs.
        /// </summary>
        protected override void HandleMouseAndKeyboardInputs()
        {           
            if (this.LMBReleased || this.RMBReleased)
            {
                if (this.commandPane.MouseIsOverControl() || this.CurrentState == State.ShowingDialog)
                {
                    // Let the UI deal with it
                    return;
                }                

                if (this.CurrentState == State.UnitMoving)
                {
                    this.SelectedEntity.AccelerateTransitionSpeed();
                    return;
                }
                else if (this.CurrentState == State.EnemyUnitMoving)
                {
                    this.activeEnemy.AccelerateTransitionSpeed();
                    return;
                }
                else if (this.CurrentState == State.EnemyUnitReadyToAttack || this.CurrentState == State.EnemyTurnReadyForNewMove)
                {
                    return;
                }
            }

            if (this.RMBReleased)
            {
                this.ClearSelection();
            }

            // Handles other stuff common to all ZoneScenes like grid clicks.
            this.HandleScrollInputs();
            base.HandleMouseAndKeyboardInputs();
        }

        /// <summary>
        /// Loads the textures and stuff.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            this.fade = new Fade();
            footsteps = TextureManager.Instance.GetTextureInfo("Footprints", ResourceType.MiscObject);
            X = TextureManager.Instance.GetTextureInfo("X", ResourceType.MiscObject);
            cursorTexture = TextureManager.Instance.GetTextureInfo("Cursor", ResourceType.MiscObject);
            enemyCursorTexture = TextureManager.Instance.GetTextureInfo("EnemyCursor", ResourceType.MiscObject);
        }

        /// <summary>
        /// Some tile in the grid was selected that was not previously selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void HandleClickedOnATile(object sender, TileGrid.TileClickedEventArgs e)
        {
            Tile tile = e.ClickedTile;
            bool selectionChanged = e.SelectionChanged;

            this.TileSelected(tile, selectionChanged);
        }

        private void TileSelected(Tile tile, bool selectionChanged)
        {
            Debug.Assert(this.CurrentState != State.UnitMoving, "Should not be able to change selected tiles now!");

            //this.DisplayTileInfoInUI(tile);

            if (this.CurrentState == State.AwaitingAbilityTargetSelection)
            {
                if (grid.TileIsInTargetRange(tile) && AbilityDriver.TileIsValidTarget(this.activeAbility, tile))
                {
                    this.UseAbilityOnTarget(this.activeAbility, this.SelectedEntity as Unit, tile);                    
                }
            }
            else if ((this.state == State.UnitSelected || this.state == State.UnitDestinationSpecified) && this.SelectedEntity.PlayerCanCommand)
            {
                // Clicking on any tile while a unit you can command is selected.
                if (tile != null && tile.TileResident is Unit && ((Unit)tile.TileResident).IsHostile)
                {
                    // If clicking on an enemy unit, try to attack
                    if (this.AttackIsPossible(tile.TileResident as Unit))
                    {
                        Debug.Assert(this.SelectedEntity is Unit);
                        this.AttackUnit(this.SelectedEntity as Unit, tile.TileResident as Unit);
                        this.UnitPerformedAnAction(this.SelectedEntity as Unit, UnitActionType.Attack);
                        return;
                    }
                }

                // Unit is selected or a unitdestination has been specified for a commandable unit
                if (selectionChanged)
                {
                    // If the selected unit is not an enemy unit...
                    if (grid.TileIsInMovementRange(tile))
                    {
                        // Clicked on an empty tile within movement range; draw movement path to that tile and change state
                        this.targetTile = tile;
                        this.movementTiles = grid.GetPathBetween(this.SelectedEntity.CurrentTile, this.targetTile);
                        this.CurrentState = State.UnitDestinationSpecified;
                    }
                    else if (tile.TileResident == null)
                    {
                        // Clicked outside of the visible movement range; clear the selection
                        this.ClearSelection();
                    }
                    else if (tile.TileResident != null)
                    {
                        // Clicked on a new entity; change selection
                        this.SelectedEntity = tile.TileResident;
                        this.CurrentState = State.UnitSelected;
                    }

                }
                else if (this.movementTiles != null && this.movementTiles.Count >= 1)
                {
                    // Clicked on a destination for the second time. Initiate movement.
                    this.CurrentState = State.UnitMoving;                    
                }
            }
            else
            {
                this.targetTile = null;

                if (tile.TileResident == null)
                {
                    // Clicked on blank tile: clear selection.
                    this.ClearSelection();
                }
                else
                {
                    // Selected a new entity. Display that entity info, change state to entity being selected.
                    this.SelectedEntity = tile.TileResident;
                    this.commandPane.SetSelectedEntityDisplay(tile.TileResident);
                    this.CurrentState = State.UnitSelected;
                }
            }
        }

        /// <summary>
        /// Called when the unit performs an action like attacking or moving. This determines whether the unit can move right away and 
        /// puts the unit in the proper lists. Returns true if turn is over, false otherwise.
        /// </summary>
        private bool UnitPerformedAnAction(Unit unit, UnitActionType actionType, AbilityInfo ability = null)
        {
            try
            {
                if (actionType == UnitActionType.Ability)
                {
                    if (ability.Stats.HasProperty(AbilityProperty.CanActAfter) && unit.CurrentStats.ActionPoints > 0)
                    {
                        // Unit can still act
                        return false;
                    }
                }

                // Handle the "FreeMove" buff.
                if (unit.StatusEffects.HasStatus(UnitStatusEffect.FreeMove))
                {
                    // This buff will fade with any action that doesn't allow "CanActAfter", but will not use up turn if action was move.
                    unit.StatusEffects.RemoveStatus(UnitStatusEffect.FreeMove);
                    if (actionType == UnitActionType.Move)
                    {
                        return false;
                    }
                }

                if (unit.StatusEffects.HasStatus(UnitStatusEffect.ActAgain))
                {
                    unit.StatusEffects.RemoveStatus(UnitStatusEffect.ActAgain);

                    if (unit.CurrentStats.ActionPoints > 0)
                    {
                        return false;
                    }
                }

                this.readyList.Remove(unit);

                if (unit.CurrentStats.ActionPoints == 0 || actionType == UnitActionType.EndTurn)
                {
                    this.PutUnitInDoneList(unit);
                }
                else
                {
                    this.PutUnitInWaitingList(unit);
                }

                this.PullFromWaitingList();
                return true;
            }
            finally
            {
                this.RefreshUIState();
            }
        }

        /// <summary>
        /// Pulls a new unit from the waiting list based on priority. If there are no such units, it will call the method to proceed to the next 
        /// round.       
        /// </summary>
        /// <param name="forcePull">If true, will pull a hostile from the list even if ready queue still has units.</param>
        private void PullFromWaitingList(bool forcePull = false)
        {
            if (this.waitingList.Count == 0)
            {
                if (forcePull || this.readyList.Count == 0)
                {
                    this.AdvanceToNextTurn();
                }
                else
                {
                    this.SelectFirstFromReadyList();
                }
            }
            else
            {
                Unit first = this.waitingList.GetFirst();
                if (first.IsHostile)
                {                    
                    if (this.readyList.Count == 0 || forcePull)
                    {
                        // If top is hostile, it moves, provided the ready list contains no player units.
                        this.UnitIsReady(first);
                    }
                    else
                    {
                        // Still units left to move... select one of them and switch states
                        this.SelectFirstFromReadyList();
                    }
                }
                else
                {
                    while (this.waitingList.Count > 0)
                    {
                        Unit readiest = this.waitingList.GetFirst();
                        if (readiest.IsHostile)
                        {
                            break;
                        }

                        this.UnitIsReady(readiest);
                    }
                }          
            }

            this.RefreshUIState();
        }

        private void SelectFirstFromReadyList()
        {
            Unit selected = this.readyList[0];
            this.SelectedEntity = selected;
            this.PanCameraTo(selected.DrawPosition.X, selected.DrawPosition.Y);
            this.CurrentState = State.UnitSelected;
        }

        /// <summary>
        /// Sets the UI according to the current state of things.
        /// </summary>
        private void RefreshUIState()
        {
            this.SetCommandPaneButtonState();
            this.actionFeed.ClearUnitView();
            foreach (Unit unit in this.readyList)
            {
                UnitState state = UnitState.Active;    
                this.actionFeed.AddToFeed(new UnitCombatActivity(unit, state));
            }

            foreach (Unit unit in this.waitingList)
            {
                UnitState state = UnitState.Wait;
                this.actionFeed.AddToFeed(new UnitCombatActivity(unit, state));
            }

            foreach (Unit unit in this.doneList)
            {
                UnitState state = UnitState.Done;
                this.actionFeed.AddToFeed(new UnitCombatActivity(unit, state));
            }
        }
       
        /// <summary>
        /// The update loop for the game!
        /// </summary>
        public override void Update(GameTime gameTime)
        {            
            if (this.CurrentState == State.Fading)
            {
                this.fade.Update(gameTime);

                if (this.fade.IsDone)
                {
                    this.CurrentState = State.Idle;
                    this.TransitionFromCombatToManagement();
                    return;
                }
            }

            if (this.CurrentState == State.ShowingDialog)
            {
                return;
            }

            // Update position of text and remove when necessary
            for (int i = this.floatingText.Count - 1; i >= 0; --i)
            {
                FloatingText text = this.floatingText[i];
                text.Update(gameTime);
                if (text.IsExpired)
                {
                    this.floatingText.RemoveAt(i);
                }
            }

            // Animate effects and remove when necessary
            for (int i = this.visualEffects.Count - 1; i >= 0; --i)
            {
                AnimatedEffect effect = this.visualEffects[i];
                effect.Update(gameTime);
                effect.UpdateAnimation(gameTime);

                if (effect.IsExpired)
                {
                    this.visualEffects.RemoveAt(i);
                }
            }

            if (this.IsPanning)
            {
                this.HandleCameraPanning();

                if (this.PauseForPanning)
                {
                    return;
                }
            }

            // Check which keys and mouse buttons have been pressed     
            this.UpdateMouseInput();

            // Deals with key presses
            this.HandleMouseAndKeyboardInputs();

            this.HandleScrolling();

            // Update all units
            foreach (Unit unit in this.Units)
            {
                unit.Update(gameTime);
            }

            // Pauses actions for a second to pace the scene. Below this should only go things like making movement decisions... all animations go before this.
            if (this.pauseUnitActions > 0)
            {
                this.pauseUnitActions -= gameTime.ElapsedGameTime.Milliseconds;
                return;
            }

            if (this.CurrentState == State.AwaitingAbilityAnimation)
            {
                if (this.visualEffects.Count == 0)
                {
                    this.AbilityReachedTarget(this.activeAbility);                   
                }
            }

            if (this.CurrentState == State.UnitMoving || this.CurrentState == State.UnitSelected || this.CurrentState == State.UnitDestinationSpecified || this.CurrentState == State.AwaitingAbilityTargetSelection)
            {
                // Animate unit
                if (this.SelectedEntity is Unit)
                {
                    ((Unit)this.SelectedEntity).UpdateAnimation(gameTime);
                }
            }

            // Deal with state-specific behavior like walking
            if (this.CurrentState == State.EnemyTurnReadyForNewMove)
            {
                this.EnemyTurnReadyForNewMove();
            }            
            else if (this.CurrentState == State.UnitMoving)
            {                
                this.HandleUnitTransitioning(this.SelectedEntity);
            }
            else if (this.CurrentState == State.EnemyUnitMoving)
            {
                // Animate enemy
                this.activeEnemy.UpdateAnimation(gameTime);

                this.HandleUnitTransitioning(this.activeEnemy);
            }
            else if (this.CurrentState == State.EnemyUnitReadyToAttack)
            {
                this.EnemyTurnReadyToAttack();
            }
            else if (this.CurrentState == State.AwaitingAbilityTargetSelection)
            {
                Debug.Assert(this.activeAbility != null);                
            }
        }

        /// <summary>
        /// Draws all the things for this scene.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Draw footprints
            if (this.CurrentState == State.UnitDestinationSpecified)
            {
                if (this.movementTiles != null && this.movementTiles.Count >= 1)
                {                    
                    foreach (Tile tile in this.movementTiles.GetRange(0, this.movementTiles.Count - 1))
                    {
                        Utilities.DrawTexture2D(footsteps.Texture, tile.AreaRectangle);
                    }

                    Utilities.DrawTexture2D(X.Texture, this.targetTile.AreaRectangle);
                }
                else
                {
                    // TODO
                }
            }
            
            if (this.CurrentState == State.UnitSelected || this.CurrentState == State.UnitDestinationSpecified || this.CurrentState == State.AwaitingAbilityTargetSelection)
            {
                if (this.SelectedEntity != null && !(this.SelectedEntity is IMultiTile))
                {                    
                    // Draw cursor                    
                    Utilities.DrawTexture2D(this.cursorTexture.Texture, this.SelectedEntity.CurrentTile.AreaRectangle);
                }
            }
            
            if (this.CurrentState == State.EnemyUnitMoving || this.CurrentState == State.EnemyUnitReadyToAttack)
            {
                if (this.activeEnemy != null)
                {
                    // Draw enemy cursor                    
                    Utilities.DrawTexture2D(this.enemyCursorTexture.Texture, this.activeEnemy.DrawPosition);
                }
            }

            // Draw fade
            if (this.CurrentState == State.Fading)
            {
                this.fade.Draw(gameTime);
            }

            Utilities.DrawDebugText(this.state.ToString(), new Vector2(10, 60));
        }

        private enum UnitActionType
        {
            Attack,
            Move,
            Ability,
            Wait,
            EndTurn,
        }
    }
}
