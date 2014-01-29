using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.PlayerThings;
using TacticsGame.UI.Groups;
using Nuclex.UserInterface.Controls;
using TacticsGame.Managers;

namespace TacticsGame.UI.Dialogs
{
    public class GameInitDialog : ModalDialogControl
    {
        SelectedObjective objective = SelectedObjective.NoneSelected;

        private string selectedClass = null;        

        private GameInitDialog()
            : base()
        {
            this.InitializeComponents();
        }

        public override bool CloseOnRightClick { get { return false; } }

        public static GameInitDialog CreateDialog(bool viewByDefault = true)
        {
            GameInitDialog newDialog = new GameInitDialog();

            if (viewByDefault)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            return newDialog;
        }

        protected override void HandleCloseClicked(object sender, EventArgs e)
        {            
            GameObjective objective = new GameObjective();
            objective.TargetDays = this.objective == SelectedObjective.Monetary ? 10 : 5;
            objective.TargetGold = this.objective == SelectedObjective.Monetary ? (int?)10000 : null;
            GameStateManager.Instance.GameObjective = objective;
            PlayerStateManager.Instance.Player = new Player(this.selectedClass, this.uxNameTextbox.Text);

            base.HandleCloseClicked(sender, e);
        }

        private void SetAvailableClassButtons()
        {
            string[] classes = new string[] { "Fool", "Ranger", "Footman" };

            this.uxClassSelectionGroup.Clear();

            foreach (string unitClass in classes)
            {
                IconInfo icon = TextureManager.Instance.GetIconInfo(unitClass);
                TooltipButtonControl button = new TooltipButtonControl();
                button.TooltipText = unitClass;                
                button.SetIcon(icon);
                button.Bounds = new UniRectangle(0, 0, 32, 32);
                button.Pressed += new EventHandler(ClassSelected);
                button.Tag = unitClass;
                this.uxClassSelectionGroup.AddControl(button);
            }
        }

        private void ClassSelected(object sender, EventArgs e)
        {
            foreach (Control control in this.uxClassSelectionGroup.Children)
            {
                if (control is TooltipButtonControl) 
                {
                    (control as TooltipButtonControl).MarkSelected = false;
                }
            }

            TooltipButtonControl button = (TooltipButtonControl)sender;
            this.selectedClass = button.Tag as string;
            button.MarkSelected = true;

            this.CheckOKCondition();
        }
            

        private void ConditionPressed(object sender, EventArgs e)
        {
            this.SetControlVisible(uxCheckmark, false);
            this.uxClose.Enabled = false;
            this.uxInstructions.Text = string.Empty;
            this.objective = SelectedObjective.NoneSelected;

            if (sender == this.uxMonetaryCondition)
            {
                this.uxCheckmark.Bounds = this.uxCheckmark.Bounds.RelocateClone(this.uxMonetaryCondition.Bounds.Right.Offset + 6.0f, this.uxMonetaryCondition.Bounds.Top.Offset + 9.0f);
                this.SetControlVisible(uxCheckmark, true);
                this.uxInstructions.Text = "Have 10000 gold by day 10.";
                this.objective = SelectedObjective.Monetary;
            }
            else if (sender == this.uxSurvivalCondition)
            {
                this.uxCheckmark.Bounds = this.uxCheckmark.Bounds.RelocateClone(this.uxSurvivalCondition.Bounds.Right.Offset + 6.0f, this.uxSurvivalCondition.Bounds.Top.Offset + 9.0f);
                this.SetControlVisible(uxCheckmark, true);
                this.uxInstructions.Text = "Survive until day 5.";
                this.objective = SelectedObjective.Survival;
            }

            this.CheckOKCondition();
        }

        private void CheckOKCondition()
        {
            this.uxClose.Enabled = false;

            if (this.objective != SelectedObjective.NoneSelected && !string.IsNullOrWhiteSpace(this.uxNameTextbox.Text) && this.selectedClass != null)
            {
                this.uxClose.Enabled = true;
            }
        }

