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
using TacticsGame.Managers;

namespace TacticsGame.UI.Controls
{
    /// <summary>
    /// Custom control with many many pieces of functionality like Tooltips, ProgressBars and Subtextures.
    /// </summary>
    public class TooltipButtonAndTextControl : ButtonControl
    {
        public TooltipButtonAndTextControl(IconInfo icon, string text, Vector2 location, int controlWidth, int? iconDimensions = null, int padding = 3, int additionalLeftPadding = 0)
        {
            this.ImageTexture = icon.SheetImage;
            this.ImageClip = icon.Clip;

            int dimensions = iconDimensions ?? icon.Dimensions;

            IconBounds = new RectangleF(additionalLeftPadding + padding, padding, dimensions, dimensions);            

            this.Text = text;

            this.Bounds = new UniRectangle(location.X, location.Y, controlWidth, IconBounds.Height + (2 * padding));
            this.TooltipDefaultsToTop = true;
        }

        public TooltipButtonAndTextControl(IconInfo icon, string text, int controlWidth, int? iconDimensions = null, int padding = 3, int additionalLeftPadding = 0)
            : this(icon, text, new Vector2(0,0), controlWidth, iconDimensions, padding, additionalLeftPadding)
        {
        }

        public enum ProgressMode
        {
            None,
            FullIconBack,
            FullIcon,
            Bar,            
        }

        public bool TooltipDefaultsToTop { get; set; }

        public bool ShowFrameOnImageButton { get; set; }

        public ProgressMode ProgressDisplayMode { get; set;}

        public new string Text { get; set; }

        public Color? TextColor { get; set; }

        public float? Progress { get; set; }

        public object Tag { get; set; }

        public string TooltipText { get; set; }

        /// <summary>
        /// If true, will paint a button that has an imageTexture using a darker tint. 
        /// </summary>
        public bool DisabledLook { get; set; }

        private TextureInfo subtexture = null;
        public RectangleF SubTextureBounds;
        
        public RectangleF IconBounds;

        /// <summary>
        /// Sets the subtexture for the button control... make sure to set the button bounds first.
        /// </summary>
        public TextureInfo Subtexture 
        { 
            get 
            {
                return subtexture;
            }
        }

        /// <summary>
        /// Sets the subtexture on the bottom right.
        /// </summary>
        /// <param name="texture">The texture to add.</param>
        /// <param name="adjustBounds">If true, will scale the subtexture to be a quarter the size of the button. If false, will leave it intact.</param>
        public void SetSubtexture(TextureInfo texture, bool adjustBounds)
        {
            this.subtexture = texture;

            if (adjustBounds)
            {
                int halfWidth = (int)this.Bounds.GetWidth() / 2;
                int halfHeight = (int)this.Bounds.GetHeight() / 2;
                this.SubTextureBounds = new RectangleF((float)(this.Bounds.GetWidth() - halfWidth), (float)(this.Bounds.GetHeight() - halfHeight), halfWidth, halfHeight);
            }
            else
            {                
                this.SubTextureBounds = new RectangleF((float)(this.Bounds.GetWidth() - texture.Width), (float)(this.Bounds.GetHeight() - texture.Height), texture.Width, texture.Height);
            }
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
    }
   
    public class TooltipButtonAndTextControlRenderer : IFlatControlRenderer<TooltipButtonAndTextControl>
    {
        public void Render(TooltipButtonAndTextControl control, IFlatGuiGraphics graphics)
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

            // Draw the progress behind the button.
            if (control.ProgressDisplayMode == TooltipButtonAndTextControl.ProgressMode.FullIconBack && control.Progress.HasValue && control.Progress.Value > 0.0)
            {
                //TODO: this is potentially a resource drain
                RectangleF progressBounds = control.GetAbsoluteBounds().ResizeClone((int)((float)control.Bounds.GetWidth() * control.Progress.Value), control.Bounds.GetHeight()); 
                graphics.DrawElement("list.selection", progressBounds);
            }

            // Draw the button's frame
            if (control.ShowFrameOnImageButton)
            {
                graphics.DrawElement(states[stateIndex], controlBounds);
            }
            
            if (control.ImageTexture != null)
            {                            
                if (control.DisabledLook)
                {
                    graphics.DrawElement(states[stateIndex], control.IconBounds.OffsetClone(controlBounds.X, controlBounds.Y), control.ImageTexture, control.ImageClip, Color.Black);
                }
                else
                {
                    graphics.DrawElement(states[stateIndex], control.IconBounds.OffsetClone(controlBounds.X, controlBounds.Y), control.ImageTexture, control.ImageClip);  // this line added by CSP                
                }
            }

            if (control.Subtexture != null)
            {
                //TODO: this is potentially a resource drain
                RectangleF subtextureBounds = control.SubTextureBounds.OffsetClone(controlBounds.X, controlBounds.Y);
                graphics.DrawElement(states[stateIndex], subtextureBounds, control.Subtexture.Texture, control.ImageClip);
            }

            // Draw the progress in front of the button.
            if (control.ProgressDisplayMode == TooltipButtonAndTextControl.ProgressMode.FullIcon && control.Progress.HasValue && control.Progress.Value > 0.0)
            {
                //TODO: this is potentially a resource drain
                RectangleF progressBounds = control.GetAbsoluteBounds().ResizeClone((int)((float)control.Bounds.GetWidth() * control.Progress.Value), control.Bounds.GetHeight());                        
                graphics.DrawElement(states[stateIndex], progressBounds);
            }

            // Draw the progress in front of the button.
            if (control.ProgressDisplayMode == TooltipButtonAndTextControl.ProgressMode.Bar && control.Progress.HasValue && control.Progress.Value > 0.0)
            {
                //TODO: this is potentially a resource drain
                // The 10 is because it needs at least 10 pixels in size to not look crappy.
                RectangleF progressBounds = control.GetAbsoluteBounds().ResizeClone(Math.Max((int)(controlBounds.Width * control.Progress), 10), Math.Max((int)(controlBounds.Height / 5.0f), 8)); 
                graphics.DrawElement("progressbar.red", progressBounds);
            }

            // If there's text assigned to the button, draw it next to the button
            if (!string.IsNullOrEmpty(control.Text))
            {
                if (control.TextColor.HasValue)
                {
                    graphics.DrawString("label", controlBounds.OffsetClone(control.IconBounds.Right + 3, 0), control.Text, control.TextColor.Value);
                }
                else
                {
                    graphics.DrawString("label", controlBounds.OffsetClone(control.IconBounds.Right + 3, 0), control.Text);                    
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
