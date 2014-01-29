using System;
using System.Collections.Generic;
using System.Linq;
using Nuclex.UserInterface.Controls;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface;
using TacticsGame.Items;
using Microsoft.Xna.Framework;
using TacticsGame.GameObjects.Units;
using TacticsGame.AI.CombatMode;
using TacticsGame.Managers;

namespace TacticsGame.UI.Groups
{
    public class UnitActivityIconsGroup : Control
    {
        private ScrollableControlList uxIconGroup = null;

        public UnitActivityIconsGroup(UniRectangle bounds)
        {
            this.Bounds = bounds;

            this.uxIconGroup = new ScrollableControlList(64, 64, 24);
            this.uxIconGroup.Bounds = this.Bounds.RelocateClone(0.0f, 0.0f);
                
            this.Children.Add(uxIconGroup);
        }        

        public void ResetActivityIcons(IEnumerable<UnitManagementActivity> activities)
        {
            this.Children.Clear();

            this.AddActivityIcons(activities);
        }

        public event EventHandler<UnitClickedEventArgs> UnitClickedEvent;
        public event EventHandler<UnitActivityClickedEventArgs> UnitActivityClickedEvent;

        private void HandleActivityUnitIconPressed(object sender, EventArgs e)
        {
            if (this.UnitActivityClickedEvent != null)
            {
                UnitManagementActivity activity = ((TooltipButtonControl)sender).Tag as UnitManagementActivity;
                if (activity != null) 
                {                
                    this.UnitActivityClickedEvent(this, new UnitActivityClickedEventArgs(activity));
                }
            }
        }

        private void HandleUnitIconPressed(object sender, EventArgs e)
        {
            if (this.UnitClickedEvent != null)
            {
                Unit unit = ((TooltipButtonControl)sender).Tag as Unit;
                if (unit != null)
                {
                    this.UnitClickedEvent(this, new UnitClickedEventArgs(unit));
                }
            }
        }

        /// <summary>
        /// Clears all the icons.
        /// </summary>
        public void Clear()
        {
            this.uxIconGroup.Clear();
        }

        /// <summary>
        /// Reacts to a change in the Bounds property.
        /// </summary>
        public void UpdateBounds()
        {
            this.uxIconGroup.Bounds = this.Bounds.RelocateClone(0.0f, 0.0f);
            this.uxIconGroup.RefreshControls(true);
        }

        /// <summary>
        /// Adds multiple icons to the list.
        /// </summary>
        /// <param name="activities"></param>
        public void AddActivityIcons(IEnumerable<UnitManagementActivity> activities)
        {
            foreach (UnitManagementActivity activity in activities)
            {
                this.AddDecisionActivityIcon(activity, false);
            }

            this.uxIconGroup.RefreshControls();
        }

        /// <summary>
        /// Adds an icon to the list representing the activity.
        /// </summary>
        public void AddCombatActivityIcon(UnitCombatActivity activity, bool refresh = true)
        {
            Unit unit = activity.Unit;
            TooltipButtonControl newButton = new TooltipButtonControl();
            
            newButton.TooltipText = unit.DisplayName;
            newButton.SetIcon(unit.GetEntityIcon());
            newButton.Bounds = new UniRectangle(0, 0, 64, 64);
            newButton.Progress = (float)unit.CurrentStats.HP / (float)unit.BaseStats.HP;
            newButton.ProgressDisplayMode = TooltipButtonControl.ProgressMode.Bar;            
            newButton.Tag = activity.Unit;
            newButton.DisabledLook = activity.State != UnitState.Active;
            newButton.Pressed += this.HandleUnitIconPressed;
            newButton.Subtexture = activity.GetActivityIcon();
            newButton.MarkSelected = unit.Selected;

            // Draw AP
            this.DrawAPOrbs(unit.BaseStats.ActionPoints, unit.CurrentStats.ActionPoints, newButton);
            UnitActivityIcon icon = new UnitActivityIcon(newButton);
            this.uxIconGroup.AddControl(icon, refresh);
        }

