using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;
using TacticsGame.Managers;
using TacticsGame.UI.Controls;
using TacticsGame.UI.Panels;

namespace TacticsGame.UI.Dialogs
{
    public class PlayerItemBidDialog : ModalDialogControl
    {
        private InputControl uxTextbox = new InputControl();
        private LabelControl uxBidLabel = new LabelControl();
        private LabelControl uxMessage = new LabelControl();
        private LabelControl uxMoney = new LabelControl();

        private TooltipButtonAndTextControl uxItemIcon = null;

        private ItemStatsPanel uxItemStats = null;

        private ButtonControl uxRequestBidButton = new ButtonControl();
        private ButtonControl uxOfferButton = new ButtonControl();
        private ButtonControl uxCancelButton = new ButtonControl();

        private Item item = null;
        private IMakeDecisions target = null;
        private int numBids = 0;

        private PlayerItemBidDialogMode mode = PlayerItemBidDialogMode.Buy;

        public event EventHandler BeforeDialogClosed;

        // If seller offers a bid, make sure it's never rejected.
        private int? lastBidOfferedByTarget = null;

        private PlayerItemBidDialog(Item item, IMakeDecisions target, PlayerItemBidDialogMode mode = PlayerItemBidDialogMode.Buy)
            : base()
        {
            this.mode = mode;
            this.item = item;
            this.target = target;

            this.InitializeComponents();

            //this.AffectsOrdering = true;
            
            this.numBids = Utilities.GetRandomNumber(3, 6); 
        }

        /// <summary>
        /// Create an instance of this dialog and make it visible.
        /// </summary>
        /// <returns></returns>
        public static PlayerItemBidDialog CreateDialog(Item item, IMakeDecisions seller, bool addToInterfaceAutomatically = true, PlayerItemBidDialogMode mode = PlayerItemBidDialogMode.Buy)
        {           
            PlayerItemBidDialog newDialog = new PlayerItemBidDialog(item, seller, mode);
            if (addToInterfaceAutomatically)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            newDialog.BringToFront();            
            return newDialog;
        }        

        private void InitializeComponents()
        {            
            this.Bounds = new UniRectangle(50, 50, 300, 200);            

            int margin = 6;            

            this.uxItemIcon = new TooltipButtonAndTextControl(this.item.Icon, this.item.DisplayName, new Vector2(margin, 20 + margin), 200, 32, margin);
            this.uxItemIcon.MouseEntered += this.HandleItemIconMouseEntered;
            this.uxItemIcon.MouseLeft += this.HandleItemIconMouseLeft;

            this.uxItemStats = new ItemStatsPanel();
            this.uxItemStats.Bounds = this.uxItemStats.Bounds.RelocateClone(this.uxItemIcon.Bounds.Left.Offset, this.uxItemIcon.Bounds.Bottom.Offset);
            this.uxItemStats.SetItemProperties(this.item);

            this.uxMoney.Bounds = new UniRectangle(margin, this.uxItemIcon.Bounds.Bottom + margin, 100, 20);
            this.uxMoney.Text = string.Format("Money: {0}", mode == PlayerItemBidDialogMode.Buy ? PlayerStateManager.Instance.PlayerInventory.Money : this.target.Inventory.Money);

            this.uxBidLabel.Bounds = new UniRectangle(margin, this.uxMoney.Bounds.Bottom + margin, 30, 20);
            this.uxBidLabel.Text = "Bid:";
            this.uxTextbox.Bounds = new UniRectangle(this.uxBidLabel.Bounds.Right + margin, this.uxMoney.Bounds.Bottom + margin, 80, 20);
            this.uxTextbox.Text = "0";
            this.uxRequestBidButton.Bounds = new UniRectangle(this.uxTextbox.Bounds.Right + margin, this.uxMoney.Bounds.Bottom + margin, 100, 20);
            this.uxRequestBidButton.Text = "Request Bid";
            this.uxRequestBidButton.Pressed += this.HandleRequestBidPressed;

            this.uxCancelButton.Bounds = new UniRectangle(new UniScalar(1.0f, -60.0f), new UniScalar(1.0f, -20.0f - margin), 50, 20);
            this.uxCancelButton.Text = "Cancel";
            this.uxCancelButton.Pressed += this.HandleCancelButtonPressed;

            this.uxOfferButton.Bounds = new UniRectangle(new UniScalar(0.0f, margin), new UniScalar(1.0f, -20.0f - margin), 50, 20);
            this.uxOfferButton.Text = "Offer";
            this.uxOfferButton.Pressed += this.HandleOfferButtonPressed;

            this.uxMessage.Bounds = new UniRectangle(margin, this.uxBidLabel.Bounds.Bottom + margin, 200, 40);
            this.uxMessage.Text = "Make me an offer!";

            this.Children.Add(this.uxMoney);
            this.Children.Add(this.uxMessage);
            this.Children.Add(this.uxBidLabel);
            this.Children.Add(this.uxCancelButton);
            this.Children.Add(this.uxItemIcon);
            this.Children.Add(this.uxOfferButton);
            this.Children.Add(this.uxRequestBidButton);
            this.Children.Add(this.uxTextbox);
        }

