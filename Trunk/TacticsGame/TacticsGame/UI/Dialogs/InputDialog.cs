using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using TacticsGame.Managers;

namespace TacticsGame.UI.Dialogs
{
    public class InputDialog : ModalDialogControl
    {
        private InputDialog()
            : base()
        {
            this.InitializeControls();
        }

        public event EventHandler<EventArgsEx<string>> InputResult; 

        public static InputDialog CreateDialog(bool addToInterface = true)
        {
            InputDialog newDialog = new InputDialog();
            if (addToInterface)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            newDialog.BringToFront();
            return newDialog;

        }

        public string Input
        {
            get { return this.uxInput.Text; }
            set { this.uxInput.Text = value; }
        }

        private void InitializeControls()
        {
            this.Bounds = new UniRectangle(new UniScalar(0.3f, 0.0f), new UniScalar(0.5f, 0.0f), 160, 60);
            this.uxInput.Bounds = new UniRectangle(6, 6, 148, 20);
            
            this.uxCloseButton.Bounds = new UniRectangle(new UniScalar(1.0f, -56.0f), new UniScalar(1.0f, -26.0f), 50, 20);
            this.uxCloseButton.Pressed += this.HandleCloseClicked;
            this.uxCloseButton.Text = "Cancel";

            this.uxOKButton.Bounds = new UniRectangle(6.0f, new UniScalar(1.0f, -26.0f), 50, 20);
            this.uxOKButton.Pressed += this.HandleOKButtonPressed;
            this.uxOKButton.Text = "OK";

            this.Children.Add(this.uxInput);
            this.Children.Add(this.uxOKButton);
            this.Children.Add(this.uxCloseButton);
        }

        private void HandleOKButtonPressed(object sender, EventArgs e)
        {
            this.CloseThisDialog();

            if (this.InputResult != null)
            {
                this.InputResult(this, new EventArgsEx<string>(this.uxInput.Text));                
            }
        }        

        private InputControl uxInput = new InputControl();
        private ButtonControl uxCloseButton = new ButtonControl();
        private ButtonControl uxOKButton = new ButtonControl();        
    }
}