        public void NameTextChanged(object sender, EventArgs e)
        {
            this.CheckOKCondition();
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(100, 100, 300, 300);

            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Start";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;
            this.uxClose.Enabled = false;

            IconInfo monetaryIcon = TextureManager.Instance.GetIconInfo("GoldCoins");
            IconInfo survivalIcon = TextureManager.Instance.GetTextureAsIconInfo("Skeleton", ResourceType.GameObject);
            IconInfo checkmarkIcon = TextureManager.Instance.GetIconInfo("Checkmark");

            this.uxNameLabel = new LabelControl();
            this.uxNameLabel.Bounds = new UniRectangle(6.0f, 35.0f, 50, 20);
            this.uxNameLabel.Text = "Name: ";

            this.uxNameTextbox = new InputControl();
            this.uxNameTextbox.Bounds = new UniRectangle(this.uxNameLabel.Bounds.Right.Offset + 6.0f, this.uxNameLabel.Bounds.Top.Offset, 150, 20);
            this.uxNameTextbox.TextChanged += this.NameTextChanged;
            this.uxNameTextbox.Text = "The Settler";

            this.uxClassLabel = new LabelControl();
            this.uxClassLabel.Bounds = new UniRectangle(6.0f, this.uxNameLabel.Bounds.Bottom.Offset + 15.0f, 40.0f, 20.0f);
            this.uxClassLabel.Text = "Class: ";

            this.uxClassSelectionGroup = new FlowPanelControl(new UniRectangle(uxClassLabel.Bounds.Right.Offset + 6.0f, this.uxNameLabel.Bounds.Bottom.Offset + 6.0f, 248, 44));

            this.uxMonetaryCondition = new TooltipButtonAndTextControl(monetaryIcon, "Monetary Victory", 200);
            this.uxMonetaryCondition.Bounds = new UniRectangle(6.0f, this.uxClassSelectionGroup.Bounds.Bottom.Offset + 6.0f, 200, 50);
            this.uxMonetaryCondition.Pressed += this.ConditionPressed;
            this.uxMonetaryCondition.ShowFrameOnImageButton = true;
            this.uxSurvivalCondition = new TooltipButtonAndTextControl(survivalIcon, "Survival Victory", 200);
            this.uxSurvivalCondition.Bounds = new UniRectangle(6.0f, this.uxMonetaryCondition.Bounds.Bottom.Offset + 6.0f, 200, 50);
            this.uxSurvivalCondition.Pressed += this.ConditionPressed;
            this.uxSurvivalCondition.ShowFrameOnImageButton = true;            

            this.uxCheckmark = new IconControl();
            this.uxCheckmark.Icon = checkmarkIcon;
            this.uxCheckmark.Bounds = new UniRectangle(0, 0, 32, 32);

            this.uxInstructions = new BetterLabelControl();
            this.uxInstructions.Bounds = new UniRectangle(6.0f, this.uxSurvivalCondition.Bounds.Bottom.Offset + 6.0f, 200, 20);

            this.Children.Add(this.uxNameLabel);
            this.Children.Add(this.uxNameTextbox);
            this.Children.Add(this.uxClassLabel);
            this.Children.Add(this.uxClassSelectionGroup);
            this.Children.Add(this.uxClose);
            this.Children.Add(this.uxInstructions);
            this.Children.Add(this.uxMonetaryCondition);
            this.Children.Add(this.uxSurvivalCondition);

            this.uxClassSelectionGroup.BringToFront();

            this.SetAvailableClassButtons();
        }

        private LabelControl uxNameLabel;
        private LabelControl uxClassLabel;
        private InputControl uxNameTextbox; 

        private ButtonControl uxClose;
        private FlowPanelControl uxClassSelectionGroup;
        private TooltipButtonAndTextControl uxMonetaryCondition;
        private TooltipButtonAndTextControl uxSurvivalCondition;
        private IconControl uxCheckmark;
        private BetterLabelControl uxInstructions;        

        private enum SelectedObjective
        {
            NoneSelected,
            Monetary,
            Survival,
        }
    }
}
