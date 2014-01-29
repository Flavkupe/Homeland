using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Controls;
using TacticsGame.UI.Groups;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.Units;
using TacticsGame.AI.CombatMode;

namespace TacticsGame.UI.Panels
{
    public class ActionFeedPanel : CollapsiblePanel
    {
        private const int MaxFeed = 20;
        private bool iconMode = false;

        public ActionFeedPanel(UniRectangle startingBounds, UniRectangle collapsedBounds)
            : base(startingBounds, collapsedBounds)
        {
            this.SetBounds();

            this.uxClearButton.Text = "Clear";
            this.uxClearButton.Pressed += this.HandleClearClicked;

            this.uxIconModeButton.Text = "View Activity";
            this.uxIconModeButton.Pressed += this.HandleToggleIconMode;

            this.uxFeedList.SelectionMode = ListSelectionMode.None;

            this.uxActivityGroup = new UnitActivityIconsGroup(this.uxFeedList.Bounds);
            this.uxActivityGroup.UnitActivityClickedEvent += this.HandleUnitActivityClickedEvent;
            this.uxActivityGroup.UnitClickedEvent += this.HandleUnitClickedEvent;

            this.Children.Add(uxFeedList);
            this.Children.Add(uxClearButton);
            this.Children.Add(uxIconModeButton);
        }

        private void HandleUnitClickedEvent(object sender, UnitActivityIconsGroup.UnitClickedEventArgs e)
        {
            if (this.UnitIconClicked != null)
            {
                this.UnitIconClicked(this, e);
            }            
        }

        public event EventHandler<TacticsGame.UI.Groups.UnitActivityIconsGroup.UnitActivityClickedEventArgs> ActivityIconClicked;
        public event EventHandler<TacticsGame.UI.Groups.UnitActivityIconsGroup.UnitClickedEventArgs> UnitIconClicked;

        private void HandleUnitActivityClickedEvent(object sender, TacticsGame.UI.Groups.UnitActivityIconsGroup.UnitActivityClickedEventArgs e)
        {
            if (this.ActivityIconClicked != null)
            {
                this.ActivityIconClicked(this, e);
            }
        } 

        public void SetBounds()
        {
            this.uxFeedList.Bounds = new UniRectangle(new UniVector(this.Bounds.Left + 6, 26), new UniVector(this.Bounds.Size.X - 12, this.Bounds.Size.Y - 32));

            if (this.uxActivityGroup != null)
            {
                this.uxActivityGroup.Bounds = this.uxFeedList.Bounds;
                this.uxActivityGroup.UpdateBounds();
            }

            this.uxClearButton.Bounds = new UniRectangle(new UniVector(3.0f, 3.0f), new UniVector(50.0f, 20.0f));
            this.uxIconModeButton.Bounds = new UniRectangle(new UniVector(this.uxClearButton.Bounds.Right + 3.0f, 3.0f), new UniVector(100.0f, 20.0f));
        }

        public void ClearContents()
        {
            this.uxFeedList.Items.Clear();
        }

        private void HandleToggleIconMode(object sender, EventArgs e)
        {
            iconMode = !iconMode;

            if (iconMode)
            {
                this.SetControlVisible(this.uxActivityGroup, true);
                this.SetControlVisible(this.uxFeedList, false);
                this.SetControlVisible(this.uxClearButton, false);
                this.uxActivityGroup.BringToFront();
                this.uxIconModeButton.Text = "View Text";                
            }
            else
            {
                this.SetControlVisible(this.uxActivityGroup, false);
                this.SetControlVisible(this.uxFeedList, true);
                this.SetControlVisible(this.uxClearButton, true);
                this.uxFeedList.BringToFront();
                this.uxIconModeButton.Text = "View Activity";                
            }
        }

        private void HandleClearClicked(object sender, EventArgs e)
        {
            this.ClearContents();
        }

        protected override void ToggleBigger()
        {
            base.ToggleBigger();

            this.SetBounds();
        }

        public void AddToFeed(string text)
        {         
            if (uxFeedList.Items.Count > MaxFeed)
            {
                this.uxFeedList.Items.RemoveAt(0);
            }
            
            this.uxFeedList.Items.Add(text);
            this.uxFeedList.Slider.ThumbPosition = 1.0f;
        }        

        private ListControl uxFeedList = new ListControl();
        private ButtonControl uxClearButton = new ButtonControl();
        private ButtonControl uxIconModeButton = new ButtonControl();
        private UnitActivityIconsGroup uxActivityGroup;

        public void AddToFeed(UnitManagementActivity activity)
        {
            this.uxActivityGroup.AddDecisionActivityIcon(activity);
        }

        public void AddToFeed(UnitCombatActivity activity)
        {
            this.uxActivityGroup.AddCombatActivityIcon(activity);
        }

        public void RemoveFromFeed(UnitManagementActivity activity)
        {
            this.uxActivityGroup.RemoveActivityIcon(activity);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.uxActivityGroup.Update(gameTime);
        }

        public void ClearUnitView()
        {
            this.uxActivityGroup.Clear();
        }

        /// <summary>
        /// Updates the existing icon to account for a new activity change.
        /// </summary>
        /// <param name="activity"></param>
        public void UpdateActivity(UnitManagementActivity activity)
        {
            this.uxActivityGroup.UpdateActivityIcon(activity);    
        }

        public void UpdateResultOnActivity(UnitManagementActivity activity, ActivityResult result)
        {
            this.uxActivityGroup.UpdateResultOnActivityIcon(activity, result);    
        }        
    }
}