        private void DrawAPOrbs(int baseAP, int currentAP, TooltipButtonControl newButton)
        {
            IconInfo greenOrb = TextureManager.Instance.GetIconInfo("GreenOrb");
            IconInfo blackOrb = TextureManager.Instance.GetIconInfo("BlackOrb");
            IconInfo yellowOrb = TextureManager.Instance.GetIconInfo("YellowOrb");

            newButton.Children.Clear();

            int x = 4;
            int y = 8;
            for (int i = 0; i < Math.Min(baseAP, currentAP); ++i)
            {
                IconControl orb = CreateOrbIcon(greenOrb, ref x, ref y);
                newButton.Children.Add(orb);
            }
            for (int i = 0; i < (baseAP - currentAP); ++i)
            {
                IconControl orb = CreateOrbIcon(blackOrb, ref x, ref y);
                newButton.Children.Add(orb);
            }
            for (int i = 0; i < (currentAP - baseAP); ++i)
            {
                IconControl orb = CreateOrbIcon(yellowOrb, ref x, ref y);
                newButton.Children.Add(orb);
            }
        }

        private IconControl CreateOrbIcon(IconInfo icon, ref int x, ref int y)
        {
            IconControl orb = new IconControl();
            orb.Icon = icon;
            orb.Bounds = new UniRectangle(x, y, 8, 8);
            x += 8;
            if (x > 56)
            {
                y += 8;
                x = 4;
            }

            return orb;
        }

        /// <summary>
        /// Adds an icon to the list representing the activity.
        /// </summary>
        public void AddDecisionActivityIcon(UnitManagementActivity activity, bool refresh = true)
        {
            TooltipButtonControl newButton = new TooltipButtonControl();
            newButton.TooltipText = activity.Unit.DisplayName;
            newButton.SetIcon(activity.Unit.GetEntityIcon());
            newButton.Bounds = new UniRectangle(0, 0, 64, 64);            
            newButton.Progress = 0.0f;            
            newButton.ProgressDisplayMode = TooltipButtonControl.ProgressMode.Bar;            
            newButton.Subtexture = activity.GetActivityIcon();
            newButton.Tag = activity;
            newButton.DisabledLook = activity.DoneForTurn;
            newButton.Pressed += this.HandleActivityUnitIconPressed;

            this.DrawAPOrbs(activity.Unit.BaseStats.ActionPoints, activity.Unit.CurrentStats.ActionPoints, newButton);

            UnitActivityIcon icon = new UnitActivityIcon(newButton);            

            this.uxIconGroup.AddControl(icon, refresh);
        }

        /// <summary>
        /// Updates the icon for when a decision activity changes
        /// </summary>
        /// <param name="activity"></param>
        public void UpdateActivityIcon(UnitManagementActivity activity)
        {
            foreach (UnitActivityIcon control in this.uxIconGroup.Controls)
            {                
                TooltipButtonControl button = control.Button;
                if (button.Tag == activity)
                {
                    button.Progress = 0.0f;
                    button.Subtexture = activity.GetActivityIcon();
                    button.DisabledLook = activity.DoneForTurn;
                    this.DrawAPOrbs(activity.Unit.BaseStats.ActionPoints, activity.Unit.CurrentStats.ActionPoints, button);
                    return;
                }                
            }
        }

