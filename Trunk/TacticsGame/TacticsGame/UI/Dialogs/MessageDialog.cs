using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using TacticsGame.UI.Groups;
using TacticsGame.Managers;

namespace TacticsGame.UI.Dialogs
{
    /// <summary>
    /// A dialog to show any message
    /// </summary>
    public class MessageDialog : ModalDialogControl
    {
        public string message = null;
        private const int MagicNumber = 30;
        private bool yesOrNoPromptMode = false;        

        private MessageDialog()
            : base()
        {
            this.InitializeComponents();
        }

        public event EventHandler<EventArgsEx<MessageDialogResult>> ButtonResult; 
        
        public bool YesOrNoPromptMode
        {
            get { return this.yesOrNoPromptMode; }
            set
            {
                this.yesOrNoPromptMode = value;
                if (value)
                {
                    this.SetControlVisible(this.uxYesButton, value);
                    this.uxCloseButton.Text = value ? "No" : "Close";
                }
            }
        }

        /// <summary>
        /// Creates an instance of the message box.
        /// </summary>
        /// <param name="addToInterface"></param>
        /// <returns></returns>
        public static MessageDialog CreateDialog(string message, bool addToInterface = true) 
        {
            MessageDialog newDialog = new MessageDialog();
            if (addToInterface)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            newDialog.SetMessage(message);
            return newDialog;
        }

        protected override void HandleCloseClicked(object sender, EventArgs e)
        {
            if (this.ButtonResult != null)
            {
                this.ButtonResult(this, new EventArgsEx<MessageDialogResult>(this.yesOrNoPromptMode ? MessageDialogResult.No : MessageDialogResult.Close));
            }

            base.HandleCloseClicked(sender, e);
        }

        private void HandleYesButtonPressed(object sender, EventArgs e)
        {
            if (this.ButtonResult != null)
            {
                this.ButtonResult(this, new EventArgsEx<MessageDialogResult>(MessageDialogResult.Yes));
            }

            this.CloseThisDialog();
        }

        private void SetMessage(string message)
        {
            this.message = message;            

            int index = 0;
            int lengthCovered = 0;
            while (index < message.Length)
            {
                BetterLabelControl label = new BetterLabelControl();
                int length = Math.Min(MagicNumber, message.Length - index);
                string substring = message.Substring(index, length);

                int trueLineEndIndex = -1;
                if (index + length < message.Length - 1)
                {
                    trueLineEndIndex = substring.LastIndexOf(" ");
                    if (trueLineEndIndex == substring.Length - 1) { trueLineEndIndex = -1; } // it's the last char in the substring... ignore it
                    if (trueLineEndIndex != -1)
                    {
                        trueLineEndIndex = lengthCovered + trueLineEndIndex;
                        length = trueLineEndIndex - index;

                        // Try to end where there's a space
                        substring = message.Substring(index, length);                    
                    }
                }

                lengthCovered += length;                
                label.Text = substring.TrimStart();

                label.Bounds = new UniRectangle(0, 0, 200, 20);
                this.uxMessage.AddControl(label);
                
                index = trueLineEndIndex == -1 ? index + MagicNumber : trueLineEndIndex + 1;
                if (trueLineEndIndex != -1)
                {
                    lengthCovered++; // Skipped a space.
                }
            }            
        }        

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(200, 200, 250, 250);

            this.uxMessage = new FlowPanelControl(new UniRectangle(12.0f, 26.0f, 250, 200));            
            //this.uxMessage.Bounds = new UniRectangle(6, 26, 238, 192);

            this.uxCloseButton.Bounds = new UniRectangle(new UniScalar(1.0f, -56.0f), new UniScalar(1.0f, -26.0f), 50, 20);
            this.uxCloseButton.Pressed += this.HandleCloseClicked;
            this.uxCloseButton.Text = "Close";

            this.uxYesButton.Bounds = new UniRectangle(this.uxCloseButton.Bounds.Left - 56.0f, this.uxCloseButton.Bounds.Top, 50, 20);
            this.uxYesButton.Pressed += this.HandleYesButtonPressed;
            this.uxYesButton.Text = "Yes";

            this.Children.Add(uxMessage);
            this.Children.Add(uxCloseButton);
        }        

        ButtonControl uxCloseButton = new ButtonControl();
        ButtonControl uxYesButton = new ButtonControl();
        FlowPanelControl uxMessage;

    }

    public enum MessageDialogResult
    {
        Yes,
        No,
        Close,
    }
}
