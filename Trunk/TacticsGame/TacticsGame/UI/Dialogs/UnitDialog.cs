using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using TacticsGame.GameObjects.Units;
using TacticsGame.UI.Controls;
using TacticsGame.UI.Groups;
using TacticsGame.Managers;
using TacticsGame.PlayerThings;

namespace TacticsGame.UI.Dialogs
{
    public partial class UnitDialog : ModalDialogControl
    {
        private Unit selectedUnit = null;

        private bool hireUnitMode = false; 

        private State state = State.ShowingUnitList;
        private enum State
        {
            ShowingUnitList,
            ShowingUnitStats,
            ShowingUnitInventory,
            ShowingUnitAttributes,
        }

        /// <summary>
        /// Initialize dialog
        /// </summary>
        private UnitDialog()
            : base()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Show a unit stats for hire mode
        /// </summary>
        public bool HireUnitMode
        {
            get { return hireUnitMode; }

            set
            {
                this.hireUnitMode = value;
                this.SetControlVisible(this.uxHireButton, value);
                this.SetControlVisible(this.uxBackButton, !value);
            }
        }

        public event EventHandler<EventArgsEx<Unit>> HiredUnit;        

        public static UnitDialog CreateDialog(List<Unit> list, Unit selectedUnit = null, bool showAutomatically = true) 
        {
            UnitDialog newDialog = new UnitDialog();
            if (showAutomatically)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            if (list != null)
            {
                List<Unit> allUnits = new List<Unit>();
                allUnits.Add(PlayerStateManager.Instance.Player);
                allUnits.AddRange(list);

                newDialog.SetUnits(allUnits);
            }
            else
            {
                newDialog.HireUnitMode = true;
            }

            if (selectedUnit != null)
            {
                newDialog.SetToViewUnitStats(selectedUnit);
            }

            newDialog.BringToFront();
            newDialog.AffectsOrdering = false;

            return newDialog;
        }        
       
        #region Handlers

        private void HandleGiveButtonClicked(object sender, EventArgs e)
        {
            PlayerStockDialog dialog = PlayerStockDialog.CreateDialog(true, PlayerStockDialogMode.Give);
            Debug.Assert(this.selectedUnit is IMakeDecisions);
            dialog.TargetUnit = this.selectedUnit as IMakeDecisions;
            dialog.CloseClicked += this.HandleGiveDialogClosed;
        }

        private void HandleGiveDialogClosed(object sender, EventArgs e)
        {
            this.SetToViewInventory(this.selectedUnit);
        }

        private void HandleHireButtonPressed(object sender, EventArgs e)
        {
            if (PlayerStateManager.Instance.ActiveTown.DecisionMakingUnits.Count >= PlayerStateManager.Instance.ActiveTown.GetUnitCapacity())
            {                
                MessageDialog.CreateDialog("\"Sorry buddy, but it doesn't seem like there is any room in this town for me.\"");
                return;
            }

            int cost = ((ICanBeHired)this.selectedUnit).GetHireCost();                       
            
            if (PlayerStateManager.Instance.PlayerInventory.Money >= cost)
            {
                MessageDialog prompt = MessageDialog.CreateDialog(string.Format("It will cost {0} to hire {1}. Pay?", cost, this.selectedUnit.DisplayName));
                prompt.YesOrNoPromptMode = true;
                prompt.ButtonResult += this.HandleHirePromptButtonResult;
            }
            else
            {
                MessageDialog prompt = MessageDialog.CreateDialog(string.Format("It will cost {0} to hire {1}. You cannot afford it.", cost, this.selectedUnit.DisplayName));
            }
        }

        private void HandleHirePromptButtonResult(object sender, EventArgsEx<MessageDialogResult> e)
        {
            if (e.Value == MessageDialogResult.Yes)
            {
                if (this.HiredUnit != null)
                {
                    int cost = ((ICanBeHired)this.selectedUnit).GetHireCost();

                    EventArgsEx<Unit> args = new EventArgsEx<Unit>(this.selectedUnit);
                    args.Value2 = cost;
                    this.HiredUnit(this, args);
                    MessageDialog prompt = MessageDialog.CreateDialog(string.Format("Unit {0} was hired and will begin working next turn.", this.selectedUnit.DisplayName));
                    this.CloseThisDialog();
                }                
            }
        }

