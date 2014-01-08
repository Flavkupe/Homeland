using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Arcade;
using Nuclex.UserInterface;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;
using TacticsGame.UI.Controls;
using TacticsGame.GameObjects.EntityMetadata;

namespace TacticsGame.UI.Groups
{
    public class UnitInventoryGroup : Control
    {
        public UnitInventoryGroup()
        {
            this.InitializeComponents();
        }

        private Unit currentUnit = null;

        public void SetUnitInventory(Unit unit)
        {
            this.currentUnit = unit;
            this.uxMoney.Text = "Money: " + unit.Inventory.Money;

            this.uxInventoryPanel.Children.Clear();

            this.uxUnitEquipment.SetEquipment(unit.Equipment);

            int x = 6;
            int y = 6;

            foreach (Item item in unit.Inventory.Items)
            {
                TooltipButtonControl newButton = new TooltipButtonControl();
                newButton.Tag = item;
                newButton.TooltipText = item.DisplayName;
                newButton.SetIcon(item.Icon);
                newButton.Bounds = new UniRectangle(new UniScalar(0.0f, x), new UniScalar(0.0f, y), item.Icon.Dimensions, item.Icon.Dimensions);
                newButton.Pressed += this.ItemPressed;

                this.uxInventoryPanel.Children.Add(newButton);

                x += item.Icon.Dimensions;

                if (x > (this.uxInventoryPanel.Bounds.Size.X.Offset - item.Icon.Dimensions))
                {
                    x = 6;
                    y += item.Icon.Dimensions;
                }
            }

            this.uxInventoryPanel.BringToFront();
        }

        public void ItemPressed(object sender, EventArgs e)
        {
            Item item = ((TooltipButtonControl)sender).Tag as Item;

            if (this.currentUnit.CanEquipItem(item))
            {
                this.currentUnit.Equip(item);
                this.currentUnit.Inventory.RemoveItem(item);
            }

            this.SetUnitInventory(this.currentUnit);
        }

        private void InitializeComponents()
        {
            this.uxInventoryPanel = new FramePanelControl();
            this.uxInventoryPanel.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 30.0f), 388, 150);

            this.uxUnitEquipment = new UnitEquipmentGroup();
            this.uxUnitEquipment.Bounds = new UniRectangle(0.0f, this.uxInventoryPanel.Bounds.Bottom.Offset + 6.0f, 400.0f, 175.0f);

            this.uxMoney = new LabelControl();
            this.uxMoney.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 6.0f), 40, 20);

            this.Children.Add(this.uxUnitEquipment);
            this.Children.Add(this.uxInventoryPanel);
            this.Children.Add(this.uxMoney);
        }

        private FramePanelControl uxInventoryPanel;
        private LabelControl uxMoney;

        protected UnitEquipmentGroup uxUnitEquipment;
    }
}
