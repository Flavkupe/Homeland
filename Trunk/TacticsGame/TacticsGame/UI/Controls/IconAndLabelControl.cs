using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Visuals.Flat;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework;

namespace TacticsGame.UI.Controls
{
    public class IconAndLabelControl : TooltipButtonControl
    {

        IconControl uxIcon = new IconControl();
        LabelControl uxLabelControl = new LabelControl();

        public IconAndLabelControl(IconInfo icon, string text, Vector2 location, int controlWidth, int? iconDimensions = null, int padding = 0)
        {
            this.uxIcon.Icon = icon;
            this.uxLabelControl.Text = text;

            if (iconDimensions.HasValue) 
            {
                this.uxIcon.Bounds = new UniRectangle(3, 3, iconDimensions.Value, iconDimensions.Value);
            }
            else
            {
                this.uxIcon.Bounds = new UniRectangle(3, 3, icon.Dimensions, icon.Dimensions);
            }

            this.Bounds = new UniRectangle(location.X, location.Y, controlWidth, (int)this.uxIcon.Bounds.GetHeight() + (2 * padding));
            this.uxLabelControl.Bounds = new UniRectangle(this.uxIcon.Bounds.Right + 3, padding, this.Bounds.GetWidth() - this.uxIcon.Bounds.GetWidth() - padding, this.Bounds.Size.Y);            

            this.Children.Add(uxIcon);
            this.Children.Add(uxLabelControl);
        }

        protected override void OnMousePressed(Nuclex.Input.MouseButtons button)
        {
            if (!ShowFrameOnImageButton && button == Nuclex.Input.MouseButtons.Right)
            {
                if (this.Parent is ICloseOnRightClick)
                {
                    ((ICloseOnRightClick)this.Parent).CloseThisDialog();
                    return;
                }
            }

            base.OnMousePressed(button);
        }  

        protected override void OnMouseEntered()
        {
            base.OnMouseEntered();
        }

        protected override void OnMouseLeft()
        {
            base.OnMouseLeft();
        }
    }
}
