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
using TacticsGame.UI.Groups;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.UI.Dialogs
{
    public partial class PlayerStockDialog : ModalDialogControl
    {        
        public PlayerStockDialog()
            : base()
        {
            this.InitializeComponent();
            this.AffectsOrdering = false;
        }

        private PlayerStockDialogMode mode = PlayerStockDialogMode.View;

        /// <summary>
        /// Unit selected from outside for selling or giving stuff
        /// </summary>
        private IMakeDecisions targetUnit = null;

        /// <summary>
        /// Item selected for dialog
        /// </summary>
        private Item targetItem = null;

        /// <summary>
        /// Create an instance of this dialog and make it visible.
        /// </summary>
        /// <returns></returns>
        public static PlayerStockDialog CreateDialog(bool addToInterfaceAutomatically = true, PlayerStockDialogMode mode = PlayerStockDialogMode.View) 
        {            
            PlayerStockDialog newDialog = new PlayerStockDialog();
            if (addToInterfaceAutomatically)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            newDialog.Mode = mode;
            newDialog.BringToFront();            
            newDialog.SetInventory();
            return newDialog;
        }

        public PlayerStockDialogMode Mode
        {
            get { return mode; }
            private set { mode = value; }
        }

        public IMakeDecisions TargetUnit
        {
            get { return targetUnit; }
            set { targetUnit = value; }
        } 

        private void SetMoney(int money)
        {
            this.uxMoney.Text = string.Format("Money: {0}", money);
        }

        public void SetInventory()
        {
            Inventory items = PlayerStateManager.Instance.PlayerInventory;            

            this.uxItemWindow.Clear();

            this.SetMoney(items.Money);

            HashSet<string> resourcesOwned = new HashSet<string>();

            foreach (Item item in items.Items)
            {
                IconInfo icon = item.Icon;
                TooltipButtonControl newButton = new TooltipButtonControl();
                newButton.TooltipText = item.DisplayName;
                newButton.Bounds = new UniRectangle(0, 0, icon.Dimensions, icon.Dimensions);
                newButton.Tag = item;
                newButton.SetIcon(icon);

                if (mode == PlayerStockDialogMode.Give || mode == PlayerStockDialogMode.Sell)
                {
                    newButton.Pressed += this.HandleItemPressed;
                }       

                this.uxItemWindow.AddControl(newButton, false);

                if (item.Stats.Type == ItemType.Resource)
                {
                    resourcesOwned.Add(item.ObjectName);
                }
            }

            // Put in list and sort to keep orders consistent
            List<string> resources = new List<string>(resourcesOwned.ToList<string>());
            resources.Sort();
            this.uxResourcesFlowPanel.Clear();
            foreach (string resource in resources)
            {
                IconInfo icon = TextureManager.Instance.GetIconInfo(resource);
                string text = items.GetItemCount(resource).ToString();
                TooltipButtonAndTextControl newResource = new TooltipButtonAndTextControl(icon, text, 100);                
                this.uxResourcesFlowPanel.AddControl(newResource);
            }

            this.uxItemWindow.RefreshControls();
        }

        private void HandleItemPressed(object sender, EventArgs e)
        {
            this.targetItem = (Item)((TooltipButtonControl)sender).Tag;

            if (this.mode == PlayerStockDialogMode.Give)
            {
                MessageDialog dialog = MessageDialog.CreateDialog(string.Format("Give away {0}?", this.targetItem.DisplayName));
                dialog.YesOrNoPromptMode = true;
                dialog.ButtonResult += this.HandleGiveButtonResult;
            }
            else if (this.mode == PlayerStockDialogMode.Sell)
            {
                PlayerItemBidDialog dialog = PlayerItemBidDialog.CreateDialog(this.targetItem, this.targetUnit, true, PlayerItemBidDialogMode.Sell);
                dialog.CloseClicked += this.HandleSellDialogClosed;
            }
        }

        private void HandleSellDialogClosed(object sender, EventArgs e)
        {
            try
            {                
                this.SetInventory();                
            }
            finally
            {
                targetItem = null;
            }
        }        

        private void HandleGiveButtonResult(object sender, EventArgsEx<MessageDialogResult> e)
        {
            try
            {
                Debug.Assert(this.TargetUnit != null, "Need to set TargetUnit for this dialog");
                if (e.Value == MessageDialogResult.Yes)
                {
                    this.TargetUnit.AcquireItem(targetItem, AcquiredItemSource.Gifted);
                    PlayerStateManager.Instance.PlayerInventory.RemoveItem(targetItem);
                    this.SetInventory();
                }
            }
            finally
            {
                targetItem = null;
            }
        }
    }

    public partial class PlayerStockDialog
    {
        private void InitializeComponent()
        {
            this.uxMoney = new LabelControl();
            this.uxMoney.Bounds = new UniRectangle(6.0f, 26.0f, 200.0f, 30.0f);

            this.uxItemWindow = new ScrollableControlList(32, 32);
            this.uxItemWindow.Bounds = new UniRectangle(6.0f, 52.0f, 388.0f, 130.0f);

            this.uxResourcesFlowPanel = new FlowPanelControl(new UniRectangle(6.0f, this.uxItemWindow.Bounds.Bottom + 6.0f, 388.0f, 100.0f));

            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Close";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.Bounds = new UniRectangle(100.0f, 50.0f, 400.0f, 350.0f);

            this.Children.Add(this.uxResourcesFlowPanel);
            this.Children.Add(this.uxMoney);
            this.Children.Add(this.uxClose);
            this.Children.Add(this.uxItemWindow);
        }

        protected FlowPanelControl uxResourcesFlowPanel; 
        protected ButtonControl uxClose;
        protected ScrollableControlList uxItemWindow;
        protected LabelControl uxMoney;
    }

    public enum PlayerStockDialogMode
    {
        View,
        Give,
        Sell,
    }
}


