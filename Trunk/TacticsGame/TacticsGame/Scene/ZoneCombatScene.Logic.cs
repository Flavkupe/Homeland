using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Obstacles;
using System.Diagnostics;
using TacticsGame.Map;
using TacticsGame.Abilities;
using TacticsGame.UI;
using Nuclex.UserInterface;
using TacticsGame.GameObjects.Effects;
using TacticsGame.Text;
using TacticsGame.GameObjects;
using TacticsGame.Utility;
using TacticsGame.GameObjects.Zones;
using TacticsGame.UI.Dialogs;
using TacticsGame.Items;
using TacticsGame.GameObjects.Units.Enemy.Types;
using Microsoft.Xna.Framework;
using TacticsGame.UI.Groups;
using Wintellect.PowerCollections;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Managers;
using TacticsGame.Items.SpecialStats;

namespace TacticsGame.Scene
{
    public partial class ZoneCombatScene
    {        
        private void StateChangedToAwaitingAbilityAnimation()
        {
            this.Grid.ClearTargetRadius();
        }

        private void StateChangedToEnemyTurnReadyToAttack()
        {
            this.ClearAllGridFilters();
        }

        private void StateChangedToEnemyTurnReadyForNewMove()
        {
            this.ClearAllGridFilters();            
        }

        /// <summary>
        /// The state has newly changed to selecting a tile to target.
        /// </summary>
        private void StateChangedToAwaitingAbilityTargetSelection()
        {
            Debug.Assert(this.SelectedEntity is Unit);
            this.Grid.ClearMovementRadius();
            this.Grid.SetDisplayedAbilityRadius(this.SelectedEntity as Unit, this.activeAbility);
        }

        /// <summary>
        /// Performs the ability
        /// </summary>
        private void ProcessAbility(AbilityInfo ability)
        {
            AbilitySceneResults results = null;
            AbilityType abilityType = ability.Stats.Type;

            if (ability.IsSelfAbility)
            {
                Unit source = this.SelectedEntity as Unit;
                ability.Cooldown = ability.Stats.Cooldown;

                // TODO: initialize ability here... handle the effect after initialization animation is done
                results = AbilityDriver.HandleAbilityEffects(ability, null);                                               
                
                if (source != null)
                {
                    if (!this.UnitPerformedAnAction(source, UnitActionType.Ability, ability))
                    {
                        this.CurrentState = State.UnitSelected;
                    }
                }

                this.RefreshSelectedUnit();
            }
            else if (ability.IsTargetAbility)
            {
                this.activeAbility = ability;
                this.CurrentState = State.AwaitingAbilityTargetSelection;
                return;
            }

            this.HandleAbilityResults(results);
        }

        private void ClearAllGridFilters()
        {
            this.Grid.ClearMovementRadius();
            this.Grid.ClearTargetRadius();
        }

        /// <summary>
        /// Initiates the EnemyTurn state.
        /// </summary>
        private void EnemyTurnStart()
        {
            this.enemyUnitsToMove.Clear();

            foreach (Unit unit in this.Units)
            {
                if (unit.IsHostile)
                {
                    unit.PrepareForNextCombatTurn();
                    this.enemyUnitsToMove.Add(unit);
                }
            }

            if (this.enemyUnitsToMove.Count == 0)
            {
                this.CombatIsOver();
            }
            else
            {
                this.CurrentState = State.EnemyTurnReadyForNewMove;
            }
        }

        /// <summary>
        /// After defeating all enemies... deal with loot and all that mumbo-jumbo
        /// </summary>
        private void CombatIsOver()
        {
            //this.ShowMessageBox("We won combat! Yay!");

            this.Units.Remove(PlayerStateManager.Instance.Player);

            // Process Loot
            List<Item> loot = new List<Item>(); 
            IGenerateLoot lootEngine = new LootGenerationEngine();
            foreach (Unit unit in this.deadEnemies)
            {
                if (unit is IDropLoot) 
                {
                    loot.AddRange(lootEngine.GetCombatLoot(unit as IDropLoot));
                }
            }

            if (loot.Count > 0)
            {
                List<DecisionMakingUnit> playerUnits = new List<DecisionMakingUnit>(this.Units.OfType<DecisionMakingUnit>());

                this.storedState = State.Fading;
                this.CurrentState = State.ShowingDialog;
                LootDialog dialog = LootDialog.CreateDialog();
                dialog.SetLoot(playerUnits, loot);
                this.openDialog = dialog;                
            }
            else
            {
                this.CurrentState = State.Fading;
            }
        }

