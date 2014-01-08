using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Visuals.Flat;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using Nuclex.Support.Collections;
using Nuclex.Input;
using Nuclex.UserInterface.Source.Controls;
using System.Diagnostics;
using TacticsGame.Managers;

namespace TacticsGame.UI.Controls
{
    public class ScrollableControlList : FramePanelControl, IFocusable, IStealsMouseWheelInput
    {
        ButtonControl uxDownButton = new ButtonControl();
        ButtonControl uxUpButton = new ButtonControl();
        Control uxInternalControls = new Control();
        VerticalSliderControl uxSlider = new VerticalSliderControl();
        IconControl uxSelection = new IconControl();

        private List<Control> controls = new List<Control>();
       


        private int currentIncrement = 0;
        private int widthOfControls;
        private int heightOfControls;        
        private int paddingLeft = 0;
        private int paddingTop = 0;
        private Control selection = null;

        public ScrollableControlList(int widthOfControls, int heightOfControls, int paddingLeft = 0, int paddingTop = 0, float scrollbarWidth = 26.0f)
        {
            this.paddingLeft = paddingLeft;
            this.paddingTop = paddingTop;
            this.uxUpButton.Bounds = new UniRectangle(new UniScalar(1.0f, -scrollbarWidth), new UniScalar(0.0f, 0.0f), scrollbarWidth, 16.0f);
            this.uxDownButton.Bounds = new UniRectangle(new UniScalar(1.0f, -scrollbarWidth), new UniScalar(1.0f, -16.0f), scrollbarWidth, 16.0f);
            this.uxInternalControls.Bounds = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(0.0f, 0.0f), new UniScalar(1.0f, -scrollbarWidth), new UniScalar(1.0f, 0.0f));
            this.uxSlider.Bounds = new UniRectangle(new UniScalar(1.0f, -scrollbarWidth), new UniScalar(0.0f, 16.0f), scrollbarWidth, new UniScalar(1.0f, -scrollbarWidth));

            // TODO: change this to use UI elements rather than a cursor icon
            this.uxSelection.Icon = TextureManager.Instance.GetTextureAsIconInfo("Cursor", ResourceType.MiscObject);

            this.uxSlider.Moved += new EventHandler(uxSlider_Moved);            

            this.uxUpButton.Pressed += new EventHandler(uxUpButton_Pressed);
            this.uxDownButton.Pressed += new EventHandler(uxDownButton_Pressed);

            this.widthOfControls = widthOfControls;
            this.heightOfControls = heightOfControls;

            this.Children.Add(uxDownButton);
            this.Children.Add(uxUpButton);
            this.Children.Add(uxInternalControls);
            this.Children.Add(uxSlider);
        }

        /// <summary>
        /// If true, pressable buttons will get selected.
        /// </summary>
        public bool AllowSelection { get; set; }        

        public Control Selection
        {
            get { return selection; }
            set { selection = value; }
        }

        /// <summary>
        /// Gets list of all controls in the scrollable area, including not visible.
        /// </summary>
        public IEnumerable<Control> Controls { get { return this.controls; } }

        /// <summary>
        /// Gets all controls currently visible
        /// </summary>
        public IEnumerable<Control> VisibleControls { get { return this.uxInternalControls.Children; } }
        
        /// <summary>
        /// The number of rows we can show right now, based on height of the bounds and height of the controls. 
        /// </summary>
        private int TotalRowsThatFit 
        { 
            get 
            { 
                return this.uxInternalControls.Bounds.GetHeight(this.Bounds) / this.heightOfControls; 
            } 
        }
        
        /// <summary>
        /// Number of additional increments of the scroll bar that are necessary to be able to display everything. Essentially it's the total number of rows needed in addition to the
        /// first set displayed.
        /// </summary>
        private int IncrementsNeeded 
        { 
            get 
            {
                return (int)Math.Ceiling((double)this.controls.Count / (double)this.ControlsPerRow) - this.MaxRowsToDisplay; 
            } 
        }

        /// <summary>
        /// Total rows needed to display everything.
        /// </summary>
        private int TotalRowsNeeded
        {
            get 
            { 
                return (int)Math.Ceiling((double)this.controls.Count / (double)this.ControlsPerRow); 
            }
        }

        /// <summary>
        /// Number of controls that fit in each row, based on their width.
        /// </summary>
        private int ControlsPerRow 
        { 
            get 
            { 
                return (int)((float)this.uxInternalControls.Bounds.GetWidth(this.Bounds) / (float)this.widthOfControls); 
            } 
        }

        /// <summary>
        /// Max number of rows that should be displayed. Essentially, it's either all the rows that fit or just the ones necessary.
        /// </summary>
        private int MaxRowsToDisplay
        {
            get 
            { 
                return Math.Min(this.TotalRowsThatFit, this.TotalRowsNeeded); 
            }
        }