        protected override void HandleCloseClicked(object sender, EventArgs e)
        {
            // For next time.
            this.SetToViewUnitList();
            base.HandleCloseClicked(sender, e);
        }

        private void HandleBackClicked(object sender, EventArgs e)
        {
            switch (this.state)
            {
                case State.ShowingUnitStats:
                    this.SetToViewUnitList();
                    return;
                case State.ShowingUnitInventory:
                case State.ShowingUnitAttributes:
                    this.SetToViewUnitStats(selectedUnit);
                    return;                    
                default:
                    return;
            }
        }

        private void HandleUnitClicked(object sender, EventArgs e)
        {
            this.SetToViewUnitStats((sender as TooltipButtonAndTextControl).Tag as Unit);
        }

        private void HandleInventoryClicked(object sender, EventArgs e)
        {
            this.SetToViewInventory(this.selectedUnit);
        }

        private void HandleAttributesClicked(object sender, EventArgs e)
        {
            this.SetToViewAttributes(this.selectedUnit);
        }        

        #endregion

        #region Modes

        private void SetToViewUnitList()
        {
            this.state = State.ShowingUnitList;
            this.SetControlVisible(this.uxUnitList, true);
            this.SetControlVisible(this.uxBackButton, false);
            this.SetControlVisible(this.uxUnitStats, false);
            this.SetControlVisible(this.uxInventoryButton, false);
            this.SetControlVisible(this.uxUnitInventory, false);
            this.SetControlVisible(this.uxUnitAttributes, false);
            this.SetControlVisible(this.uxAttributesButton, false);
            this.SetControlVisible(this.uxHireButton, this.HireUnitMode);
            this.SetControlVisible(this.uxGiveButton, false);
            this.selectedUnit = null;
        }

        private void SetToViewAttributes(Unit unit)
        {
            this.state = State.ShowingUnitAttributes;
            this.uxUnitAttributes.SetUnitStats(unit);
            this.SetControlVisible(this.uxUnitList, false);            
            this.SetControlVisible(this.uxBackButton, true);
            this.SetControlVisible(this.uxUnitStats, false);
            this.SetControlVisible(this.uxInventoryButton, false);            
            this.SetControlVisible(this.uxUnitInventory, false);
            this.SetControlVisible(this.uxUnitAttributes, true);
            this.SetControlVisible(this.uxAttributesButton, false);
            this.SetControlVisible(this.uxHireButton, this.HireUnitMode);
            this.SetControlVisible(this.uxGiveButton, false);
        }

        private void SetToViewInventory(Unit unit)
        {
            this.state = State.ShowingUnitInventory;
            this.uxUnitInventory.SetUnitInventory(unit);
            this.SetControlVisible(this.uxUnitList, false);
            this.SetControlVisible(this.uxBackButton, true);
            this.SetControlVisible(this.uxUnitStats, false);
            this.SetControlVisible(this.uxInventoryButton, false);
            this.SetControlVisible(this.uxAttributesButton, false);
            this.SetControlVisible(this.uxUnitInventory, true);
            this.SetControlVisible(this.uxUnitAttributes, false);
            this.SetControlVisible(this.uxAttributesButton, false);
            this.SetControlVisible(this.uxHireButton, this.HireUnitMode);
            this.SetControlVisible(this.uxGiveButton, !this.HireUnitMode && !unit.IsPlayer);
        }

        public void SetToViewUnitStats(Unit unit)
        {
            Debug.Assert(unit != null);

            this.selectedUnit = unit;
            this.state = State.ShowingUnitStats;
            this.uxUnitStats.SetUnitStats(unit);
            this.SetControlVisible(this.uxBackButton, !this.HireUnitMode);
            this.SetControlVisible(this.uxUnitStats, true);
            this.SetControlVisible(this.uxUnitList, false);
            this.SetControlVisible(this.uxInventoryButton, true);
            this.SetControlVisible(this.uxUnitInventory, false);
            this.SetControlVisible(this.uxUnitAttributes, false);
            this.SetControlVisible(this.uxAttributesButton, true);
            this.SetControlVisible(this.uxHireButton, this.HireUnitMode);
            this.SetControlVisible(this.uxGiveButton, false);
        }

