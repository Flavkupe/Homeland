using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.GameObjects;
using Nuclex.UserInterface.Controls.Arcade;
using Nuclex.UserInterface.Controls;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.UI.Panels;
using TacticsGame.GameObjects.EntityMetadata;
using System.Diagnostics;
using TacticsGame.Managers;
using TacticsGame.PlayerThings;
using TacticsGame.GameObjects.Buildings.Types;
using TacticsGame.Items;

namespace TacticsGame.UI
{
    /// <summary>The Command dialog on the right of the screen.</summary>
    public partial class CommandPane : PanelControl
    {
        /// <summary>The Command dialog on the right of the screen.</summary>
        public CommandPane()
        {
            this.Width = 200;
            this.Height = GameStateManager.Instance.CameraView.Height;
            InitializeComponent();
        }

        public int Width { get; set; }
        public int Height { get; set; }        

        /// <summary>
        /// The continue button gets clicked.
        /// </summary>
        public event EventHandler ContinueButtonClicked;

        public event EventHandler<CommandPaneCommandEventArgs> CommandButtonClicked;

        /// <summary>
        /// Build button from main management menu clicked.
        /// </summary>
        public event EventHandler<BuildingToBuildIconClickedEventArgs> BuildButtonClicked;

        public event EventHandler<UnitAbilitiesPanel.AbilityClickedEventArgs> AbilityClicked;
        

        #region Handlers
        
        private void HandleContinueButtonClicked(object sender, EventArgs e)
        {
            if (this.ContinueButtonClicked != null)
            {
                this.ContinueButtonClicked(sender, e);
            }
        }

        protected void HandleAbilityClicked(object sender, UnitAbilitiesPanel.AbilityClickedEventArgs e)
        {
            if (this.AbilityClicked != null)
            {
                this.AbilityClicked(this, e);
            }
        }

        protected void HandleBuildClicked(object sender, EventArgs e)
        {
            this.SetControlVisible(this.uxBuildingConstructionPanel, true);
            this.SetControlVisible(this.uxBuildingCommandsPanel, false);
        }

        protected void HandleBuildBuildingIconClicked(object sender, BuildingToBuildIconClickedEventArgs e)
        {
            Debug.Assert(e.Building is IBuildable);
            this.SetDraggingBuildingDisplay(e.Building as IBuildable, PlayerStateManager.Instance.PlayerInventory);

            if (this.BuildButtonClicked != null)
            {
                this.BuildButtonClicked(this, e);
            }
        }

        protected void HandleBuildingCommandClicked(object sender, CommandPaneCommandEventArgs e)
        {
            if (e.Command == Commands.BuildClicked)
            {
                this.HandleBuildClicked(sender, e);
            }
            else
            {
                if (this.CommandButtonClicked != null)
                {
                    this.CommandButtonClicked(this, e);
                }
            }
        }

        /// <summary>
        /// The button on the bottom-right.
        /// </summary>
        public ButtonControl ContinueButton
        {
            get { return this.uxNextTurnButton; }            
        }
        

        #endregion

        public string SelectionText
        {
            get { return this.uxSelectionLabel.Text; }
            set { this.uxSelectionLabel.Text = value; }
        }


        public void ClearUnitDisplay()
        {
            this.uxSelectionLabel.Text = string.Empty;
            this.uxSelectedIcon.ImageTexture = null;
            this.SetControlVisible(this.uxSelectedIcon, false);
            this.SetControlVisible(this.uxUnitStatsPanel, false);
            this.SetControlVisible(this.uxUnitAbilitiesPanel, false);
            this.SetControlVisible(this.uxBuildingCommandsPanel, false);
            this.SetControlVisible(this.uxBuildingDetailsPanel, false);
            this.SetControlVisible(this.uxBuildingConstructionPanel, false);
        }

        public bool MouseIsOverControl()
        {
            return this.MouseOverControl != null;
        }

        public void SetSelectedEntityDisplay(GameEntity selectedEntity)
        {
            if (selectedEntity == null)
            {                
                return;
            }

            this.ClearUnitDisplay();

            this.uxSelectionLabel.Text = selectedEntity.DisplayName;
            IconInfo icon = selectedEntity.Sprite.GetEntityIcon();
            this.uxSelectedIcon.ImageTexture = icon.SheetImage;
            this.uxSelectedIcon.ImageClip = icon.Clip;

            this.SetControlVisible(this.uxSelectedIcon, true);

            if (selectedEntity is Unit)
            {
                this.SetSelectedUnitDisplay(selectedEntity as Unit);
            }
            else
            {
                this.SetControlVisible(this.uxUnitStatsPanel, false);
            }
        }