        private void HandleItemIconMouseLeft(object sender, EventArgs e)
        {
            this.SetControlVisible(this.uxItemStats, false);
        }

        private void HandleItemIconMouseEntered(object sender, EventArgs e)
        {
            this.uxItemStats.Bounds = this.uxItemStats.Bounds.RelocateClone(this.uxItemIcon.Bounds.Left.Offset, this.uxItemIcon.Bounds.Bottom.Offset);
            this.SetControlVisible(this.uxItemStats, true);
            this.uxItemStats.BringToFront();
        }        

        private void HandleRequestBidPressed(object sender, EventArgs e)
        {
            if (this.mode == PlayerItemBidDialogMode.Buy)
            {
                this.RequestBuyBid();
            }
            else
            {
                this.RequestSellBid();
            }
        }

        private void RequestSellBid()
        {
            int shopkeepBid = UnitDecisionUtils.MakeBid(this.target, this.item, false);
            this.lastBidOfferedByTarget = shopkeepBid;
            this.uxTextbox.Text = shopkeepBid.ToString();
            this.uxRequestBidButton.Enabled = false;
            this.uxMessage.Text = shopkeepBid > 0 ? "I'll pay this much." : "Not interested in buying that.";            
        }

        private void RequestBuyBid()
        {
            int shopkeepBid = UnitDecisionUtils.MakeBid(this.target, this.item, true);            
            this.lastBidOfferedByTarget = shopkeepBid;
            this.uxTextbox.Text = shopkeepBid.ToString();
            this.uxRequestBidButton.Enabled = false;
            this.uxMessage.Text = shopkeepBid > 0 ? "How about this much?" : "Not interested in selling that.";
        }        

        private void HandleCancelButtonPressed(object sender, EventArgs e)
        {            
            this.CloseThisDialog();
        }

