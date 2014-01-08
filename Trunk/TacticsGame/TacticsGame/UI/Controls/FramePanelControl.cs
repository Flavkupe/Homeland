using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Visuals.Flat;
using Nuclex.UserInterface;

namespace TacticsGame.UI.Controls
{
    /// <summary>
    /// A panel without the thick top part
    /// </summary>
    public class FramePanelControl : Control
    {
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

    public class FramePanelControlRenderer : IFlatControlRenderer<FramePanelControl>
    {
        public void Render(FramePanelControl control, IFlatGuiGraphics graphics)
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();
            graphics.DrawElement("frame.panel", controlBounds);
        }
    }            
}