        /// <summary>
        /// Advances combat to the player's turn.
        /// </summary>
        private void AdvanceToNextTurn()
        {
            IconInfo clock = TextureManager.Instance.GetIconInfo("Zs");
            foreach (Unit unit in this.readyList)
            {
                this.FloatTextureOverUnit(unit, clock);
            }

            this.readyList.Clear();
            this.waitingList.Clear();
            this.doneList.Clear();
            foreach (Unit unit in this.Units)
            {
                unit.PrepareForNextCombatTurn();                
            }
            foreach (Unit unit in this.Units)
            {
                if (this.IsUnitDoneWithTurn(unit) || unit.StatusEffects.HasStatus(UnitStatusEffect.SkipTurn))
                {
                    this.PutUnitInDoneList(unit);
                }
                else
                {
                    this.PutUnitInWaitingList(unit);
                }
            }            

            this.PullFromWaitingList();                       
        }

        private void PutUnitInReadyList(Unit unit)
        {
            unit.Sprite.CurrentDrawMode = DrawMode.Normal;
            this.waitingList.RemoveIfExists(unit);
            this.doneList.RemoveIfExists(unit);
            this.readyList.Add(unit);
        }

        private void PutUnitInWaitingList(Unit unit)
        {
            unit.Sprite.CurrentDrawMode = DrawMode.Waiting;
            this.readyList.RemoveIfExists(unit);
            this.doneList.RemoveIfExists(unit);
            this.waitingList.Add(unit);
        }

        private void PutUnitInDoneList(Unit unit)
        {
            unit.Sprite.CurrentDrawMode = DrawMode.Done;
            this.readyList.RemoveIfExists(unit);
            this.waitingList.RemoveIfExists(unit);
            this.doneList.Add(unit);
        }

        /// <summary>
        /// When the close button gets clicked on the tile dialog.
        /// </summary>
        private void HandleTileSelectionDialogCloseButtonClicked(object sender, EventArgs e)
        {
            GuiManager gui = InterfaceManager.Instance.Gui;
            if (gui.Screen.Desktop.Children.Contains(this.tileSelectionDialog))
            {
                gui.Screen.Desktop.Children.Remove(this.tileSelectionDialog);
            }
        }

        /// <summary>
        /// Button handler for an ability.
        /// </summary>
        private void HandleAbilityClicked(object sender, UnitAbilitiesPanel.AbilityClickedEventArgs e)
        {
            AbilityInfo ability = e.ClickedAbility;

            this.ProcessAbility(ability);
        }        

        private void HandleActionFeedUnitIconClicked(object sender, UnitActivityIconsGroup.UnitClickedEventArgs e)
        {
            this.TileSelected(e.Unit.CurrentTile, true);
            this.PanCameraTo(e.Unit.DrawPosition.Center.X, e.Unit.DrawPosition.Center.Y);
        }   

        /// <summary>
        /// After an ability is triggered, it will have effects such as creating floating text
        /// </summary>        
        private void HandleAbilityResults(AbilitySceneResults abilityEffects)
        {
            if (abilityEffects != null)
            {
                foreach (FloatingText text in abilityEffects.FloatingText)
                {
                    this.floatingText.Add(text);
                }
                                
                foreach (ProjectileEffect effect in abilityEffects.Projectiles)
                {
                    this.visualEffects.Add(effect);
                }
                
                foreach (AnimatedEffect effect in abilityEffects.Animations)
                {
                    this.visualEffects.Add(effect);
                }                
            }

            this.RefreshQueues();
        }

        /// <summary>
        /// Ensures everything works properly with the queues. This ensures that units are in the proper queue. 
        /// </summary>
        private void RefreshQueues()
        {
            OrderedSet<Unit> newWaitQueue = new OrderedSet<Unit>(new Comparers.UnitReadinessComparer());            
            foreach (Unit unit in this.waitingList.ToList())
            {
                if (this.IsUnitDoneWithTurn(unit))
                {
                    this.PutUnitInDoneList(unit);                    
                }
                else
                {
                    unit.Sprite.CurrentDrawMode = DrawMode.Waiting;
                    newWaitQueue.Add(unit);
                }
            }

            this.waitingList = newWaitQueue;
        }

