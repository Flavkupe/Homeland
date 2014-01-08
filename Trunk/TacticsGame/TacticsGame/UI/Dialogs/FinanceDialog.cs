using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using TacticsGame.PlayerThings;
using System.Diagnostics;
using TacticsGame.Managers;

namespace TacticsGame.UI.Dialogs
{
    public class FinanceDialog : ModalDialogControl
    {
        private FinanceDialog()
            : base()
        {
            this.InitializeComponents();
        }

        public static FinanceDialog CreateDialog(bool viewByDefault = true) 
        {
            FinanceDialog newDialog = new FinanceDialog();

            if (viewByDefault)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            return newDialog;
        }

        public void SetActiveFinances()
        {
            this.uxTaxesInput.Text = PlayerStateManager.Instance.ActiveTown.DailyTaxes.ToString();
            this.uxVisitorTaxesInput.Text = PlayerStateManager.Instance.ActiveTown.VisitorTaxes.ToString();
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(100, 100, 200, 300);

            this.uxTaxesLabel.Bounds = new UniRectangle(6.0f, 26.0f, 80, 20);
            this.uxTaxesLabel.Text = "Taxes (%):";

            this.uxTaxesInput.Bounds = new UniRectangle(100.0f, this.uxTaxesLabel.Bounds.Location.Y, 60, 20);

            this.uxVisitorTaxesLabel.Bounds = new UniRectangle(6.0f, this.uxTaxesLabel.Bounds.Bottom.Offset + 6.0f, 80, 20);
            this.uxVisitorTaxesLabel.Text = "Visitor Taxes:";

            this.uxVisitorTaxesInput.Bounds = new UniRectangle(100.0f, this.uxVisitorTaxesLabel.Bounds.Location.Y, 60, 20);

            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Close";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.Children.Add(this.uxVisitorTaxesLabel);
            this.Children.Add(this.uxVisitorTaxesInput);
            this.Children.Add(this.uxTaxesLabel);
            this.Children.Add(this.uxTaxesInput);
            this.Children.Add(this.uxClose);
        }

        protected override void HandleCloseClicked(object sender, EventArgs e)
        {           
            int taxes;
            if (int.TryParse(this.uxTaxesInput.Text, out taxes))
            {
                PlayerStateManager.Instance.ActiveTown.DailyTaxes = taxes;    
            }

            int visitorTaxes;
            if (int.TryParse(this.uxVisitorTaxesInput.Text, out visitorTaxes))
            {
                PlayerStateManager.Instance.ActiveTown.VisitorTaxes = visitorTaxes;
            }

            base.HandleCloseClicked(sender, e);
        }

        private BetterLabelControl uxTaxesLabel = new BetterLabelControl();
        private InputControl uxTaxesInput = new InputControl();

        private BetterLabelControl uxVisitorTaxesLabel = new BetterLabelControl();
        private InputControl uxVisitorTaxesInput = new InputControl();

        private ButtonControl uxClose;
    }
}