        private void uxSlider_Moved(object sender, EventArgs e)
        {
            this.UpdateIncrementToSlider();
        }

        private void UpdateIncrementToSlider()
        {
            float inc = this.uxSlider.ThumbPosition * (float)IncrementsNeeded;
            if (inc != this.currentIncrement)
            {
                this.currentIncrement = (int)inc;
                this.RefreshControls();
            }
        }

        protected override void OnMouseWheel(float ticks)
        {
            float starting = this.uxSlider.ThumbPosition;
            this.uxSlider.ThumbPosition -= ticks;
            this.uxSlider.ThumbPosition = this.uxSlider.ThumbPosition.GetClampedValue(0.0f, 1.0f);

            if (starting != this.uxSlider.ThumbPosition)
            {
                this.UpdateIncrementToSlider();
                this.RefreshControls();
            }
        }

        void uxDownButton_Pressed(object sender, EventArgs e)
        {            
            if (this.currentIncrement < this.IncrementsNeeded)
            {
                this.currentIncrement++;
                this.RefreshControls();
            }
        }

        void uxUpButton_Pressed(object sender, EventArgs e)
        {
            if (this.currentIncrement > 0)
            {
                this.currentIncrement--;
                this.RefreshControls();
            }
        }

        public void Clear()
        {
            this.controls.Clear();
            this.uxInternalControls.Children.Clear();
        }

        public void AddControl(Control newControl, bool refresh = false)
        {
            this.controls.Add(newControl);
            this.uxInternalControls.Children.Add(newControl);

            if (refresh)
            {
                this.RefreshControls();
            }
        }

        public void RemoveControl(Control control, bool refresh = true)
        {
            this.controls.Remove(control);
            this.uxInternalControls.Children.Remove(control);

            if (refresh)
            {
                this.RefreshControls();
            }            
        }

        public void AddControls(IEnumerable<Control> newControls, bool refresh = true)
        {
            this.controls.AddRange(newControls);
            this.uxInternalControls.Children.AddRangeIfNotExists(newControls);

            if (refresh)
            {
                this.RefreshControls();
            }
        }

        protected override void OnMousePressed(Nuclex.Input.MouseButtons button)
        {
            if (button == Nuclex.Input.MouseButtons.Right)
            {
                if (this.Parent is ICloseOnRightClick)
                {
                    ((ICloseOnRightClick)this.Parent).CloseThisDialog();
                    return;
                }
            }

            base.OnMousePressed(button);
        }

        public void RefreshControls(bool clearIncrement = false)
        {
            if (clearIncrement)
            {
                this.currentIncrement = 0;
            }

            this.uxInternalControls.Children.Clear();            
            int controlsPerRow = this.ControlsPerRow;
            int startIndex = this.currentIncrement * controlsPerRow;
            int endIndex = (TotalRowsThatFit * controlsPerRow) + startIndex - 1;
            
            endIndex = Math.Min(endIndex, this.controls.Count - 1);

            int x = paddingLeft;
            int y = paddingTop;
            int itemsThisRow = 0;
            for (int i = startIndex; i <= endIndex; ++i)
            {
                Control current = this.controls[i];
                current.Bounds = current.Bounds.RelocateClone(x, y);
                x += this.widthOfControls;
                this.uxInternalControls.Children.Add(current);

                if (this.AllowSelection && current is IPressable)
                {
                    // Just in case
                    ((IPressable)current).Pressed -= this.HandleControlSelected;
                    ((IPressable)current).Pressed += this.HandleControlSelected;
                }

                current.BringToFront();
                itemsThisRow++;
                if (itemsThisRow >= controlsPerRow)
                {
                    itemsThisRow = 0;
                    y += this.heightOfControls;
                    x = paddingLeft;
                }
            }

            this.uxSlider.ThumbSize = ((float)((float)this.TotalRowsThatFit / (float)this.TotalRowsNeeded)).GetClampedValue(0.1f, 1.0f);
            this.uxSlider.ThumbPosition = this.IncrementsNeeded == 0 ? 0.0f : ((float)this.currentIncrement / (float)IncrementsNeeded).GetClampedValue(0.0f, 1.0f);

        }

        private void HandleControlSelected(object sender, EventArgs e)
        {
            Debug.Assert(sender is Control);
            this.Selection = sender as Control;            
            this.uxSelection.Bounds = ((Control)sender).Bounds;
            this.SetControlVisible(this.uxSelection, true);
        }

        public bool CanGetFocus
        {
            get { return true; }
        }

        private bool stealsInput = true;
        public bool StealsInput 
        {
            get { return this.stealsInput; }
            set { this.stealsInput = value; }
        }
    }
}