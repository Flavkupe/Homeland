using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using TacticsGame.UI.Groups;
using TacticsGame.Edicts;
using Nuclex.UserInterface.Controls;
using TacticsGame.Managers;
using Microsoft.Xna.Framework;

namespace TacticsGame.UI.Dialogs
{
    public class EdictsDialog : ModalDialogControl
    {
        private EdictsDialog()
            : base()
        {
            this.InitializeComponents();
        }        

        public static EdictsDialog CreateDialog(bool viewByDefault = true)
        {
            EdictsDialog newDialog = new EdictsDialog();

            if (viewByDefault)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            return newDialog;
        }

        public void SetEdicts()
        {
            this.uxButtons.Clear();

            int current = PlayerStateManager.Instance.ActiveTown.CurrentEdictCount;
            int max = PlayerStateManager.Instance.ActiveTown.MaxEdicts;

            this.uxAmountLabel.Text = string.Format("{0} / {1}", current, max);
            
            this.uxAmountLabel.LabelColor = current == max ? (Color?)Color.Red : null;
            
            foreach (string edictName in Enum.GetNames(typeof(EdictType)))
            {
                Edict edict = new Edict(edictName);
                TooltipButtonAndTextControl button = new TooltipButtonAndTextControl(edict.Icon, edict.DisplayName, 188, 32);
                button.ShowFrameOnImageButton = true;

                if (PlayerStateManager.Instance.ActiveTown.EdictIsActive(edict.Type))
                {
                    button.SetSubtexture(TextureManager.Instance.GetTextureInfo("Scroll", ResourceType.MiscObject), false);                    
                }

                button.Tag = edict;
                button.Pressed += this.EdictButtonPressed;
                this.uxButtons.AddControl(button);
            }
        }

        public void EdictButtonPressed(object sender, EventArgs e)
        {
            int current = PlayerStateManager.Instance.ActiveTown.CurrentEdictCount;
            int max = PlayerStateManager.Instance.ActiveTown.MaxEdicts;

            TooltipButtonAndTextControl button = (TooltipButtonAndTextControl)sender;
            Edict edict = (Edict)button.Tag;

            bool active = PlayerStateManager.Instance.ActiveTown.EdictIsActive(edict.Type);

            if (!active && current >= max)
            {
                // Reached max
                return;
            }

            PlayerStateManager.Instance.ActiveTown.ToggleEdict(edict.Type, !active);

            this.SetEdicts();
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(100, 100, 588, 300);

            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Close";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.uxAmountLabel = new BetterLabelControl();
            this.uxAmountLabel.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -126.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));

            this.uxButtons = new FlowPanelControl(new UniRectangle(6.0f, 26.0f, 576.0f, 238.0f));

            this.Children.Add(this.uxAmountLabel);
            this.Children.Add(this.uxButtons);
            this.Children.Add(this.uxClose);
        }

        private ButtonControl uxClose;
        private BetterLabelControl uxAmountLabel;
        private FlowPanelControl uxButtons;
    }
}
