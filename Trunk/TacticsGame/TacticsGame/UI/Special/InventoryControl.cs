using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using TacticsGame.Items;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using System.Collections.ObjectModel;

namespace TacticsGame.UI.Special
{
    public class InventoryControl : Control
    {
        private ScrollableControlList scrollableControl;

        private List<Item> items = new List<Item>();        

        public event EventHandler<ValueEventArgs<Item>> ItemClicked;

        public InventoryControl(UniRectangle bounds)
        {
            this.Bounds = bounds;
            this.scrollableControl = new ScrollableControlList(32, 32, 3, 3);
            this.scrollableControl.Bounds = bounds.RelocateClone(0, 0);
            this.Children.Add(scrollableControl);
        }

        public void AddItem(Item item) 
        {
            items.Add(item);
            this.RefreshItems();
        }

        public void RemoveItem(Item item) 
        {
            items.Remove(item);
            this.RefreshItems();
        }        

        public void SetItems(IEnumerable<Item> items)
        {
            this.items = items.ToList();
            this.RefreshItems();
        }

        public ReadOnlyCollection<Item> Items
        {
            get { return new ReadOnlyCollection<Item>(items); }
        }

        private void RefreshItems()
        {
            this.scrollableControl.Clear();
            
            foreach (Item item in this.items)
            {
                TooltipButtonControl itemButton = new TooltipButtonControl();
                itemButton.Bounds = new UniRectangle(0, 0, 32, 32);
                itemButton.Tag = item;
                itemButton.TooltipText = item.DisplayName;
                itemButton.SetIcon(item.Icon);
                itemButton.Pressed += this.ItemIconClicked;
                this.scrollableControl.AddControl(itemButton);
            }

            this.scrollableControl.RefreshControls();
        }

        private void ItemIconClicked(object sender, EventArgs e)
        {
            TooltipButtonControl itemButton = (TooltipButtonControl)sender;
            Item item = (Item)itemButton.Tag;

            if (this.ItemClicked != null)
            {
                this.ItemClicked(this, new ValueEventArgs<Item>(item));
            }
        }
    }
}
