using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using TacticsGame.UI.Groups;
using TacticsGame.Edicts;
using Nuclex.UserInterface.Controls;
using TacticsGame.Managers;
using Microsoft.Xna.Framework;
using TacticsGame.Items;
using TacticsGame.GameObjects.Units;
using System.Diagnostics;
using TacticsGame.PlayerThings;

namespace TacticsGame.UI.Dialogs
{
    public class LootDialog : ModalDialogControl
    {
        Dictionary<DecisionMakingUnit, List<Item>> lootPerUnit = null;
        List<DecisionMakingUnit> units = null;
        List<Item> claimedItems = new List<Item>();

        private LootDialog()
            : base()
        {
            this.InitializeComponents();
        }        

        public static LootDialog CreateDialog(bool viewByDefault = true)
        {
            LootDialog newDialog = new LootDialog();

            if (viewByDefault)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            return newDialog;
        }

        public void SetLoot(List<DecisionMakingUnit> units, List<Item> loot)
        {            
            Debug.Assert(units.Count > 0 && loot.Count > 0);

            // Initialize the listing of loot per each unit
            this.lootPerUnit = this.DistributeLoot(units, loot);
            this.units = units;
            this.LayoutLoot();
        }

        private void LayoutLoot()
        {
            this.uxContainer.Clear();
            this.uxClaimed.Clear();
            foreach (DecisionMakingUnit unit in this.units)
            {
                FlowPanelControl group = new FlowPanelControl(new UniRectangle(0, 0, 470, 50));

                TooltipButtonAndTextControl unitLabel = new TooltipButtonAndTextControl(unit.GetEntityIcon(), string.Format("{0}:", unit.DisplayName), 170);
                unitLabel.ShowFrameOnImageButton = false;
                unitLabel.TooltipText = string.Format("Loyalty: {0}", unit.CurrentStats.Loyalty);
                group.AddControl(unitLabel);

                foreach (Item item in lootPerUnit[unit])
                {
                    TooltipButtonControl itemButton = new TooltipButtonControl();
                    itemButton.SetIcon(item.Icon);
                    itemButton.TooltipText = item.DisplayName;
                    itemButton.Bounds = new UniRectangle(0, 0, 32, 32);
                    itemButton.Pressed += this.UnitItemClicked;
                    itemButton.Tag = new UnitItemPair(unit, item);
                    group.AddControl(itemButton);
                }

                this.uxContainer.AddControl(group);
            }

            this.uxContainer.RefreshControls();

            foreach (Item item in this.claimedItems)
            {
                TooltipButtonControl itemButton = new TooltipButtonControl();
                itemButton.SetIcon(item.Icon);
                itemButton.TooltipText = item.DisplayName;
                itemButton.Bounds = new UniRectangle(0, 0, 32, 32);                                
                this.uxClaimed.AddControl(itemButton);
            }

            this.uxClaimed.RefreshControls();
        }

        private void UnitItemClicked(object sender, EventArgs e)
        {
            TooltipButtonControl itemButton = (TooltipButtonControl)sender;
            UnitItemPair pair = itemButton.Tag as UnitItemPair;
            this.claimedItems.Add(pair.Item);
            this.lootPerUnit[pair.Unit].Remove(pair.Item);

            // TEMP
            pair.Unit.CurrentStats.Loyalty = Math.Max(0, pair.Unit.CurrentStats.Loyalty - 10);

            this.LayoutLoot();
        }

        /// <summary>
        /// Randomly distributes loot amongst units.
        /// </summary>
        private Dictionary<DecisionMakingUnit, List<Item>> DistributeLoot(List<DecisionMakingUnit> units, List<Item> loot)
        {
            Dictionary<DecisionMakingUnit, List<Item>> lootPerUnit;
            lootPerUnit = new Dictionary<DecisionMakingUnit, List<Item>>();
            foreach (DecisionMakingUnit unit in units)
            {
                lootPerUnit[unit] = new List<Item>();
            }

            List<DecisionMakingUnit> unitQueue = new List<DecisionMakingUnit>();
            while (loot.Count > 0)
            {
                if (unitQueue.Count == 0)
                {
                    unitQueue.AddRange(units);
                }

                DecisionMakingUnit currentUnit = unitQueue.GetRandomItem();
                unitQueue.Remove(currentUnit);

                Item currentItem = loot.GetRandomItem();
                loot.Remove(currentItem);
                lootPerUnit[currentUnit].Add(currentItem);
            }
            return lootPerUnit;
        }

        protected override void HandleCloseClicked(object sender, EventArgs e)
        {
            foreach (DecisionMakingUnit unit in this.lootPerUnit.Keys)
            {
                foreach (Item item in this.lootPerUnit[unit])
                {
                    unit.AcquireItem(item, AcquiredItemSource.Looted);
                }
            }

            PlayerStateManager.Instance.Player.AcquireItems(this.claimedItems, AcquiredItemSource.Looted);

            base.HandleCloseClicked(sender, e);
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(100, 100, 688, 300);

            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Done";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.uxTopLabel = new BetterLabelControl();
            this.uxTopLabel.Bounds = new UniRectangle(6.0f, 26.0f, 60.0f, 30.0f);
            this.uxTopLabel.Text = "Loot distribution:";

            this.uxContainer = new ScrollableControlList(300, 50, 6, 6);
            this.uxContainer.Bounds = new UniRectangle(6, this.uxTopLabel.Bounds.Bottom + 6, 500, 200);

            this.uxClaimed = new ScrollableControlList(32, 32, 6, 6);
            this.uxClaimed.Bounds = new UniRectangle(this.uxContainer.Bounds.Right + 6, this.uxTopLabel.Bounds.Bottom + 6, 164, 200);

            this.uxTopLabelClaimed = new BetterLabelControl();
            this.uxTopLabelClaimed.Bounds = new UniRectangle(this.uxClaimed.Bounds.Left, 26.0f, 60.0f, 30.0f);
            this.uxTopLabelClaimed.Text = "Claimed:";

            this.Children.Add(this.uxTopLabel); 
            this.Children.Add(this.uxTopLabelClaimed);
            this.Children.Add(this.uxContainer);
            this.Children.Add(this.uxClaimed);
            this.Children.Add(this.uxClose);        
        }        

        private ButtonControl uxClose;
        private BetterLabelControl uxTopLabel;
        private BetterLabelControl uxTopLabelClaimed;
        private ScrollableControlList uxContainer;
        private ScrollableControlList uxClaimed;

        private class UnitItemPair
        {
            public DecisionMakingUnit Unit { get; set; }
            public Item Item { get; set; }
            public UnitItemPair(DecisionMakingUnit unit, Item item)
            {
                this.Unit = unit;
                this.Item = item;
            }
        }
    }
}
