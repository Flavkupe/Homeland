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
using TacticsGame.GameObjects.Units;

namespace TacticsGame.UI.Dialogs
{
    public partial class StockDialog : ModalDialogControl
    {
        private IMakeDecisions seller = null;

        private PlayerItemBidDialog bidDialog = null; 

        private StockDialog()
        {            
            this.InitializeComponent();            
        }

        public static StockDialog CreateDialog(bool showAutomatically = true) 
        {
            StockDialog newDialog = new StockDialog();
            if (showAutomatically)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            newDialog.BringToFront();
            newDialog.AffectsOrdering = false;
            
            return newDialog;
        }

        private void HandleItemClicked(object sender, EventArgs e)
        {
            Debug.Assert(this.seller != null);

            TooltipButtonControl button = (TooltipButtonControl)sender;
            Item item = (Item)button.Tag;

            this.bidDialog = PlayerItemBidDialog.CreateDialog(item, this.seller, true);
            this.bidDialog.BeforeDialogClosed += this.HandlePlayerBidDialogClosed;                        
            this.bidDialog.BringToFront();
        }

        private void HandleSellButtonPressed(object sender, EventArgs e)
        {
            PlayerStockDialog dialog = PlayerStockDialog.CreateDialog(true, PlayerStockDialogMode.Sell);
            dialog.TargetUnit = this.seller;
            dialog.CloseClicked += this.HandleSellDialogCloseClicked;
        }

        private void HandleSellDialogCloseClicked(object sender, EventArgs e)
        {
            this.SetStockOwner(this.seller);
        }

        private void HandlePlayerBidDialogClosed(object sender, EventArgs e)
        {
            this.Children.Remove(this.bidDialog);
            this.SetStockOwner(this.seller);
        }

        private void SetMoney(int money)
        {
            this.uxMoney.Text = string.Format("Money: {0}", money);
        }

        public void SetStockOwner(IMakeDecisions owner)
        {
            Inventory items = owner.Inventory;
            this.seller = owner;

            this.uxItemWindow.Clear();

            this.SetMoney(items.Money);

            foreach (Item item in items.Items)
            {
                IconInfo icon = item.Icon;
                TooltipButtonControl newButton = new TooltipButtonControl();
                newButton.TooltipText = item.DisplayName;
                newButton.Bounds = new UniRectangle(0, 0, icon.Dimensions, icon.Dimensions);
                newButton.Pressed += this.HandleItemClicked;
                newButton.Tag = item;
                newButton.SetIcon(icon);
                this.uxItemWindow.AddControl(newButton, false);
            }

            this.uxItemWindow.RefreshControls();
        }
    }

    public partial class StockDialog
    {
        private void InitializeComponent()
        {
            this.uxMoney = new LabelControl();
            this.uxMoney.Bounds = new UniRectangle(6.0f, 26.0f, 200.0f, 30.0f);

            this.uxItemWindow = new ScrollableControlList(32, 32);
            this.uxItemWindow.Bounds = new UniRectangle(6.0f, 52.0f, 388.0f, 130.0f);

            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Close";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.uxSellButton = new ButtonControl();
            this.uxSellButton.Text = "Sell";
            this.uxSellButton.Bounds = new UniRectangle(new UniVector(6.0f, new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxSellButton.Pressed += this.HandleSellButtonPressed;

            this.Bounds = new UniRectangle(100.0f, 100.0f, 400.0f, 300.0f);

            this.Children.Add(this.uxMoney);
            this.Children.Add(this.uxClose);
            this.Children.Add(this.uxItemWindow);
            this.Children.Add(this.uxSellButton);
        }

        protected ButtonControl uxClose;
        protected ButtonControl uxSellButton;
        protected ScrollableControlList uxItemWindow;
        protected LabelControl uxMoney;
    }
}


