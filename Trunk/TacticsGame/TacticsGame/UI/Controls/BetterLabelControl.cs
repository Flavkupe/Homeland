using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Visuals.Flat;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Managers;


namespace TacticsGame.UI.Controls
{
    public class BetterLabelControl : LabelControl
    {
        private bool mouseHovering = false;
        public bool MouseHovering { get { return this.mouseHovering; } }

        public BetterLabelControl() 
        {     
            this.TooltipText = null;                 
        }
        
        public Color? LabelColor { get; set; }
        public SpriteFont Font { get; set; }

        public string TooltipText { get; set; }

        protected override void OnMouseEntered()
        {
            this.mouseHovering = true;
        }

        protected override void OnMouseLeft()
        {
            this.mouseHovering = false;
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
    }

    public class BetterLabelControlRenderer : IFlatControlRenderer<BetterLabelControl>
    {
        public void Render(BetterLabelControl control, IFlatGuiGraphics graphics)
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();

            if (control.LabelColor.HasValue)
            {
                if (control.Font != null)
                {
                    graphics.DrawString("better.label", controlBounds, control.Text, control.LabelColor.Value, control.Font);
                }
                else
                {
                    graphics.DrawString("label", controlBounds, control.Text, control.LabelColor.Value, null);
                }
            }            
            else
            {             
                graphics.DrawString("label", controlBounds, control.Text);                
            }

            if (control.MouseHovering && !string.IsNullOrEmpty(control.TooltipText))
            {
                RectangleF areaUnderButton = new RectangleF(controlBounds.X, controlBounds.Bottom + 10.0f, controlBounds.Width, controlBounds.Height);
                RectangleF tooltipBounds = graphics.MeasureString("tooltip", areaUnderButton, control.TooltipText);

                if ((areaUnderButton.Y + tooltipBounds.Height) >= GameStateManager.Instance.CameraView.Height)
                {
                    // If the tooltip would come off the bottom, move it up.
                    areaUnderButton.Offset(0.0f, -10.0f - controlBounds.Height);
                }

                tooltipBounds.X = areaUnderButton.X;
                tooltipBounds.Y = areaUnderButton.Y;
                tooltipBounds.Inflate(6.0f, 0.0f);

                graphics.DrawElement("tooltip", tooltipBounds);
                graphics.DrawString("tooltip", tooltipBounds, control.TooltipText);
            } 
        }
    }
}
