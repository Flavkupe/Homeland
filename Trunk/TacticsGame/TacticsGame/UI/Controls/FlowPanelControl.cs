using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework;

namespace TacticsGame.UI
{
    /// <summary>
    /// Group that maintains position of items.
    /// </summary>
    public class FlowPanelControl : Control
    {
        private Margin margin;        

        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <param name="rectangle">Bounds of the whole panel.</param>
        /// <param name="margin">Margin around buttons... defaults to 3,3,3,3</param>
        public FlowPanelControl(UniRectangle rectangle, Margin margin = null)
        {
            this.Bounds = rectangle;

            this.margin = margin != null ? margin : new Margin(3, 3, 3, 3);

            currentX = this.Margin.Left;
            currentY = this.Margin.Top;
        }

        /// <summary>
        /// Space around the buttons. Left and Top define space between buttons and left/top walls. Right/bottom defines space between next control on right, next control on bottom.
        /// </summary>
        public Margin Margin
        {
            get 
            { 
                return margin; 
            }

            set             
            { 
                margin = value;
                this.ResetLayout();
            }
        }

        int currentX;
        int currentY;

        public void Clear()
        {
            this.currentX = this.Margin.Left;
            this.currentY = this.Margin.Top;
            this.Children.Clear();
        }

        /// <summary>
        /// Add a list of controls to the flow.
        /// </summary>
        /// <param name="controls"></param>
        public void AddControls(IEnumerable<Control> controls) 
        {
            foreach (Control control in controls)
            {
                this.AddControl(control);
            }
        }

        /// <summary>
        /// Add a control to the flow.
        /// </summary>
        /// <param name="control"></param>
        public void AddControl(Control control, Margin extraPadding = null, bool forceNewLine = false)
        {
            int extraLeft = extraPadding == null ? 0 : extraPadding.Left;
            int extraTop = extraPadding == null ? 0 : extraPadding.Top;
            int extraRight = extraPadding == null ? 0 : extraPadding.Right;
            int extraBottom = extraPadding == null ? 0 : extraPadding.Bottom;

            UniRectangle newBounds = new UniRectangle(this.currentX + extraLeft, currentY + extraTop, control.Bounds.GetWidth(), control.Bounds.GetHeight());

            if (this.currentX + newBounds.GetWidth() + this.Margin.Right > this.Bounds.GetWidth())
            {
                this.currentX = this.Margin.Left;
                this.currentY += control.Bounds.GetHeight() + this.Margin.Bottom + extraBottom;                                
            }

            control.Bounds = newBounds.RelocateClone(this.currentX + extraLeft, this.currentY);
            this.currentX += control.Bounds.GetWidth() + this.Margin.Right + extraRight;

            if (forceNewLine)
            {
                this.currentX = this.Margin.Left;
                this.currentY += control.Bounds.GetHeight() + this.Margin.Bottom + extraBottom;
            }

            this.Children.Add(control);
        }        

        /// <summary>
        /// Removes control, returns true if control existed.
        /// </summary>
        public bool RemoveControl(Control control)
        {
            if (this.Children.Contains(control))
            {
                this.Children.Remove(control);
                this.ResetLayout();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Replaces all the controls again.
        /// </summary>
        private void ResetLayout()
        {
            this.currentX = this.Margin.Left;
            this.currentY = this.Margin.Top;

            if (this.Children.Count > 0)
            {
                List<Control> temp = new List<Control>(this.Children);
                this.Children.Clear();
                this.AddControls(temp);
            }
        }
    }
}
