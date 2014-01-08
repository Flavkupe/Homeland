using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using TacticsGame.Items;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using System.Collections.ObjectModel;
using TacticsGame.UI.Dialogs;
using System.Diagnostics;

namespace TacticsGame.UI.Special
{
    public class ItemValueControl : Control
    {
        private ScrollableControlList scrollableControl;

        private List<ItemOrder> items = new List<ItemOrder>();        

        public event EventHandler<ValueEventArgs<Item>> ItemClicked;
        
        private ItemOrder clickedOrder = null;

        public ItemValueControl(UniRectangle bounds)
        {
            this.Bounds = bounds;
            this.scrollableControl = new ScrollableControlList(100, 32, 3, 3);
            this.scrollableControl.Bounds = bounds.RelocateClone(0, 0);
            this.Children.Add(scrollableControl);
        }

        public bool HasItem(Item item)
        {
            return this.items.Any(a => a.ItemName == item.ObjectName);
        }

        public void SetItem(ItemOrder item)
        {
            foreach (ItemOrder order in this.items.ToList())
            {
                if (order.ItemName == item.ItemName)
                {
                    order.SetTo(item);
                    this.RefreshItems();
                    return;
                }
            }

            this.items.Add(item);            
        }

        public List<ItemOrder> Items
        {
            get { return this.items; }
        }

        public void RemoveItem(Item item)
        {
            if (this.HasItem(item))
            {
                ItemOrder order = this.items.First(a => a.ItemName == item.ObjectName);
                order.Amount--;

                if (order.Amount <= 0)
                {
                    this.items.RemoveAll(a => a.ItemName == item.ObjectName);
                }

                this.RefreshItems();
            }
        }

        public void AddItem(Item item)
        {
            if (this.HasItem(item))
            {
                ItemOrder order = this.items.First(a => a.ItemName == item.ObjectName);
                order.Amount++;            
            }
            else
            {
                this.SetItem(new ItemOrder(item.ObjectName, 1, 0, ItemOrder.ItemOrderType.Selling));
            }

            this.RefreshItems();
        }

        private void RefreshItems()
        {
            this.scrollableControl.Clear();

            IconInfo coinIcon = TextureManager.Instance.GetIconInfo(ResourceId.Icons.GoldCoins.ToString());
            foreach (ItemOrder itemOrder in this.items)
            {
                Item backingItem = new Item(itemOrder.ItemName);
                TooltipButtonAndTextControl itemButton = new TooltipButtonAndTextControl(backingItem.Icon, string.Format("x{0}", itemOrder.Amount), 100, 32, 3);
                itemButton.Tag = itemOrder;
                itemButton.TooltipText = itemOrder.ItemName;                
                itemButton.Pressed += this.ItemIconClicked;
                this.scrollableControl.AddControl(itemButton);

                TooltipButtonAndTextControl valueButton = new TooltipButtonAndTextControl(coinIcon, itemOrder.Offer.ToString(), 100, 32, 0, 30);
                valueButton.ShowFrameOnImageButton = false;
                valueButton.Tag = itemOrder;
                valueButton.Pressed += HandleValueButtonPressed;
                this.scrollableControl.AddControl(valueButton);
            }

            this.scrollableControl.RefreshControls();
        }

        private void HandleValueButtonPressed(object sender, EventArgs e)
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

            this.RefreshItems();
        }

        private void ItemIconClicked(object sender, EventArgs e)
        {            
        }                
    }
}
