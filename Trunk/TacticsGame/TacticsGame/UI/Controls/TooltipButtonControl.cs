using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Visuals.Flat;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace TacticsGame.UI.Controls
{
    /// <summary>
    /// Custom control with many many pieces of functionality like Tooltips, ProgressBars and Subtextures.
    /// </summary>
    public class TooltipButtonControl : ButtonControl
    {
        public TooltipButtonControl()
        {
            this.TooltipDefaultsToTop = true;
        }

        public enum ProgressMode
        {
            None,
            FullIconBack,
            FullIcon,
            Bar,            
        }

        public bool ShowFrameOnImageButton { get; set; }

        public ProgressMode ProgressDisplayMode { get; set;}        

        public float? Progress { get; set; }

        public object Tag { get; set; }

        public string TooltipText { get; set; }

        public bool TooltipDefaultsToTop { get; set; }

        public bool MarkSelected { get; set; }

        /// <summary>
        /// If true, will paint a button that has an imageTexture using a darker tint. 
        /// </summary>
        public bool DisabledLook { get; set; }

        private IconInfo subtexture = null;
        public RectangleF SubTextureBounds;

        /// <summary>
        /// Sets the subtexture for the button control... make sure to set the button bounds first.
        /// </summary>
        public IconInfo Subtexture 
        { 
            get 
            {
                return subtexture;
            }
            
            set
            {
                this.subtexture = value;
                Debug.Assert(this.ImageTexture != null, "Subtexture needs imageTexture set!");

                int halfWidth = (int)this.Bounds.GetWidth() / 2;
                int halfHeight = (int)this.Bounds.GetHeight() / 2;                 
                this.SubTextureBounds = new RectangleF(halfWidth, halfHeight, halfWidth, halfHeight);                
            }
        }

        public void SetIcon(IconInfo icon)
        {
            this.ImageTexture = icon.SheetImage;
            this.ImageClip = icon.Clip;
        }
    }    
   
    public class TooltipButtonControlRenderer : IFlatControlRenderer<TooltipButtonControl>
    {
        public void Render(TooltipButtonControl control, IFlatGuiGraphics graphics)
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();

            // Determine the style to use for the button
            int stateIndex = 0;
            if (control.Enabled)
            {
                if (control.Depressed)
                {
                    stateIndex = 3;
                }
                else if (control.MouseHovering || control.HasFocus)
                {
                    stateIndex = 2;
                }
                else
                {
                    stateIndex = 1;
                }
            }

            if (control.MarkSelected)
            {
                graphics.DrawElement("input.highlighted", controlBounds);                
            }

            // Draw the progress behind the button.
            if (control.ProgressDisplayMode == TooltipButtonControl.ProgressMode.FullIconBack && control.Progress.HasValue && control.Progress.Value > 0.0)
            {
                //TODO: this is potentially a resource drain
                RectangleF progressBounds = controlBounds.ResizeClone((int)((float)control.Bounds.GetWidth() * control.Progress.Value), control.Bounds.GetHeight()); 
                graphics.DrawElement("list.selection", progressBounds);
            }

            // Draw the button's frame
            if (control.ImageTexture == null)
            {
                if (control.ShowFrameOnImageButton)
                {
                    graphics.DrawElement(states[stateIndex], controlBounds);
                }
            }
            else
            {
                if (control.ShowFrameOnImageButton)
                {
                    graphics.DrawElement(states[stateIndex], controlBounds);
                }

                if (control.DisabledLook)
                {
                    graphics.DrawElement(states[stateIndex], controlBounds, control.ImageTexture, control.ImageClip, Color.Black);
                }
                else
                {
                    graphics.DrawElement(states[stateIndex], controlBounds, control.ImageTexture, control.ImageClip);  // this line added by CSP                
                }
            }

            if (control.Subtexture != null)
            {
                //TODO: this is potentially a resource drain
                RectangleF subtextureBounds = control.SubTextureBounds.OffsetClone((int)controlBounds.X, (int)controlBounds.Y);
                graphics.DrawElement(states[stateIndex], subtextureBounds, control.Subtexture.SheetImage, control.Subtexture.Clip);
            }

            // Draw the progress in front of the button.
            if (control.ProgressDisplayMode == TooltipButtonControl.ProgressMode.FullIcon && control.Progress.HasValue && control.Progress.Value > 0.0)
            {
                //TODO: this is potentially a resource drain
                RectangleF progressBounds = controlBounds.ResizeClone((int)((float)control.Bounds.GetWidth() * control.Progress.Value), control.Bounds.GetHeight());                        
                graphics.DrawElement(states[stateIndex], progressBounds);
            }

            // Draw the progress in front of the button.
            if (control.ProgressDisplayMode == TooltipButtonControl.ProgressMode.Bar && control.Progress.HasValue && control.Progress.Value > 0.0)
            {
                //TODO: this is potentially a resource drain
                // The 10 is because it needs at least 10 pixels in size to not look crappy.
                RectangleF progressBounds = controlBounds.ResizeClone(Math.Max((int)(controlBounds.Width * control.Progress), 10), Math.Max((int)(controlBounds.Height / 5.0f), 8)); 
                graphics.DrawElement("progressbar.red", progressBounds);
            }

            // If there's text assigned to the button, draw it into the button
            if (!string.IsNullOrEmpty(control.Text))
            {
                // Draw in easier to see color if there's an image.
                if (control.ImageTexture != null)
                {
                    graphics.DrawString("tooltip", controlBounds, control.Text);
                }
                else
                {
                    graphics.DrawString(states[stateIndex], controlBounds, control.Text);
                }
            }

            if (control.MouseHovering && !string.IsNullOrEmpty(control.TooltipText))
            {
                RectangleF targetArea = control.TooltipDefaultsToTop ? new RectangleF(controlBounds.X, controlBounds.Top - 15.0f, controlBounds.Width, controlBounds.Height) :
                                                                       new RectangleF(controlBounds.X, controlBounds.Bottom + 15.0f, controlBounds.Width, controlBounds.Height);

                if ((targetArea.Y + targetArea.Height) >= GameStateManager.Instance.CameraView.Height)
                {
                    // If the tooltip would come off the bottom, move it up.
                    targetArea.Offset(0.0f, -30.0f - controlBounds.Height);                    
                }
                else if (targetArea.Y < 0)
                {
                    // If the tooltip would come off the top, move it down
                    targetArea.Offset(0.0f, 30.0f + controlBounds.Height);
                }

                RectangleF tooltipBounds = graphics.MeasureString("tooltip", targetArea, control.TooltipText);
                tooltipBounds.X = targetArea.X;
                tooltipBounds.Y = targetArea.Y;  
                tooltipBounds.Inflate(6.0f, 0.0f);

                graphics.DrawElement("tooltip", tooltipBounds);
                graphics.DrawString("tooltip", tooltipBounds, control.TooltipText);
            }            
        }

        /// <summary>Names of the states the button control can be in</summary>
        /// <remarks>
        ///   Storing this as full strings instead of building them dynamically prevents
        ///   any garbage from forming during rendering.
        /// </remarks>
        private static readonly string[] states = new string[] 
        {
            "button.disabled",
            "button.normal",
            "button.highlighted",
            "button.depressed"
        };
    }    
}