        private bool IsUnitDoneWithTurn(Unit unit)
        {
            if (unit.CurrentStats.ActionPoints == 0 || unit.StatusEffects.HasStatus(UnitStatusEffect.Stun))
            {
                return true;
            }

            return false;
        }        

        private void AttackUnit(Unit unitAttacking, Unit unitBeingAttacked)
        {
            int attack = unitAttacking.GetAttackRoll();
            int ap = unitAttacking.GetAttackAP();
            int mitigation = unitBeingAttacked.GetDamageMitigation();

            attack = (attack - mitigation).MinCap(0);

            this.floatingText.Add(FloatingText.CreateRandomlyHorizontalFloatingText(attack.ToString(), unitBeingAttacked.DrawPosition));

            unitAttacking.CurrentStats.ActionPoints -= ap;
            unitBeingAttacked.CurrentStats.HP -= attack;
            
            if (unitAttacking.Equipment.MissingWeapon)
            {
                AnimatedEffect effect = new AnimatedEffect(ResourceId.GameObjects.Effect_SwordSlash.ToString(), unitBeingAttacked.CurrentTile, ResourceType.GameObject, 2);
                effect.DrawPosition = unitBeingAttacked.DrawPosition.Clone();
                this.visualEffects.Add(effect);
            }
            else
            {
                Item weapon = unitAttacking.Equipment.GetWeapon();
                Debug.Assert(weapon != null);

                foreach (AbilityVisualEffectInfo effect in (weapon.Stats as WeaponStats).VisualEffects)
                {
                    AnimatedEffect animation = AbilityDriver.GenerateAnimatedEffect(effect, unitAttacking, unitBeingAttacked.CurrentTile);
                    if (animation != null)
                    {
                        this.visualEffects.Add(animation);
                    }
                }                
            }                      

            if (unitBeingAttacked.CurrentStats.HP <= 0)
            {
                this.ProcessUnitDeath(unitBeingAttacked);
            }

            this.RefreshSelectedUnit();
        }

        /// <summary>
        /// Ability was used on a valid target.
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void UseAbilityOnTarget(AbilityInfo ability, Unit source, Tile target)
        {
            this.targetTile = target;
            ability.Cooldown = ability.Stats.Cooldown;

            AbilitySceneResults results = AbilityDriver.InitiateAbility(ability, source, target);
            this.HandleAbilityResults(results);
            
            this.CurrentState = State.AwaitingAbilityAnimation;            
        }

        /// <summary>
        /// After the animation has reached the target
        /// </summary>
        /// <param name="ability"></param>
        private void AbilityReachedTarget(AbilityInfo ability)
        {
            Debug.Assert(this.SelectedEntity is Unit && this.targetTile != null);

            AbilitySceneResults result = AbilityDriver.HandleAbilityEffects(ability, this.targetTile);            
                            
            foreach(Unit unit in this.Units.ToList()) 
            {
                // foreach ensures we also handle AoE cases and such
                if (unit.CurrentStats.HP <= 0)
                {
                    this.ProcessUnitDeath(unit);
                }
            }            

            this.HandleAbilityResults(result);            
            
            if (!this.UnitPerformedAnAction(ability.Owner as Unit, UnitActionType.Ability, ability))
            {
                this.CurrentState = State.UnitSelected;
            }            
        }

        /// <summary>
        /// Handles a unit dieing.
        /// </summary>
        private void ProcessUnitDeath(Unit unitBeingKilled)
        {
            Tile tileOfDeath = unitBeingKilled.CurrentTile;
            this.Units.Remove(unitBeingKilled);
            this.readyList.RemoveIfExists(unitBeingKilled);
            this.waitingList.RemoveIfExists(unitBeingKilled);
            this.doneList.RemoveIfExists(unitBeingKilled);
            this.deadEnemies.Add(unitBeingKilled);

            tileOfDeath.TileResident = null;

            Obstacle skelly = new Obstacle("Skeleton");
            tileOfDeath.AddPassableObject(skelly);
        }

