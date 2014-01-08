using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using TacticsGame.Items;
using Nuclex.UserInterface;
using TacticsGame.UI.Controls;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.UserInterface.Controls;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Owners;
using System.Diagnostics;
using TacticsGame.Managers;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Visitors;
using TacticsGame.GameObjects.Visitors.Types;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.UI.Dialogs
{
    public partial class VisitorsDialog : ModalDialogControl
    {
        private Building building = null;

        public VisitorsDialog()
            : base()
        {            
            this.InitializeComponent();            
        }

        public event EventHandler<EventArgsEx<Unit>> HiredUnit;

        /// <summary>
        /// Create an instance of this dialog and make it visible.
        /// </summary>
        /// <returns></returns>
        public static VisitorsDialog CreateDialog(bool addToInterfaceAutomatically = true) 
        {            
            VisitorsDialog newDialog = new VisitorsDialog();
            if (addToInterfaceAutomatically)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }
            
            newDialog.BringToFront();
            newDialog.AffectsOrdering = false;

            return newDialog;
        }

        private void HandleHiredVisitor(object sender, EventArgsEx<Unit> e)
        {
            if (this.HiredUnit != null)
            {
                this.HiredUnit(this, e);
            }

            this.SetVisitors(building);
        }

        private void HandleItemClicked(object sender, EventArgs e)
        {
            DecisionMakingUnit visitor = (DecisionMakingUnit)((TooltipButtonControl)sender).Tag;

            if (visitor is Merchant)
            {
                StockDialog dialog = StockDialog.CreateDialog();
                dialog.SetStockOwner(visitor);
            }
            else if (visitor is Traveller)
            {
                string message = ((Traveller)visitor).GenerateMessage();
                MessageDialog dialog = MessageDialog.CreateDialog(message);                
            }
            else if (visitor is HireableUnit)
            {
                HireableUnit unit = (HireableUnit)visitor;
                UnitDialog dialog = UnitDialog.CreateDialog(null, unit, true);
                dialog.HiredUnit += new EventHandler<EventArgsEx<Unit>>(HandleHiredVisitor);
            }
        }

        public void SetVisitors(Building building)
        {
            this.building = building;

            this.uxVisitorWindow.Clear();

            Debug.Assert(building.IsBuildingWithVisitors && building.Visitors != null && building.Visitors.Count > 0);

            foreach (DecisionMakingUnit visitor in building.Visitors)
            {
                IconInfo icon = visitor.GetEntityIcon();
                TooltipButtonControl newButton = new TooltipButtonControl();
                newButton.TooltipText = visitor.DisplayName;
                newButton.Bounds = new UniRectangle(0, 0, icon.Dimensions, icon.Dimensions);
                newButton.Tag = visitor;
                newButton.Pressed += this.HandleItemClicked;
                newButton.SetIcon(icon);
                this.uxVisitorWindow.AddControl(newButton, false);
            }

            this.uxVisitorWindow.RefreshControls();
        }
    }

    public partial class VisitorsDialog
    {
        private void InitializeComponent()
        {
            this.uxLabel.Bounds = new UniRectangle(6.0f, 26.0f, 100.0f, 20.0f);
            this.uxLabel.Text = "Visitors:";
            
            this.uxVisitorWindow = new ScrollableControlList(32, 32);
            this.uxVisitorWindow.Bounds = new UniRectangle(6.0f, 52.0f, 388.0f, 130.0f);

            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Close";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.Bounds = new UniRectangle(100.0f, 100.0f, 400.0f, 300.0f);

            this.Children.Add(this.uxClose);
            this.Children.Add(this.uxLabel);
            this.Children.Add(this.uxVisitorWindow);
        }

        protected BetterLabelControl uxLabel = new BetterLabelControl();
        protected ButtonControl uxClose;
        protected ScrollableControlList uxVisitorWindow;
    }
}


