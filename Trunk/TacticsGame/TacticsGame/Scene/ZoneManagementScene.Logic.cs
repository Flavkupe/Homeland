using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Dialogs;
using System.Diagnostics;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TacticsGame.Text;
using TacticsGame.Map;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Structures;
using TacticsGame.Edicts;
using TacticsGame.PlayerThings;

namespace TacticsGame.Scene
{
    public partial class ZoneManagementScene
    {       
        /// <summary>
        /// When a misc entity is selected.
        /// </summary>
        private void RefreshMiscSelection()
        {
            this.commandPane.SetSelectedEntityDisplay(this.SelectedEntity);
        }

        /// <summary>
        /// When a building is selected.
        /// </summary>
        private void RefreshBuildingSelection()
        {
            this.commandPane.SetSelectedBuildingDisplay(this.SelectedEntity as Building);
        }
       
        /// <summary>
        /// Whether the player can afford the building passed, in terms of resources and money.
        /// </summary>
        private bool PlayerCanAffordBuilding(IBuildable building)
        {
            if (PlayerStateManager.Instance.PlayerInventory.Money < building.MoneyCost)
            {
                return false;
            }

            foreach (ObjectValuePair<string> itemAndCost in building.ResourceCost)
            {
                if (PlayerStateManager.Instance.PlayerInventory.GetItemCount(itemAndCost.Object) < itemAndCost.Value)
                {
                    return false;
                }
            }

            return true;
        }
       
        /// <summary>
        /// Puts a building on a tile and handles followup logic.
        /// </summary>
        /// <param name="selection"></param>
        private void PlaceBuildingOnTile(Tile selection)
        {
            if (this.buildingCanBePlaced && !this.draggingBuilding.CannotBeBuilt)
            {
                IBuildable building = this.draggingBuilding as IBuildable;

                // Check that the building can still be afforded
                if (this.PlayerCanAffordBuilding(building))
                {
                    float x = this.draggingBuilding.DrawPosition.Left + 16;
                    float y = this.draggingBuilding.DrawPosition.Top;

                    PlayerStateManager.Instance.PlayerInventory.Money -= building.MoneyCost;

                    string text = string.Format("-{0}", building.MoneyCost);
                    IconInfo icon = TextureManager.Instance.GetIconInfo("Coin");
                    this.floatingText.Add(new FloatingText(text, new Vector2(x, y), icon, 2000, Color.Yellow));

                    foreach (ObjectValuePair<string> itemAndCost in building.ResourceCost)
                    {
                        y -= 30;

                        string item = itemAndCost.Object;
                        int cost = itemAndCost.Value;

                        PlayerStateManager.Instance.PlayerInventory.RemoveItems(item, cost);
                        IconInfo icon2 = TextureManager.Instance.GetIconInfo(item);
                        string text2 = string.Format("-{0}", cost);
                        this.floatingText.Add(new FloatingText(text2, new Vector2(x, y), icon2, 2000, Color.Yellow));
                    }
                }

                this.draggingBuilding.SetLocationTo(selection);

                if (this.draggingBuilding is Building)
                {
                    this.Buildings.Add(this.draggingBuilding as Building);
                }
                else if (this.draggingBuilding is Structure)
                {
                    this.obstacles.Add(this.draggingBuilding as Structure);
                }

                if (this.draggingBuilding.BuildAgain)
                {
                    this.draggingBuilding = Activator.CreateInstance(this.draggingBuilding.GetType()) as IBuildable;
                }
                else
                {
                    this.draggingBuilding = null;
                    this.ClearSelection();
                }
            }
        }

        /// <summary>
        /// Handle stuff like showing the right tile colors and crap
        /// </summary>
        private void RefreshBuildSelectedBuildingState()
        {
            Debug.Assert(this.draggingBuilding != null);

            float zoom = GameStateManager.Instance.ZoomLevel;

            this.ClearExistingTileFilters();

            int halfTile = this.grid.VisibleTileDimensions / 2;

            int xPos = (AbsoluteMouseX - halfTile).GetClampedValue(0, grid.LevelPixelWidth).DivideBy(zoom);
            int yPos = (AbsoluteMouseY - halfTile).GetClampedValue(0, grid.LevelPixelHeight).DivideBy(zoom);

            this.draggingBuilding.DrawPosition = this.draggingBuilding.DrawPosition.CloneAndRelocate(xPos, yPos);

            int xCoord = AbsoluteMouseX / this.grid.VisibleTileDimensions;
            int yCoord = AbsoluteMouseY / this.grid.VisibleTileDimensions;

            xCoord = xCoord.GetClampedValue(0, this.grid.Width - this.draggingBuilding.TileWidth);
            yCoord = yCoord.GetClampedValue(0, this.grid.Height - this.draggingBuilding.TileHeight);

            this.draggingBuilding.Coordinates = new Point(xCoord, yCoord);
            this.draggingBuilding.CurrentTile = grid.GetTile(xCoord, yCoord);

            this.buildingCanBePlaced = true;
            foreach (Tile tile in this.draggingBuilding.CurrentTiles)
            {
                if (!tile.IsTraversable)
                {
                    this.buildingCanBePlaced = false;
                    break;
                }
            }

            foreach (Tile tile in this.draggingBuilding.CurrentTiles)
            {
                tile.Filter = this.buildingCanBePlaced ? Tile.TileDrawFilter.CanPlaceBuilding : Tile.TileDrawFilter.CannotPlaceBuilding;
                this.tilesWeJustFiltered.Add(tile);
            }
        }

