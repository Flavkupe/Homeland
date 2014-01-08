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
using TacticsGame.UI.Special;
using TacticsGame.World.Caravans;

namespace TacticsGame.UI.Dialogs
{
    public class CaravanDialog : ModalDialogControl
    {
        private CaravanDialog()
            : base()
        {
            this.InitializeComponents();
        }

        public static CaravanDialog CreateDialog(bool viewByDefault = true)
        {
            CaravanDialog newDialog = new CaravanDialog();

            if (viewByDefault)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            newDialog.SetInventory();

            return newDialog;
        }

        public void SetInventory()
        {
            Inventory inventory = PlayerStateManager.Instance.PlayerInventory;
            this.uxInventory.SetItems(inventory.Items);            
        }

        public void HandleCargoItemClicked(object sender, ValueEventArgs<Item> e)
        {
            this.uxCargo.RemoveItem(e.Value);
            this.uxInventory.AddItem(e.Value);           
            this.uxItemValues.RemoveItem(e.Value);            
        }

        public void HandleInventoryItemClicked(object sender, ValueEventArgs<Item> e)
        {
            this.uxCargo.AddItem(e.Value);
            this.uxInventory.RemoveItem(e.Value);            
            this.uxItemValues.AddItem(e.Value);            
        }

        void HandleConfirmPressed(object sender, EventArgs e)
        {
            PlayerStateManager.Instance.PlayerInventory.RemoveItems(this.uxCargo.Items);            
            
            Caravan caravan = new Caravan();
            caravan.Cargo.AddItems(this.uxCargo.Items);
            PlayerStateManager.Instance.ActiveTown.Caravans.Add(caravan);
            this.CloseThisDialog();
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(50, 50, 588, 400);

            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Cancel";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.uxConfirm = new ButtonControl();
            this.uxConfirm.Text = "Confirm";
            this.uxConfirm.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -128.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxConfirm.Pressed += HandleConfirmPressed;

            this.uxInventoryLabel = new LabelControl();
            this.uxCargoLabel = new LabelControl();
            this.uxValueLabel = new LabelControl();

            this.uxInventoryLabel.Bounds = new UniRectangle(6.0f, 26.0f, 100.0f, 20.0f);
            this.uxInventoryLabel.Text = "Inventory:";

            this.uxInventory = new InventoryControl(new UniRectangle(6.0f, 46.0f, 300.0f, 100.0f));

            this.uxCargoLabel.Bounds = new UniRectangle(6.0f, this.uxInventory.GetBottom() + 6.0f, 100.0f, 20.0f);
            this.uxCargoLabel.Text = "Cargo:";
            
            this.uxCargo = new InventoryControl(new UniRectangle(6.0f, this.uxCargoLabel.GetBottom() + 6.0f, 300.0f, 100.0f));            

            this.uxInventory.ItemClicked += this.HandleInventoryItemClicked;
            this.uxCargo.ItemClicked += this.HandleCargoItemClicked;

            this.uxValueLabel.Bounds = new UniRectangle(this.uxInventory.GetRight() + 12.0f, 24.0f, 100.0f, 20.0f);
            this.uxValueLabel.Text = "Prices:";

            this.uxItemValues = new ItemValueControl(new UniRectangle(this.uxInventory.GetRight() + 12.0f, 46.0f, 250.0f, this.uxCargo.GetBottom() - this.uxInventory.GetTop()));

            this.Children.Add(this.uxCargoLabel);
            this.Children.Add(this.uxValueLabel);
            this.Children.Add(this.uxInventoryLabel);
            this.Children.Add(this.uxCargo);
            this.Children.Add(this.uxInventory);
            this.Children.Add(this.uxClose);
            this.Children.Add(this.uxConfirm);
            this.Children.Add(this.uxItemValues);
        }

        private LabelControl uxValueLabel;
        private LabelControl uxInventoryLabel;
        private LabelControl uxCargoLabel;
        private ButtonControl uxClose;
        private ButtonControl uxConfirm;
        private InventoryControl uxInventory;
        private InventoryControl uxCargo;
        private ItemValueControl uxItemValues;

    }
}
