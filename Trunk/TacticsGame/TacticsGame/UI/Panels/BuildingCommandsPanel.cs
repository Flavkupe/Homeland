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
using TacticsGame.UI.Controls;
using TacticsGame.GameObjects.Buildings;
using Microsoft.Xna.Framework;
using TacticsGame.UI.Groups;
using TacticsGame.GameObjects.Buildings.Types;
using TacticsGame.PlayerThings;
using TacticsGame.Managers;

namespace TacticsGame.UI
{
    /// <summary>Panel for building controls.</summary>
    public partial class BuildingCommandsPanel : FramePanelControl
    {
        /// <summary>Initializes a new GUI demonstration dialog</summary>
        public BuildingCommandsPanel()
        {
            InitializeComponent();
        }

        public event EventHandler<CommandPaneCommandEventArgs> BuildingCommandTriggered;

        private void HandleVisitorButtonPressed(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.VisitorsClicked));
            }
        }

        private void HandleFinancesButtonPressed(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.FinancesClicked));
            }
        }

        private void HandleCaravanButtonPressed(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.CaravanClicked));
            }
        }

        private void HandleRequestsButtonPressed(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.RequestsClicked));
            }
        }

        private void HandleBuildClicked(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.BuildClicked));
            }
        }

        private void HandleUnitsButtonPressed(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.UnitsButtonClicked));
            }
        }

        private void HandleShowStockClicked(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.ShowStockClicked));
            }
        }

        private void HandleSellButtonPressed(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.SellClicked));
            }
        }

        private void HandleEdictsButtonPressed(object sender, EventArgs e)
        {
            if (this.BuildingCommandTriggered != null)
            {
                this.BuildingCommandTriggered(this, new CommandPaneCommandEventArgs(Commands.EdictsClicked));
            }
        }        

        private bool stockEnabled = false;
        public bool StockEnabled
        {
            get { return this.stockEnabled; }
            set
            {
                if (this.stockEnabled != value)
                {
                    this.SetControlVisible(this.uxShowStockButton, value);
                }

                this.stockEnabled = value;
            }
        }

        public void SetSelectedBuilding(Building building)
        {
            this.uxButtons.Clear();

            if (building.IsBuildingWithUnits)
            {
                this.uxButtons.AddControl(this.uxShowUnitsButton);
            }

            if (building.IsBuildingWithStock)
            {
                this.uxButtons.AddControl(this.uxShowStockButton);

                if (building.IsBuildingThatBuysThings)
                {
                    this.uxButtons.AddControl(this.uxSellButton);
                }
            }            

            if (building is GuildHouse)
            {
                this.uxButtons.AddControl(this.uxBuildBuildingControl);
                this.uxButtons.AddControl(this.uxOrdersButton);
                this.uxButtons.AddControl(this.uxCaravanButton);
                this.uxButtons.AddControl(this.uxFinancesButton);
                this.uxButtons.AddControl(this.uxEdictsButton);
            }

            if (building.IsBuildingWithVisitors && building.Visitors.Count > 0)
            {
                this.uxButtons.AddControl(this.uxVisitorsButton);
            }
        }
    }

    public partial class BuildingCommandsPanel
    {
        /// <summary> 
        ///   Required method for user interface initialization -
        ///   do modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // Init bounds here. They can be overriden.
            this.Bounds = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(0.0f, 60.0f), 200.0f, 200.0f);
            this.uxButtons = new FlowPanelControl(this.Bounds.RelocateClone(0,0), new Margin(4, 3, 3, 4));

            IconInfo stockIcon = TextureManager.Instance.GetIconInfo("stockIcon");
            this.uxShowStockButton = new TooltipButtonAndTextControl(stockIcon, "Stock", 94);
            this.uxShowStockButton.Pressed += this.HandleShowStockClicked;
            this.uxShowStockButton.ShowFrameOnImageButton = true;

            IconInfo unitsIcon = TextureManager.Instance.GetIconInfo("sampleIcon");
            this.uxShowUnitsButton = new TooltipButtonAndTextControl(unitsIcon, "Units", 94);
            this.uxShowUnitsButton.Pressed += this.HandleUnitsButtonPressed;
            this.uxShowUnitsButton.ShowFrameOnImageButton = true;

            IconInfo scrollIcon = TextureManager.Instance.GetIconInfo("Scroll1");
            this.uxOrdersButton = new TooltipButtonAndTextControl(scrollIcon, "Orders", 94);
            this.uxOrdersButton.Pressed += this.HandleRequestsButtonPressed;
            this.uxOrdersButton.ShowFrameOnImageButton = true;

            IconInfo caravanIcon = TextureManager.Instance.GetIconInfo("Scroll1");
            this.uxCaravanButton = new TooltipButtonAndTextControl(scrollIcon, "Caravan", 94);
            this.uxCaravanButton.Pressed += this.HandleCaravanButtonPressed;
            this.uxCaravanButton.ShowFrameOnImageButton = true;

            IconInfo coinIcon = TextureManager.Instance.GetIconInfo("GoldCoins");
            this.uxFinancesButton = new TooltipButtonAndTextControl(coinIcon, "Finance", 94);
            this.uxFinancesButton.Pressed += this.HandleFinancesButtonPressed;
            this.uxFinancesButton.ShowFrameOnImageButton = true;

            IconInfo sellIcon = TextureManager.Instance.GetIconInfo("Coin");
            this.uxSellButton = new TooltipButtonAndTextControl(sellIcon, "Sell", 94);
            this.uxSellButton.Pressed += this.HandleSellButtonPressed;
            this.uxSellButton.ShowFrameOnImageButton = true;

            IconInfo edictsIcon = TextureManager.Instance.GetIconInfo("Scroll");
            this.uxEdictsButton = new TooltipButtonAndTextControl(edictsIcon, "Edicts", 94);
            this.uxEdictsButton.Pressed += this.HandleEdictsButtonPressed;
            this.uxEdictsButton.ShowFrameOnImageButton = true;

            IconInfo buildingsIcon = TextureManager.Instance.GetTextureAsIconInfo("Building_Shop", ResourceType.GameObject);
            this.uxBuildBuildingControl = new TooltipButtonAndTextControl(buildingsIcon, "Build", 94, 32);
            this.uxBuildBuildingControl.Pressed += HandleBuildClicked;
            this.uxBuildBuildingControl.ShowFrameOnImageButton = true;

            IconInfo visitorIcon = TextureManager.Instance.GetIconInfo("ShopkeepIcon");
            this.uxVisitorsButton= new TooltipButtonAndTextControl(visitorIcon, "Visitors", 94, 32);
            this.uxVisitorsButton.Pressed += this.HandleVisitorButtonPressed;
            this.uxVisitorsButton.ShowFrameOnImageButton = true;

            this.Children.Add(this.uxButtons);
        }

        protected FlowPanelControl uxButtons;
        protected TooltipButtonAndTextControl uxShowStockButton;
        protected TooltipButtonAndTextControl uxShowUnitsButton;
        protected TooltipButtonAndTextControl uxBuildBuildingControl;
        protected TooltipButtonAndTextControl uxVisitorsButton;
        protected TooltipButtonAndTextControl uxOrdersButton;
        protected TooltipButtonAndTextControl uxCaravanButton;
        protected TooltipButtonAndTextControl uxFinancesButton;
        protected TooltipButtonAndTextControl uxSellButton;
        protected TooltipButtonAndTextControl uxEdictsButton;
    }
}