        #endregion

        public void SetUnits(List<Unit> units)
        {
            this.uxUnitList.Clear();

            foreach (Unit unit in units)
            {
                IconInfo icon = unit.GetEntityIcon();
                TooltipButtonAndTextControl newButton = new TooltipButtonAndTextControl(icon, unit.DisplayName, 200);
                newButton.TooltipText = unit.DisplayName;
                newButton.Tag = unit;
                newButton.Pressed += this.HandleUnitClicked;                                            
                this.uxUnitList.AddControl(newButton, false);                
            }

            this.uxUnitList.RefreshControls();
        }
    }

    public partial class UnitDialog
    {
        private void InitializeComponent()
        {
            this.uxClose = new ButtonControl();
            this.uxHireButton = new ButtonControl();
            this.uxBackButton = new ButtonControl();
            this.uxInventoryButton = new ButtonControl();
            this.uxAttributesButton = new ButtonControl();
            this.uxUnitList = new ScrollableControlList(200, 32);
            this.uxUnitStats = new UnitStatsGroup();
            this.uxUnitInventory = new UnitInventoryGroup();
            this.uxUnitAttributes = new UnitAttributeGroup();
            this.uxGiveButton = new ButtonControl();

            this.uxBackButton.Text = "Back";
            this.uxBackButton.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(1.0f, -26.0f), 40, 20);
            this.uxBackButton.Pressed += this.HandleBackClicked;

            this.uxInventoryButton.Text = "Inventory";
            this.uxInventoryButton.Bounds = new UniRectangle(new UniScalar(0.0f, this.uxBackButton.Bounds.Right.Offset + 6.0f), new UniScalar(1.0f, -26.0f), 80, 20);
            this.uxInventoryButton.Pressed += this.HandleInventoryClicked;

            this.uxGiveButton.Text = "Give";
            this.uxGiveButton.Bounds = new UniRectangle(new UniScalar(0.0f, this.uxBackButton.Bounds.Right.Offset + 6.0f), new UniScalar(1.0f, -26.0f), 80, 20);
            this.uxGiveButton.Pressed += this.HandleGiveButtonClicked;

            this.uxAttributesButton.Text = "Attributes";
            this.uxAttributesButton.Bounds = new UniRectangle(new UniScalar(0.0f, this.uxInventoryButton.Bounds.Right.Offset + 6.0f), new UniScalar(1.0f, -26.0f), 80, 20);
            this.uxAttributesButton.Pressed += this.HandleAttributesClicked;

            this.uxClose.Text = "Close";
            this.uxClose.Bounds = new UniRectangle(new UniScalar(1.0f, -46.0f), new UniScalar(1.0f, -26.0f), 40, 20);
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.uxHireButton.Text = "Hire";
            this.uxHireButton.Bounds = new UniRectangle(this.uxClose.Bounds.Left - 46.0f, this.uxClose.Bounds.Top, 40, 20);
            this.uxHireButton.Pressed += this.HandleHireButtonPressed;

            this.Bounds = new UniRectangle(100.0f, 30.0f, 400.0f, 400.0f);
            this.uxUnitList.Bounds = new UniRectangle(20.0f, 40.0f, 360.0f, 200.0f);
            this.uxUnitStats.Bounds = new UniRectangle(0.0f, 20.0f, 400.0f, 350.0f);
            this.uxUnitInventory.Bounds = new UniRectangle(0.0f, 20.0f, 400.0f, 350.0f);
            this.uxUnitAttributes.Bounds = new UniRectangle(0.0f, 20.0f, 400.0f, 350.0f);            

            this.Children.Add(this.uxClose);
            this.Children.Add(this.uxUnitList);            
        }        

        protected ButtonControl uxClose;
        protected ButtonControl uxBackButton;
        protected ButtonControl uxInventoryButton;
        protected ButtonControl uxAttributesButton;
        protected ButtonControl uxHireButton;
        protected ButtonControl uxGiveButton;

        protected ScrollableControlList uxUnitList;
        protected UnitStatsGroup uxUnitStats;
        protected UnitInventoryGroup uxUnitInventory;
        protected UnitAttributeGroup uxUnitAttributes;        
    }
}



