using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.UI.Dialogs;
using System.Diagnostics;
using TacticsGame.GameObjects.Units;
using TacticsGame.Managers;

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

        private void HandleContinueButtonClicked(object sender, EventArgs e)
        {
            if (this.allUnitsDone)
            {
                this.ProcessTurnDone();
                this.InitiateFade();
            }
            else
            {
                if (this.activityTickDuration == hurryActivityTickDuration)
                {
                    this.activityTickDuration = baseActivityTickDuration;
                    this.commandPane.ContinueButton.Text = "Hurry!";
                }
                else
                {
                    this.activityTickDuration = hurryActivityTickDuration;
                    this.commandPane.ContinueButton.Text = "Slow Down!";
                }
            }
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
    }
}