        public void SetSelectedBuildingDisplay(Building building) 
        {
            this.SetSelectedEntityDisplay(building);

            this.SetControlVisible(this.uxBuildingCommandsPanel, true);
            this.SetControlVisible(this.uxBuildingDetailsPanel, true);
            this.SetControlVisible(this.uxBuildingConstructionPanel, false);
            this.uxBuildingCommandsPanel.SetSelectedBuilding(building);
            this.uxBuildingDetailsPanel.SetSelectedBuilding(building);
            this.uxBuildingDetailsPanel.BringToFront();
        }

        public void SetDraggingBuildingDisplay(IBuildable building, Inventory playerInventory)
        {            
            this.uxBuildingDetailsPanel.SetBuildingForConstruction(building, playerInventory);
        }

        private void SetSelectedUnitDisplay(Unit unit)
        {
            this.SetControlVisible(this.uxUnitStatsPanel, true);            
            this.uxUnitStatsPanel.SetSelectedUnitDisplay(unit);

            if (unit.PlayerCanCommand)
            {
                this.SetControlVisible(this.uxUnitAbilitiesPanel, true);
                this.uxUnitAbilitiesPanel.SetSelectedUnitDisplay(unit);
            }
            else
            {
                this.SetControlVisible(this.uxUnitAbilitiesPanel, false);
            }
        }

        /// <summary>
        /// Gets the shop command panel
        /// </summary>
        public BuildingCommandsPanel BuildingCommands
        {
            get { return this.uxBuildingCommandsPanel; }
        }

        /// <summary>
        /// Shows the guild commands, being the "main" commands.
        /// </summary>
        /// <param name="guildHouse"></param>
        public void ShowGuildButtonGroup(GuildHouse guildHouse)
        {
            this.SetSelectedBuildingDisplay(guildHouse);
        }
    }        
   
    public partial class CommandPane
    {
        /// <summary> 
        ///   Required method for user interface initialization -
        ///   do modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uxSelectionLabel = new Nuclex.UserInterface.Controls.LabelControl();
            this.uxSelectedIcon = new ButtonControl();
            this.uxNextTurnButton = new ButtonControl();
            this.uxUnitStatsPanel = new UnitStatsPanel();
            this.uxUnitAbilitiesPanel = new UnitAbilitiesPanel();
            this.uxBuildingCommandsPanel = new BuildingCommandsPanel();
            this.uxBuildingDetailsPanel = new BuildingDetailsPanel();
            this.uxBuildingConstructionPanel = new BuildingConstructionPanel();

            this.uxUnitStatsPanel.Bounds = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(0.2f, 0.0f), 200.0f, 150.0f);            

            this.uxUnitAbilitiesPanel.AbilityClicked += this.HandleAbilityClicked;

            this.uxBuildingCommandsPanel.BuildingCommandTriggered += this.HandleBuildingCommandClicked;

            this.uxBuildingConstructionPanel.BuildBuildingIconClicked += this.HandleBuildBuildingIconClicked;

            this.uxSelectionLabel.Text = string.Empty;
            this.uxSelectionLabel.Bounds = new UniRectangle(38.0f, 26.0f, 110.0f, 30.0f);                      

            this.uxNextTurnButton.Text = "Continue";
            this.uxNextTurnButton.Bounds = new UniRectangle(new UniScalar(0.0f, 10.0f), new UniScalar(1.0f, -38.0f), 180, 32);
            this.uxNextTurnButton.Pressed += this.HandleContinueButtonClicked;

            this.uxSelectedIcon.Text = string.Empty;
            this.uxSelectedIcon.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 26.0f), 32, 32);
            this.uxSelectedIcon.Enabled = false;

            this.Bounds = new UniRectangle(new UniScalar(1.0f, -200.0f), new UniScalar(0.0f, 0.0f), (float)this.Width, (float)this.Height);

            Children.Add(this.uxNextTurnButton);
            Children.Add(this.uxSelectionLabel);            
        }     

        /// <summary>A label used to display a 'hello world' message</summary>
        protected Nuclex.UserInterface.Controls.LabelControl uxSelectionLabel;

        protected Nuclex.UserInterface.Controls.Desktop.ButtonControl uxSelectedIcon;

        protected UnitStatsPanel uxUnitStatsPanel;

        protected UnitAbilitiesPanel uxUnitAbilitiesPanel;

        protected BuildingCommandsPanel uxBuildingCommandsPanel;
        protected BuildingDetailsPanel uxBuildingDetailsPanel;
        protected BuildingConstructionPanel uxBuildingConstructionPanel;

        protected Nuclex.UserInterface.Controls.Desktop.ButtonControl uxNextTurnButton;

        
    }

    public class CommandPaneCommandEventArgs : EventArgs
    {
        public Commands Command { get; private set; }

        public CommandPaneCommandEventArgs(Commands command)
        {
            this.Command = command;
        }
    }

    public enum Commands
    {
        UnitsButtonClicked,
        ShowStockClicked,
        BuildClicked,
        RequestsClicked,
        VisitorsClicked,
        FinancesClicked,
        SellClicked,
        EdictsClicked,
        CaravanClicked,
    }
}