        /// <summary>
        /// Shows visible result effects on the visible icons, such as the numbers going up
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="result"></param>
        public void UpdateResultOnActivityIcon(UnitManagementActivity activity, ActivityResult result)
        {
            foreach (UnitActivityIcon control in this.uxIconGroup.VisibleControls)
            {
                TooltipButtonControl button = control.Button;
                if (button.Tag == activity)
                {
                    int y = (int)button.Bounds.Top.Offset - 6;

                    if (result.ItemsGained != null)
                    {
                        HashSet<string> used = new HashSet<string>();
                        foreach (Item item in result.ItemsGained)
                        {
                            if (!used.Contains(item.ObjectName))
                            {
                                used.Add(item.ObjectName);
                                string text = "+ " + result.GainedItemsCounter.GetItemCount(item);
                                CrazyTextWithIcon crazyText = new CrazyTextWithIcon(text, item.Icon, 0, y);
                                control.Children.Add(crazyText);
                                y -= (int)crazyText.Bounds.Size.Y.Offset - 3;
                            }
                        }
                    }

                    if (result.ItemsLost != null)
                    {
                        HashSet<string> used = new HashSet<string>();
                        foreach (Item item in result.ItemsLost)
                        {
                            if (!used.Contains(item.ObjectName))
                            {
                                used.Add(item.ObjectName);
                                string text = "- " + result.LostItemsCounter.GetItemCount(item);
                                CrazyTextWithIcon crazyText = new CrazyTextWithIcon(text, item.Icon, 0, y);
                                control.Children.Add(crazyText);
                                y -= (int)crazyText.Bounds.Size.Y.Offset - 3;
                            }
                        }
                    }

                    if (result.MoneyMade.HasValue)
                    {
                        string text = "+ " + result.MoneyMade.Value;
                        CrazyTextWithIcon crazyText = new CrazyTextWithIcon(text, TextureManager.Instance.GetIconInfo("Coin"), 0, y, Color.Yellow);
                        control.Children.Add(crazyText);
                        y -= (int)crazyText.Bounds.Size.Y.Offset - 3;
                    }

                    if (result.MoneyLost.HasValue)
                    {
                        string text = "- " + result.MoneyLost.Value;
                        CrazyTextWithIcon crazyText = new CrazyTextWithIcon(text, TextureManager.Instance.GetIconInfo("Coin"), 0, y, Color.Red);
                        control.Children.Add(crazyText);
                        y -= (int)crazyText.Bounds.Size.Y.Offset - 3;
                    }

                    if (result.ActionPointCost < 0)
                    {
                        string text = "+ " + Math.Abs(result.ActionPointCost);
                        IconInfo icon = TextureManager.Instance.GetIconInfo(ResourceId.Icons.RunnyGuyIcon);
                        CrazyTextWithIcon crazyText = new CrazyTextWithIcon(text, icon, 0, y);
                        control.Children.Add(crazyText);
                        y -= (int)crazyText.Bounds.Size.Y.Offset - 3;
                    }

                    return;
                }
            }             
        }

        /// <summary>
        /// Removes first occurance of control whose tag matches "activity".
        /// </summary>
        /// <param name="activity"></param>
        public void RemoveActivityIcon(UnitManagementActivity activity)
        {            
            foreach (UnitActivityIcon control in this.uxIconGroup.Controls)
            {                
                TooltipButtonControl button = control.Button;
                if (button.Tag == activity)
                {
                    this.uxIconGroup.RemoveControl(control);
                    return;
                }                                        
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Control control in this.uxIconGroup.Controls)
            {                               
                TooltipButtonControl tooltipControl = ((UnitActivityIcon)control).Button;
                UnitManagementActivity activity = (UnitManagementActivity)tooltipControl.Tag;

                if (!activity.Complete && activity.Decision != Decision.Idle)
                {
                    tooltipControl.Progress = activity.PercentComplete;
                }

                ((UnitActivityIcon)control).Update(gameTime);
            }
        }

        public class UnitClickedEventArgs : EventArgs
        {
            public Unit Unit { get; set; }
            public UnitClickedEventArgs(Unit unit)
            {
                this.Unit = unit;
            }
        }

        public class UnitActivityClickedEventArgs : EventArgs
        {
            public UnitManagementActivity Activity { get; set; }
            public UnitActivityClickedEventArgs(UnitManagementActivity activity)
            {
                this.Activity = activity;
            }
        }

        private class UnitActivityIcon : Control
        {
            public TooltipButtonControl Button { get; set; }

            public UnitActivityIcon(TooltipButtonControl newButton)
            {
                this.Button = newButton;
                this.Bounds = this.Button.Bounds;
                this.Button.Bounds = new UniRectangle(0, 0, this.Button.Bounds.GetWidth(), this.Button.Bounds.GetHeight());

                this.Children.Add(this.Button);
            }

            public void Update(GameTime gameTime)
            {
                foreach (Control control in this.Children.ToList<Control>())
                {
                    if (control is CrazyTextWithIcon)
                    {
                        CrazyTextWithIcon crazyControl = (CrazyTextWithIcon)control;
                        crazyControl.Update(gameTime);
                        if (crazyControl.IsExpired)
                        {
                            this.Children.Remove(crazyControl);
                        }
                    }
                }
            }
        }
    }
}