        private void HandleOfferButtonPressed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.uxTextbox.Text))
            {
                this.uxMessage.Text = "Well?";
                return;
            }

            int playerBid;
            if (!int.TryParse(this.uxTextbox.Text, out playerBid))
            {
                this.uxMessage.Text = string.Format("{0} aint no currency amount I ever heard of!", this.uxTextbox.Text);
                return;
            }

            if (mode == PlayerItemBidDialogMode.Buy)
            {
                this.HandleBuyOfferMade(playerBid);
            }
            else
            {
                this.HandleSellOfferMade(playerBid);
            }
        }

        private void HandleSellOfferMade(int playerBid)
        {
            if (playerBid > this.target.Inventory.Money)
            {
                this.uxMessage.Text = "I can't afford that!";
                return;
            }

            int shopkeepBid = UnitDecisionUtils.MakeBid(this.target, this.item, false);

            if (shopkeepBid >= playerBid || (this.lastBidOfferedByTarget.HasValue && this.lastBidOfferedByTarget.Value >= playerBid))
            {
                this.lastBidOfferedByTarget = null;
                this.AcceptBid(playerBid);
            }
            else
            {
                this.lastBidOfferedByTarget = null;
                this.RejectBid();
            }
        }

        private void HandleBuyOfferMade(int playerBid)
        {
            if (playerBid > PlayerStateManager.Instance.PlayerInventory.Money)
            {
                this.uxMessage.Text = "You aint got 'nuff dough!";
                return;
            }

            int shopkeepBid = UnitDecisionUtils.MakeBid(this.target, this.item, true);
            //Console.WriteLine("Seller {0} is willing to sell {1} for {2}", this.target.DisplayName, this.item.DisplayName, shopkeepBid);
            shopkeepBid += Utilities.GetRangePercent(shopkeepBid, -10, 10); // Range it anywhere from -10 to 10 percent of value
            //Console.WriteLine("Seller {0} is adjusting price of {1} to require a cost of {2}", this.target.DisplayName, this.item.DisplayName, shopkeepBid);

            if (shopkeepBid <= playerBid || (this.lastBidOfferedByTarget.HasValue && this.lastBidOfferedByTarget.Value <= playerBid))
            {
                this.lastBidOfferedByTarget = null;
                this.AcceptBid(playerBid);
            }
            else
            {
                this.lastBidOfferedByTarget = null;
                this.RejectBid();
            }

            return;
        }

        private void AcceptBid(int bid)
        {
            if (mode == PlayerItemBidDialogMode.Buy)
            {
                PlayerStateManager.Instance.PlayerInventory.AddItem(item);
                PlayerStateManager.Instance.PlayerInventory.Money -= bid;

                this.target.Inventory.Money += bid;
                this.target.Inventory.RemoveItem(item);
                this.uxRequestBidButton.Enabled = true;

                this.CloseThisDialog();
            }
            else
            {
                PlayerStateManager.Instance.PlayerInventory.RemoveItem(item);
                PlayerStateManager.Instance.PlayerInventory.Money += bid;

                this.target.Inventory.Money -= bid;
                this.target.Inventory.AddItem(item);
                this.uxRequestBidButton.Enabled = true;

                this.CloseThisDialog();
            }
        }

        private void RejectBid()
        {
            this.numBids--;
            if (this.numBids <= 0)
            {
                this.uxRequestBidButton.Enabled = false;
                this.uxTextbox.Enabled = false;
                this.uxOfferButton.Enabled = false;
                this.uxMessage.Text = this.GenerateMessage(MessageType.NoMoreBids);
            }
            else
            {
                this.uxMessage.Text = this.GenerateMessage(mode == PlayerItemBidDialogMode.Buy ? MessageType.PriceTooLow : MessageType.PriceTooHigh);
                this.uxRequestBidButton.Enabled = true;
            }         
        }

        public override void CloseThisDialog()
        {
            base.CloseThisDialog();

            this.uxRequestBidButton.Enabled = true;

            if (this.BeforeDialogClosed != null)
            {
                this.BeforeDialogClosed(this, new EventArgs());
            }

            InterfaceManager.Instance.MakeControlVisible(this, false);
        }

        private string GenerateMessage(MessageType messageType) 
        {
            string current = this.uxMessage.Text;
            switch (messageType)
            {
                case MessageType.PriceTooLow:
                    return this.FilterAndGetRandom(new string[] { "Aint no way I'm goin' that low!", 
                                                                  "Now *there* is an offer I CAN refuse!",
                                                                  "I'll lose my pants over this!", 
                                                                  "Too cheap!" }, current);
                case MessageType.PriceTooHigh:
                    return this.FilterAndGetRandom(new string[] { "Aint no way I'm goin' that high!", 
                                                                  "Now *there* is an offer I CAN refuse!",
                                                                  "I'll lose my pants over this!", 
                                                                  "Too much!" }, current);
                case MessageType.NoMoreBids:
                    return this.FilterAndGetRandom(new string[] { "Ok lady, no more of this!", 
                                                                  "I aint got all day! Door's that way!", 
                                                                  "It's time for my lunch break, door's that way!" }, current);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get a random item, excluding "toRemove".
        /// </summary>
        private string FilterAndGetRandom(string[] items, string toRemove)
        {
            return items.Where<string>(a => !a.Equals(toRemove)).GetRandomItem<string>();
        }

        private enum MessageType
        {
            PriceTooLow,
            NoMoreBids,
            PriceTooHigh,
        }
    }

    public enum PlayerItemBidDialogMode
    {
        Buy,
        Sell
    }
}
