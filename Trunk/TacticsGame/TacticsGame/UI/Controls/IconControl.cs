using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.UserInterface.Visuals.Flat;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework;

namespace TacticsGame.UI.Controls
{
    public class IconControl : Control
    {
        public IconInfo Icon { get; set; }
        public Color? Color { get; set; }
    }

    public class IconControlRenderer : IFlatControlRenderer<IconControl>
    {
        public void Render(IconControl control, IFlatGuiGraphics graphics)
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();

            // Don't think frame matters here...
            if (control.Color.HasValue)
            {
                graphics.DrawElement("button.normal", controlBounds, control.Icon.SheetImage, control.Icon.Clip, control.Color.Value);
            }
            else
            {
                graphics.DrawElement("button.normal", controlBounds, control.Icon.SheetImage, control.Icon.Clip);
            }
        }
    }
}
