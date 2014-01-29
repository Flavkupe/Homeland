using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.UI.Dialogs;
using System.Diagnostics;
using TacticsGame.GameObjects.Units;
using TacticsGame.Managers;
using TacticsGame.Map;
using TacticsGame.UI;
using TacticsGame.AI.MaintenanceMode;
using Microsoft.Xna.Framework.Input;

namespace TacticsGame.Scene
{
    public partial class ZoneManagementScene
    {
        protected void HandleDialogClosed(object sender, EventArgs e)
        {
            if (sender == this.openDialog)
            {
                this.openDialog = null;
                this.CurrentState = this.storedState;
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

        private void HandleCommandPaneCommand(object sender, CommandPaneCommandEventArgs e)
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

        private void HandleContinueButtonClicked(object sender, EventArgs e)
        {
            if (this.Simulation.DayIsOver)
            {
                this.PlayerEndedTurn();                
            }
            else
            {
                if (GameManager.GameStateManager.GameSpeed == GameSpeed.Fast)
                {
                    GameManager.GameStateManager.GameSpeed = GameSpeed.Normal;
                    this.commandPane.ContinueButton.Text = "Hurry!";
                }
                else
                {
                    GameManager.GameStateManager.GameSpeed = GameSpeed.Fast;                    
                    this.commandPane.ContinueButton.Text = "Slow Down!";
                }
            }
        }

        private void HandleDailyReportClosed(object sender, EventArgs e)
        {
            this.InitiateEndOfTurnFade();
        }

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

        /// <summary>
        /// Handles clicking the icons in the unit feed
        /// </summary>
        private void HandleUnitIconClicked(object sender, UI.Groups.UnitActivityIconsGroup.UnitActivityClickedEventArgs e)
        {
            this.OpenUnitDialog(e.Activity.Unit);
        }

        private void HandleCaravanButtonClicked()
        {
            CaravanDialog dialog = CaravanDialog.CreateDialog();
            this.openDialog = dialog;            
            this.CurrentState = State.ShowingDialog;
        }

        private void HandleRequestsButtonClicked()
        {
            PlaceOrdersDialog dialog = PlaceOrdersDialog.CreateDialog();
            this.openDialog = dialog;
            dialog.SetPurchaseOrders();            
            this.CurrentState = State.ShowingDialog;
        }

        private void HandleSellButtonClicked()
        {
            Debug.Assert(this.selectedEntity is Building && (this.selectedEntity as Building).Owner != null);
            PlayerStockDialog dialog = PlayerStockDialog.CreateDialog(true, PlayerStockDialogMode.Sell);
            this.openDialog = dialog;
            dialog.TargetUnit = ((Building)this.selectedEntity).Owner;
            this.CurrentState = State.ShowingDialog;
        }

        private void HandleEdictsButtonClicked()
        {
            EdictsDialog dialog = EdictsDialog.CreateDialog();
            this.openDialog = dialog;
            dialog.SetEdicts();
            this.CurrentState = State.ShowingDialog;
        }

        private void HandleFinanceButtonClicked()
        {
            FinanceDialog dialog = FinanceDialog.CreateDialog();
            this.openDialog = dialog;
            dialog.SetActiveFinances();            
            this.CurrentState = State.ShowingDialog;
        }

        private void HandleVisitorsButtonClicked()
        {
            Debug.Assert(this.selectedEntity is Building);

            this.StorePreDialogState();

            VisitorsDialog dialog = VisitorsDialog.CreateDialog();
            this.openDialog = dialog;
            dialog.SetVisitors(this.selectedEntity as Building);
            dialog.HiredUnit += new EventHandler<EventArgsEx<Unit>>(HandleHiredUnit);
            this.CurrentState = State.ShowingDialog;            
        }

        private void HandleHiredUnit(object sender, EventArgsEx<Unit> e)
        {
            Debug.Assert(e.Value2 is int);
            DecisionMakingUnit unit = e.Value as DecisionMakingUnit;
            int cost = (int)e.Value2;

            PlayerStateManager.Instance.PlayerInventory.Money -= cost;
            unit.Inventory.Money += cost;
            this.DecisionMakingUnits.Add(unit);
            (this.selectedEntity as Building).Visitors.Remove(unit);            
        }

        private void HandleBuildBuildingIconClicked(object sender, UI.Panels.BuildingToBuildIconClickedEventArgs e)
        {
            this.CurrentState = State.PlacingBuilding;
            this.draggingBuilding = e.Building as IBuildable;

            if (!PlayerCanAffordBuilding((IBuildable)e.Building))
            {
                this.draggingBuilding.CannotBeBuilt = true;
            }
        }

        private void HandleUnitButtonClicked()
        {
            this.OpenUnitDialog();
        }

        private void HandleShowStockClicked()
        {
            this.StorePreDialogState();

            if (this.selectedEntity == this.guildBuilding)
            {
                this.openDialog = PlayerStockDialog.CreateDialog();
                this.CurrentState = State.ShowingDialog;
            }
            else
            {
                Debug.Assert(this.selectedEntity != null && this.selectedEntity is Building && ((Building)this.selectedEntity).IsBuildingWithStock && ((Building)this.selectedEntity).IsBuildingWithOwner, "Should not be able to throw this event without building selected!");

                StockDialog dialog = StockDialog.CreateDialog();
                dialog.SetStockOwner((this.selectedEntity as Building).Owner);
                this.openDialog = dialog;
                this.CurrentState = State.ShowingDialog;
            }
        }

        private void HandleStateChangedToMiscSelection()
        {
            this.RefreshMiscSelection();
        }

        private void HandleStateChangedToBuildingSelected()
        {
            this.RefreshBuildingSelection();
        }

        /// <summary>
        /// Fires off the post-dialog event delegate in case it got set, and clears it afterwards.
        /// </summary>
        private void HandleShowingDialogStateChanged()
        {
            if (this.postDialogEvent != null)
            {
                this.postDialogEvent();
            }

            this.postDialogEvent = null;
        }

        /// <summary>
        /// Override if there is no action feed.
        /// </summary>
        protected virtual void ShowActivityResultsOnActionFeed(UnitManagementActivity activity, ActivityResult results)
        {
            this.actionFeed.UpdateResultOnActivity(activity, results);
        }

        protected virtual void UpdateActivityOnUI(UnitManagementActivity activity)
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
        protected virtual void AddActivityToUIFeed(UnitManagementActivity activity)
        {
            this.actionFeed.AddToFeed(activity);
        }
    }
}
