using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using TacticsGame.GameObjects.Buildings.Types;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Managers;
using System.Diagnostics;
using TacticsGame.PlayerThings;
using TacticsGame.Items;

namespace TacticsGame.UI.Dialogs
{
    public class PlaceOrdersDialog : ModalDialogControl
    {        
        private PlaceOrdersDialog()
            : base()
        {
            InitializeComponents();
        }

        private ItemOrder clickedOrder = null;

        /// <summary>
        /// Create an instance of this dialog and make it visible.
        /// </summary>
        /// <returns></returns>
        public static PlaceOrdersDialog CreateDialog(bool addToInterface = true)
        {
            PlaceOrdersDialog newDialog = new PlaceOrdersDialog();
            if (addToInterface)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }
            
            return newDialog;
        }

        public void SetPurchaseOrders()
        {
            this.uxOrders.Clear();            

            List<ItemOrder> orders = PlayerStateManager.Instance.ActiveTown.ItemOrders;

            IconInfo coinIcon = TextureManager.Instance.GetIconInfo("Coin");
            foreach (ItemOrder order in orders.ToList<ItemOrder>()) 
            {
                if (order.Amount <= 0)
                {
                    orders.Remove(order);
                    continue;
                }

                string displayName = GameResourceManager.Instance.GetDisplayNameByResourceType(order.ItemName, ResourceType.Item);
                IconInfo icon = TextureManager.Instance.GetIconInfo(order.ItemName);
                TooltipButtonAndTextControl newButton = new TooltipButtonAndTextControl(icon, string.Format("{0}    x{1}", displayName, order.Amount), 130, 32, 0);
                newButton.ShowFrameOnImageButton = false;
                newButton.Tag = order;
                newButton.Pressed += this.HandleOrderAmountEditPressed;
                this.uxOrders.AddControl(newButton);
                               
                TooltipButtonAndTextControl newButton2 = new TooltipButtonAndTextControl(coinIcon, order.Offer.ToString(), 130, 32, 0, 30);
                newButton2.ShowFrameOnImageButton = false;
                newButton2.Tag = order;
                newButton2.Pressed += this.HandleOrderPriceEditPressed;
                this.uxOrders.AddControl(newButton2);
            }

            this.uxOrders.RefreshControls(true);
        }

        private void HandleOrderAmountEditPressed(object sender, EventArgs e)
        {
            InputDialog dialog = InputDialog.CreateDialog();
            this.clickedOrder = (ItemOrder)((TooltipButtonAndTextControl)sender).Tag;
            dialog.Input = this.clickedOrder.Amount.ToString();
            dialog.InputResult += this.HandleInputResultForAmountEdit;
        }

        private void HandleInputResultForAmountEdit(object sender, EventArgsEx<string> e)
        {
            Debug.Assert(this.clickedOrder != null);
            int result;
            if (int.TryParse(e.Value, out result))
            {
                this.clickedOrder.Amount = result;
            }

            this.clickedOrder = null;

            this.SetPurchaseOrders();
        }

        private void HandleOrderPriceEditPressed(object sender, EventArgs e)
        {
            InputDialog dialog = InputDialog.CreateDialog();
            this.clickedOrder = (ItemOrder)((TooltipButtonAndTextControl)sender).Tag;
            dialog.Input = this.clickedOrder.Offer.ToString();
            dialog.InputResult += this.HandleInputResultForPriceEdit;
        }

        private void HandleInputResultForPriceEdit(object sender, EventArgsEx<string> e)
        {
            Debug.Assert(this.clickedOrder != null);
            int result;
            if (int.TryParse(e.Value, out result))
            {
                this.clickedOrder.Offer = result;
            }

            this.clickedOrder = null;

            this.SetPurchaseOrders();
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(100, 100, 300, 300);

            this.uxOrders = new ScrollableControlList(130, 32, 3, 3);
            this.uxOrders.Bounds = new UniRectangle(6.0f, 26.0f, 288, 160);

            this.uxCloseButton.Bounds = new UniRectangle(new UniScalar(1.0f, -56.0f), new UniScalar(1.0f, -26.0f), 50, 20);
            this.uxCloseButton.Pressed += this.HandleCloseClicked;
            this.uxCloseButton.Text = "Close";

            this.uxPlaceOrder.Bounds = new UniRectangle(6.0f, this.uxOrders.Bounds.Bottom + 6.0f, 130, 20);
            this.uxPlaceOrder.Text = "Place New Order";
            this.uxPlaceOrder.Pressed += this.HandlePlaceOrderPressed;

            this.Children.Add(this.uxCloseButton);
            this.Children.Add(this.uxOrders);
            this.Children.Add(this.uxPlaceOrder);
        }

        private void HandlePlaceOrderPressed(object sender, EventArgs e)
        {            
            NewOrderDialog dialog = NewOrderDialog.CreateDialog();
            dialog.SetAvailableOrders();
            dialog.NewOrderMade += this.HandleNewOrderMade;
        }

        private void HandleNewOrderMade(object sender, EventArgsEx<ItemOrder> e)
        {
            List<ItemOrder> orders = PlayerStateManager.Instance.ActiveTown.ItemOrders;
            orders.Add(e.Value);
            this.SetPurchaseOrders();
        }

        private ButtonControl uxCloseButton = new ButtonControl();
        private ButtonControl uxPlaceOrder = new ButtonControl();
        private ScrollableControlList uxOrders = null;             
    }
}