        /// <summary>
        /// Clears the tile highlighting for the building we are dragging.
        /// </summary>
        private void ClearExistingTileFilters()
        {
            // Reset the previous filters
            foreach (Tile tile in this.tilesWeJustFiltered)
            {
                tile.Filter = null;
            }

            tilesWeJustFiltered.Clear();
        }        

        /// <summary>
        /// Clears all selections.
        /// </summary>
        private void ClearSelection()
        {
            this.SelectedEntity = null;
            this.commandPane.ClearUnitDisplay();

            this.ClearExistingTileFilters();

            this.draggingBuilding = null;

            this.CurrentState = State.Idle;
        }

        /// <summary>
        /// Show buttons for idle menu (equivalent to guild selection)
        /// </summary>
        private void ShowIdleMenuButtons()
        {
            this.selectedEntity = this.guildBuilding;
            this.commandPane.ShowGuildButtonGroup(this.guildBuilding);
        }

        /// <summary>
        /// Starts the screen fade animation
        /// </summary>
        private void InitiateFade()
        {
            this.fade.Reset();
            this.CurrentState = State.Fading;
        }

        /// <summary>
        /// Stores the state of the scene before opening a dialog.
        /// </summary>
        private void StorePreDialogState()
        {
            // Avoid making stored state the same as current state (opening dialog twice)
            if (this.CurrentState != State.ShowingDialog)
            {
                this.storedState = this.CurrentState;
            }
        }

        /// <summary>
        /// When units button is clicked.
        /// </summary>
        /// <param name="unit"></param>
        private void OpenUnitDialog(Unit unit = null)
        {
            this.StorePreDialogState();

            this.openDialog = UnitDialog.CreateDialog(this.DecisionMakingUnits.ToList<Unit>(), unit);
            this.CurrentState = State.ShowingDialog;
        }

        /// <summary>
        /// Deals with logic about what will happen next day.
        /// </summary>
        private void PrepareForNextDay()
        {
            this.TransitionFromManagementToCombat();

            //Temp
            //this.ResetTurn();
        }

        /// <summary>
        /// Initializes the next "round".
        /// </summary>
        protected virtual void ResetTurn()
        {

            if (GameStateManager.Instance.GetObjectiveStatus() == ObjectiveStatus.Succeeded)
            {
                this.ShowMessageBox("Are are success!");   
            }
            else if (GameStateManager.Instance.GetObjectiveStatus() == ObjectiveStatus.Failed)
            {
                this.ShowMessageBox("Oh no! You are lose!");
            }

            this.actionFeed.ClearUnitView();
            this.actionFeed.ClearContents();

            this.selectedEntity = this.guildBuilding;
            this.commandPane.ShowGuildButtonGroup(this.guildBuilding);

            this.RefreshUnitAndOwnerStatusForNewTurn();

            foreach (Building building in this.Buildings)
            {
                building.RefreshAtStartOfTurn();
                this.TaxVisitors(building);
            }

            this.commandPane.ContinueButton.Text = "Hurry!";
            this.allUnitsDone = false;
            this.activityTickDuration = baseActivityTickDuration;
        }

        private void ShowMessageBox(string message)
        {
            MessageDialog dialog = MessageDialog.CreateDialog(message);
            this.openDialog = dialog;
            this.storedState = this.CurrentState;
            this.CurrentState = State.ShowingDialog;
        }

        /// <summary>
        /// Taxes each visitor.
        /// </summary>
        /// <param name="building"></param>
        private void TaxVisitors(Building building)
        {
            int taxes = PlayerStateManager.Instance.ActiveTown.VisitorTaxes;

            bool mercantilismEnabled = PlayerStateManager.Instance.ActiveTown.EdictIsActive(EdictType.Mercantilism);

            if (taxes > 0)
            {
                foreach (DecisionMakingUnit visitor in building.Visitors)
                {
                    if (mercantilismEnabled && visitor.IsTrader)
                    {
                        // Under mercantilism, traders are tax exempt. 
                        continue;
                    } 

                    visitor.Inventory.Money -= taxes;
                    PlayerStateManager.Instance.PlayerInventory.Money += taxes;
                    this.AnnounceTextToUI(this.unitDecisionUtils.GetStringForUnitTaxed(visitor, taxes));
                    // TODO: display this in some meaningful way.
                }
            }
        }

        /// <summary>
        /// Manage starting of combat
        /// </summary>
        private void TransitionFromManagementToCombat()
        {
            ZoneCombatScene newScene = new ZoneCombatScene();
            
            // load the scene-specific content first 
            newScene.LoadContent();

            // So that this scene no longer handles these clicks
            this.grid.TileClicked -= this.HandleClickedOnATile;
            this.ClearUIHandlers();

            List<GameObject> allObjects = new List<GameObject>();
            allObjects.AddRange(this.DecisionMakingUnits);
            allObjects.AddRange(this.obstacles);
            allObjects.AddRange(this.Buildings);
            allObjects.AddRange(this.zones);
            newScene.SetGameObjects(allObjects, this.grid);
            newScene.DispatchUnitsForCombat();
            GameStateManager.Instance.PushScene(newScene);
        }

        /// <summary>
        /// Called after ZoneCombatScene is done
        /// </summary>
        public void ReturnFromCombatScene()
        {
            this.grid.TileClicked += this.HandleClickedOnATile;
            this.SetUIHandlers();
            this.ResetTurn();
        }
    }
}