        /// <summary>
        /// When a selected unit clicks on an enemy unit... determines whether the attack is possible, accounting for range and AP.
        /// </summary>
        /// <param name="targetUnit"></param>
        private bool AttackIsPossible(Unit targetUnit)
        {
            Debug.Assert(this.SelectedEntity is Unit);

            Unit currentUnit = this.SelectedEntity as Unit;

            int currentUnitAPCost = currentUnit.GetAttackAP();
            int currentUnitMinRange = currentUnit.GetAttackMinRange();

            if (currentUnit.CurrentStats.ActionPoints < currentUnitAPCost)
            {
                return false;
            }

            HashSet<Tile> tileRange = Grid.GetTileRadius(currentUnit.CurrentTile, currentUnit.GetAttackMaxRange(), true, currentUnit.GetAttackMinRange());
            if (tileRange.Contains(targetUnit.CurrentTile))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles state changing to unit getting selected
        /// </summary>
        private void StateChangedToUnitSelected()
        {
            this.RefreshSelectedUnit();
        }

        /// <summary>
        /// Some unit getting selected.
        /// </summary>
        private void RefreshSelectedUnit()
        {
            if (this.SelectedEntity != null)
            {
                this.commandPane.SetSelectedEntityDisplay(this.SelectedEntity);
            }

            if (this.SelectedEntity is Unit && this.SelectedEntity.PlayerCanCommand)
            {
                if (this.readyList.Contains(this.SelectedEntity as Unit))
                {
                    this.Grid.SetDisplayedMovementRadius(this.SelectedEntity as Unit);

                    if ((this.SelectedEntity as Unit).CanAttack)
                    {
                        this.Grid.SetDisplayedAttackRadius(this.SelectedEntity as Unit);
                    }
                    else
                    {
                        this.Grid.ClearTargetRadius();
                    }
                }
            }
            else
            {
                this.Grid.ClearMovementRadius();
                this.Grid.ClearTargetRadius();
            }

            this.RefreshUIState();
        }

        /// <summary>
        /// NOTE: Currently Unused
        /// Shows the info about the selected tile in the tileSelectionDialog, if visible.
        /// </summary>
        /// <param name="tile"></param>
        private void DisplayTileInfoInUI(Tile tile)
        {
            GuiManager gui = InterfaceManager.Instance.Gui;

            if (gui.Screen.Desktop.Children.Contains(this.tileSelectionDialog))
            {
                this.tileSelectionDialog.MainLabelText = tile.TileDisplayName;
                this.tileSelectionDialog.SetTileIcon(tile.Texture);
            }
        }

        private void EnemyTurnReadyToAttack()
        {
            if (this.enemyDecisionEngine.CanPerformAnAttack(this.activeEnemy))
            {
                Tile target = this.enemyDecisionEngine.DecideAttackTarget(this.activeEnemy, this.Grid, this.Units);

                if (target != null)
                {
                    Debug.Assert(target.TileResident != null && target.TileResident is Unit);
                    this.PanCameraTo(this.activeEnemy);
                    this.AttackUnit(this.activeEnemy, target.TileResident as Unit);
                    this.UnitPerformedAnAction(this.activeEnemy, UnitActionType.Attack);
                    this.pauseUnitActions = 500;
                }
            }
            else
            {
                this.UnitDoneForTurn(this.activeEnemy);
                this.UnitPerformedAnAction(this.activeEnemy, UnitActionType.EndTurn);                
            }
        }

        /// <summary>
        /// Indicates that the unit is done for the remainder of the turn.
        /// </summary>
        /// <param name="unit"></param>
        private void UnitDoneForTurn(Unit unit)
        {
            unit.Sprite.CurrentDrawMode = DrawMode.Done;            
        }

        /// <summary>
        /// Puts a ready unit into the ready queue.
        /// </summary>
        /// <param name="unit"></param>
        private void UnitIsReady(Unit unit)
        {
            this.PutUnitInReadyList(unit);            
            IconInfo icon = TextureManager.Instance.GetIconInfo("Focus");
            this.FloatTextureOverUnit(unit, icon);
            if (unit.IsHostile)
            {
                this.CurrentState = State.EnemyTurnReadyForNewMove;
            }
            else
            {
                if (this.SelectedEntity == null)
                {
                    this.SelectedEntity = unit;
                    this.PanCameraTo(unit.DrawPosition.X, unit.DrawPosition.Y);
                }

                this.CurrentState = State.UnitSelected;
            }            
        }

        private void FloatTextureOverUnit(Unit unit, IconInfo icon)
        {
            Vector2 pos = new Vector2();
            pos.X = unit.DrawPosition.Left + 8;
            pos.Y = unit.DrawPosition.Top - 8;
            FloatingText effect = new FloatingText(null, pos, icon, 1000, null, true, 0.5f);

            unit.Sprite.AddSubSprite(effect);
            //this.floatingText.Add(effect);
        }

        /// <summary>
        /// Enemy turn is ready to process some new move.
        /// </summary>
        private void EnemyTurnReadyForNewMove()
        {
            this.activeEnemy = null;
            foreach (Unit unit in this.readyList)
            {
                if (unit.IsHostile)
                {
                    this.activeEnemy = unit;
                    break;
                }
            }

            Debug.Assert(this.activeEnemy != null, "Error in the logic somewhere! Enemy should not be null!");
                       
            Tile target = this.enemyDecisionEngine.DecideMovementTarget(this.activeEnemy, this.Grid, this.Units);

            this.movementTiles = this.Grid.GetPathBetween(this.activeEnemy.CurrentTile, target);

            if (this.movementTiles != null)
            {
                // get subset of movement tiles, based on the enemy's AP
                int lastTile = this.movementTiles.Count;
                int cost = 0;
                for (int i = 0; i < this.movementTiles.Count; ++i)
                {
                    cost += this.movementTiles[i].Cost;
                    if (cost > this.activeEnemy.CurrentStats.ActionPoints)
                    {
                        lastTile = i;
                        break;
                    }
                }

                this.movementTiles = this.movementTiles.GetRange(0, lastTile);
            }

            if (this.movementTiles != null && this.movementTiles.Count > 0)
            {                
                this.CurrentState = State.EnemyUnitMoving;
            }
            else
            {
                this.CurrentState = State.EnemyUnitReadyToAttack;                
            }
        }

        private void ClearSelection()
        {
            this.commandPane.ClearUnitDisplay();
            this.SelectedEntity = null;
            this.targetTile = null;
            this.CurrentState = State.Idle;
            this.Grid.ClearMovementRadius();
            this.Grid.ClearTargetRadius();
            this.activeAbility = null;

            this.RefreshUIState();
        }

        private void HandleContinueClicked(object sender, EventArgs e)
        {
            this.ClearSelection();

            // Float clocks over all units made to wait intentionally, as long as they are waiting and not ending turn
            if (this.waitingList.Count > 0)
            {
                IconInfo check = TextureManager.Instance.GetIconInfo("SandClock");
                foreach (Unit unit in this.readyList)
                {
                    this.FloatTextureOverUnit(unit, check);
                }
            }

            this.PullFromWaitingList(true);            

            //this.PlayerEndedTurn();
            //this.EnemyTurnStart();
        }

        protected void HandleDialogClosed(object sender, EventArgs e)
        {
            if (sender == this.openDialog)
            {
                this.openDialog = null;
                this.CurrentState = this.storedState;
            }
        }

        /// <summary>
        /// Handles a unit moving from one place to the next
        /// </summary>
        /// <param name="entity"></param>
        private void HandleUnitTransitioning(GameEntity entity)
        {
            if (!entity.IsTransitioning)
            {
                Debug.Assert(this.movementTiles != null);

                if (this.movementTiles.Count == 0)
                {
                    // Done moving
                    entity.Sprite.ResetTransitionSpeed();
                    if (this.CurrentState == State.EnemyUnitMoving)
                    {
                        if (!this.UnitPerformedAnAction(this.activeEnemy as Unit, UnitActionType.Move))
                        {
                            this.CurrentState = State.EnemyUnitReadyToAttack;
                        }
                    }
                    else if (this.CurrentState == State.UnitMoving)
                    {
                        if (!this.UnitPerformedAnAction(this.SelectedEntity as Unit, UnitActionType.Move))
                        {
                            this.CurrentState = State.UnitSelected;
                        }
                    }

                    return;
                }
                else
                {
                    Tile nextTile = this.movementTiles[0];
                    this.movementTiles.RemoveAt(0);
                    entity.InitiateTransitionTo(nextTile);
                }
            }
            else
            {
                this.PanCameraTo(entity.DrawPosition.X, entity.DrawPosition.Y);
            }
        }

        /// <summary>
        /// Sets the enabled and disabled state of the CommandPane Continue button, as well as its text.
        /// </summary>
        private void SetCommandPaneButtonState()
        {
            this.commandPane.ContinueButton.Text = "End Turn";
            this.commandPane.ContinueButton.Enabled = true;
            if (this.readyList.Count > 0 && this.readyList.Any(a => !a.IsHostile))
            {
                if (this.waitingList.Count != 0)
                {
                    this.commandPane.ContinueButton.Text = "Wait";
                }
            }
            else if (this.readyList.Count > 0)
            {
                this.commandPane.ContinueButton.Enabled = false;
            }
        }

        private void ShowMessageBox(string message)
        {
            MessageDialog dialog = MessageDialog.CreateDialog(message);
            this.openDialog = dialog;
            this.storedState = this.CurrentState;
            this.CurrentState = State.ShowingDialog;
        }

        # region End Combat and go to Management Scene

        /// <summary>
        /// Manage starting of combat
        /// </summary>
        private void TransitionFromCombatToManagement()
        {
            GameStateManager.Instance.PopScene(); // this scene                        
            
            // So that this scene no longer handles these clicks
            this.Grid.TileClicked -= this.HandleClickedOnATile;
            this.ClearUIHandlers();

            ZoneManagementScene scene = (ZoneManagementScene)GameStateManager.Instance.CurrentScene; // scene we want
            scene.ReturnFromCombatScene();                       
        }

        #endregion

        # region Start Combat from Management Scene

        /// <summary>
        /// Given a list of game objects, puts them all in the proper list
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="grid"></param>
        public void SetGameObjects(List<GameObject> gameObjects, TileGrid grid)
        {
            this.Grid = grid;                        

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject is Unit)
                {
                    this.Units.Add(gameObject as Unit);
                }
                else if (gameObject is Obstacle)
                {
                    this.Obstacles.Add(gameObject as Obstacle);
                }
                else if (gameObject is Building)
                {
                    this.Buildings.Add(gameObject as Building);
                }
                else if (gameObject is Zone)
                {
                    this.Zones.Add(gameObject as Zone);
                }
                else
                {
                    Debug.Assert(false, "Unknown object transfered");
                }
            }

