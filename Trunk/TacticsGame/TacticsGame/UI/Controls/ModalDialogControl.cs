using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using TacticsGame.UI.Dialogs;

namespace TacticsGame.UI.Controls
{   
    public abstract class ModalDialogControl : WindowControl, ICanBeClosed, ICloseOnRightClick
    {       
        public event EventHandler CloseClicked;

        public ModalDialogControl()
        {
            this.ModalZOrder = InterfaceManager.Instance.GetNextZOrder();
        }

        public virtual bool CloseOnRightClick
        {
            get { return true; }
        }

        protected virtual void HandleCloseClicked(object sender, EventArgs e)
        {
            InterfaceManager.Instance.RevertZOrder();

            if (this.CloseClicked != null)
            {
                this.CloseClicked(this, e);
            }
        }

        public virtual void CloseThisDialog()
        {
            this.HandleCloseClicked(this, new EventArgs());
        }

        protected override void OnMousePressed(Nuclex.Input.MouseButtons button)
        {
            if (button == Nuclex.Input.MouseButtons.Right && this.CloseOnRightClick)
            {
                this.CloseThisDialog();
            }
            else
            {
                base.OnMousePressed(button);
            }
        }        
    }


}
