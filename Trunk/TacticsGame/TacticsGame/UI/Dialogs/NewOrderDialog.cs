using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using TacticsGame.Utility;
using TacticsGame.GameObjects.Buildings.Types;
using TacticsGame.Managers;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.PlayerThings;
using TacticsGame.Items;

namespace TacticsGame.UI.Dialogs
{
    public class NewOrderDialog : ModalDialogControl
    {
        private NewOrderDialog()
            : base()
        {
            this.InitializeComponent();
        }

        public event EventHandler<EventArgsEx<ItemOrder>> NewOrderMade;

        /// <summary>
        /// Create an instance of this dialog and make it visible.
        /// </summary>
        /// <returns></returns>
        public static NewOrderDialog CreateDialog(bool addToInterface = true)
        {
            NewOrderDialog newDialog = new NewOrderDialog();
            if (addToInterface)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            return newDialog;
        }

        public void SetAvailableOrders()
        {
            List<ItemOrder> orders = PlayerStateManager.Instance.ActiveTown.ItemOrders;
            List<string> resources = ItemGenerationUtilities.GetAllResources();
            foreach (ItemOrder order in orders)
            {
                if (resources.Contains(order.ItemName))
                {
                    resources.Remove(order.ItemName);
                }
            }

            foreach (string resource in resources)
            {
                string displayName = GameResourceManager.Instance.GetDisplayNameByResourceType(resource, ResourceType.Item);
                IconInfo icon = TextureManager.Instance.GetIconInfo(resource);
                TooltipButtonAndTextControl newButton = new TooltipButtonAndTextControl(icon, displayName, 130, 32, 0);
                newButton.ShowFrameOnImageButton = false;
                newButton.Tag = resource;
                this.uxItemsAvailable.AddControl(newButton);
            }

            this.uxItemsAvailable.RefreshControls(true);
        }

        private void InitializeComponent()
        {
            this.Bounds = new UniRectangle(100, 100, 200, 300);

            this.uxItemsAvailable = new ScrollableControlList(130, 32, 3, 3);
            this.uxItemsAvailable.Bounds = new UniRectangle(6.0f, 26.0f, 188, 160);
            this.uxItemsAvailable.AllowSelection = true;

            this.uxCloseButton.Bounds = new UniRectangle(new UniScalar(1.0f, -56.0f), new UniScalar(1.0f, -26.0f), 50, 20);
            this.uxCloseButton.Pressed += this.HandleCloseClicked;
            this.uxCloseButton.Text = "Close";
            
            this.uxAmountLabel.Bounds = new UniRectangle(6.0f, this.uxItemsAvailable.Bounds.Bottom + 6.0f, 80, 20);
            this.uxAmountLabel.Text = "Amount";
            this.uxPriceLabel.Bounds = new UniRectangle(6.0f, this.uxAmountLabel.Bounds.Bottom + 6.0f, 80, 20);
            this.uxPriceLabel.Text = "Price";
            this.uxAmountBox.Bounds = new UniRectangle(86.0f, this.uxAmountLabel.Bounds.Location.Y, 40, 20);
            this.uxPriceBox.Bounds = new UniRectangle(86.0f, this.uxPriceLabel.Bounds.Location.Y, 40, 20);

            this.uxPlaceOrderButton.Bounds = new UniRectangle(6.0f, this.uxPriceLabel.Bounds.Bottom + 6.0f, 100, 20);
            this.uxPlaceOrderButton.Text = "Place Order";
            this.uxPlaceOrderButton.Pressed += this.HandlePlaceOrderButtonPressed;

            this.Children.Add(this.uxAmountLabel);
            this.Children.Add(this.uxAmountBox);
            this.Children.Add(this.uxPriceLabel);
            this.Children.Add(this.uxPriceBox);
            this.Children.Add(this.uxCloseButton);
            this.Children.Add(this.uxItemsAvailable);
            this.Children.Add(this.uxPlaceOrderButton);
        }

        void HandlePlaceOrderButtonPressed(object sender, EventArgs e)
        {
            if (this.uxItemsAvailable.Selection == null)
            {
                return;
            }

            int amount;
            if (!int.TryParse(this.uxAmountBox.Text, out amount)) 
            {
                return;
            }

            int offer;
            if (!int.TryParse(this.uxPriceBox.Text, out offer)) 
            {
                return;
            }

            TooltipButtonAndTextControl control = (TooltipButtonAndTextControl)this.uxItemsAvailable.Selection;
            string itemType = (string)control.Tag;

            ItemOrder newOrder = new ItemOrder(itemType, amount, offer, ItemOrder.ItemOrderType.Buying);

            this.CloseThisDialog();

            if (this.NewOrderMade != null)
            {
                this.NewOrderMade(this, new EventArgsEx<ItemOrder>(newOrder));
            }
        }

        ScrollableControlList uxItemsAvailable;
        InputControl uxAmountBox = new InputControl();
        InputControl uxPriceBox = new InputControl();
        BetterLabelControl uxAmountLabel = new BetterLabelControl();
        BetterLabelControl uxPriceLabel = new BetterLabelControl();
        ButtonControl uxPlaceOrderButton = new ButtonControl();
        ButtonControl uxCloseButton = new ButtonControl();

    }
}
