using System;
using System.Diagnostics;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;
using TacticsGame.UI.Controls;
using TacticsGame.UI.Panels;

namespace TacticsGame.UI.Groups
{
    public class UnitEquipmentGroup : Control
    {
        public UnitEquipmentGroup() 
        {
            this.InitializeComponents();
        }

        public void SetEquipment(Equipment equipment)
        {
            this.uxItems.Clear();

            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                if (slot == EquipmentSlot.None) { continue; }

                Item item = equipment.EquippedItem(slot);

                BetterLabelControl label = new BetterLabelControl();
                label.Bounds = new UniRectangle(0, 0, 80, 20);
                label.Text = string.Format("{0}:", slot.ToString());
                this.uxItems.AddControl(label, null, item == null);

                if (item != null) 
                {
                    TooltipButtonAndTextControl itemControl = new TooltipButtonAndTextControl(equipment[slot].Icon, equipment[slot].DisplayName, 200, 20, 0);
                    itemControl.Bounds = new UniRectangle(0,0,200,20);
                    itemControl.Tag = item;
                    itemControl.MouseEntered += this.HandleItemMouseEntered;
                    itemControl.MouseLeft += this.HandleItemMouseLeft;
                    this.uxItems.AddControl(itemControl);
                }                               
            }
        }

        private void HandleItemMouseLeft(object sender, EventArgs e)
        {
            this.SetControlVisible(this.uxItemStats, false);
        }

        private void HandleItemMouseEntered(object sender, EventArgs e)
        {
            TooltipButtonAndTextControl control = (TooltipButtonAndTextControl)sender;
            Item item = control.Tag as Item;
            Debug.Assert(item != null);            
            this.uxItemStats.Bounds = this.uxItemStats.Bounds.RelocateClone(control.Bounds.Right.Offset, control.Bounds.Top.Offset);
            this.uxItemStats.SetItemProperties(item);
            this.SetControlVisible(this.uxItemStats, true);
            this.uxItemStats.BringToFront();   
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(0,0, 400, 175);
            this.uxItems = new FlowPanelControl(this.Bounds);
            this.uxItems.Bounds = this.Bounds.NudgeClone(0, 0);

            this.uxItemStats = new ItemStatsPanel();            

            this.Children.Add(uxItems);
        }

        private FlowPanelControl uxItems;
        private ItemStatsPanel uxItemStats = null;
    }
}
