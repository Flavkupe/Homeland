using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nuclex.UserInterface;
using TacticsGame.Managers;

namespace TacticsGame.UI.Controls
{
    public class CrazyTextWithIcon : Control
    {
        private int maxDuration; 
        private int duration;

        private bool isExpired = false;

        private BetterLabelControl uxLabel = new BetterLabelControl();
        private IconControl uxIcon = new IconControl();

        public CrazyTextWithIcon(string text, IconInfo icon, int startingX = 0, int startingY = 0, Color? color = null, int duration = 1500)
        {
            this.AffectsOrdering = false;            

            this.maxDuration = duration;
            this.duration = 0;

            this.uxIcon.Icon = icon;
            this.uxLabel.Text = text;
            this.uxLabel.AffectsOrdering = false;        
    
            Vector2 size = TextureManager.Instance.GetTextSize(text);

            this.uxLabel.Bounds = new Nuclex.UserInterface.UniRectangle(0, 0, size.X, size.Y);
            this.uxLabel.Font = TextureManager.Instance.DebugFont;
            
            this.uxLabel.LabelColor = color.HasValue ? color.Value : Color.Black;            

            // make the size of the icon YxY based on the font height so it aligns well and looks ok.
            this.uxIcon.Bounds = new Nuclex.UserInterface.UniRectangle(this.uxLabel.Bounds.Right + 12, 3, size.Y, size.Y);

            this.Bounds = new UniRectangle(startingX, startingY, this.uxLabel.Bounds.Size.X + 12 + this.uxIcon.Bounds.Size.X, size.Y);
            
            this.Children.Add(this.uxIcon);
            this.Children.Add(this.uxLabel);

            this.BringToFront();
        }

        public bool IsExpired
        {
            get { return isExpired; }
        }

        public void Update(GameTime gameTime)
        {
            this.Bounds = this.Bounds.NudgeClone(0.0f, -1.0f);

            duration += gameTime.ElapsedGameTime.Milliseconds;
            if (duration > maxDuration)
            {
                this.isExpired = true;     
            }

            //this.BringToFront();
        }
    }
}