            this.Grid.TileClicked += this.HandleClickedOnATile;
            this.SetCommandPaneButtonState();
        }        

        public void DispatchUnitsForCombat()
        {
            // Add enemies
            int numEnemies = Utilities.GetRandomNumber(3, 3 + GameStateManager.Instance.GameStatus.CurrentDay);
            this.Units.AddRange(EnemyGenerationUtilities.GetEasyEnemies(numEnemies));

            this.Units.Add(PlayerStateManager.Instance.Player);

            List<Building> dispatchBuildings = new List<Building>();
            dispatchBuildings.AddRange(this.Buildings.Where(a => a.CanDeployTroopsToBuilding));

            // TODO: let the player choose this   
            Building dispatch = dispatchBuildings[0];

            // Take the ~ 8x3 area under the guild to place units.
            int tileXMin = dispatch.CurrentTile.Coordinate.X - 2;
            int tileXMax = dispatch.CurrentTile.Coordinate.X + dispatch.TileWidth + 2;
            int tileYMin = dispatch.CurrentTile.Coordinate.Y + dispatch.TileHeight + 1;
            int tileYMax = tileYMin + 2;

            List<Tile> tilesOfDispatch = this.Grid.GetRangeOfTiles(tileXMin, tileXMax, tileYMin, tileYMax, true);

            List<Tile> tilesOfEnemyDispatch = new List<Tile>();
            foreach (Zone zone in this.Zones)
            {
                tilesOfEnemyDispatch.AddRangeIfNotExists(zone.CurrentTiles);
            }

            foreach (Unit unit in this.Units)
            {
                unit.PrepareForNextCombatTurn();

                if (unit.PlayerCanCommand)
                {                    
                    unit.SetLocationTo(tilesOfDispatch.PopRandomItem<Tile>());
                }
                else
                {
                    unit.SetLocationTo(tilesOfEnemyDispatch.PopRandomItem<Tile>());
                }
            }

            this.CurrentState = State.Idle;            

            this.AdvanceToNextTurn();

            this.ShowMessageBox("Enemies are attacking our town! Oh no!");
        }        
        
        # endregion
    }
}
