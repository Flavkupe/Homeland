using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TacticsGame.Utility;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.GameObjects;

namespace TacticsGame.Text
{
    public class FloatingText : IExpire, ISubSprite
    {
        public Vector2 DrawPosition { get; set; }

        private Rectangle iconBounds;

        public string Text { get; set; }

        public Color Color { get; set; }

        private float duration { get; set; }

        private float currentDuration = 0.0f;

        private IconInfo icon = null;        

        private bool travel;

        private float scale;

        private int textWidth;

        public FloatingText(string text, Vector2 drawPosition, IconInfo icon = null, float duration = 2000, Color? color = null, bool travel = true, float scale = 1.0f)
        {
            this.DrawPosition = drawPosition;
            this.Text = text;
            this.duration = duration;
            this.travel = travel;
            this.scale = scale;
            this.Color = color == null ? Color.Red : color.Value;
            this.icon = icon;
            
            if (icon != null)
            {
                this.textWidth = text == null ? 0 : (int)(TextureManager.Instance.GetTextSize(this.Text).X * scale);
                iconBounds = new Rectangle((int)this.DrawPosition.X + this.textWidth, (int)this.DrawPosition.Y, 32, 32);
            }
        }

        public bool IsExpired { get { return this.currentDuration > this.duration; } }

        public virtual void Update(GameTime gameTime)
        {
            this.currentDuration += gameTime.ElapsedGameTime.Milliseconds;

            if (this.travel)
            {                
                this.DrawPosition = new Vector2(this.DrawPosition.X, this.DrawPosition.Y - 0.4f);

                if (this.icon != null)
                {
                    this.iconBounds = this.iconBounds.CloneAndRelocate(this.iconBounds.X, (int)(this.DrawPosition.Y - 0.4f));
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (this.Text != null)
            {
                Color color = this.Color;
                //color.A = (byte)(this.currentDuration / duration);
                Utilities.DrawText(this.Text, this.DrawPosition, color, this.scale);
            }

            if (this.icon != null)
            {
                Utilities.DrawTexture2D(this.icon.SheetImage, iconBounds, this.icon.Clip, null, 0.0f, this.scale);
            }
        }

        /// <summary>
        /// Gets floating text at a random horizontal position on the given drawPosition rectangle.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="drawPosition"></param>
        /// <returns></returns>
        public static FloatingText CreateRandomlyHorizontalFloatingText(string text, Rectangle drawPosition)
        {
            double range = Utilities.RandomGenerator.NextDouble();
            int horizontalPos = drawPosition.Left + (int)(range * (double)drawPosition.Width);
            Vector2 textPosition = new Vector2(horizontalPos, drawPosition.Top);
            return new FloatingText(text, textPosition, null, 2000, null, true, 0.8f);        
        }

        public void AddSubSprite(ISubSprite subsprite)
        {
            // Ignore
        }

        public void UpdateSprite(GameTime time)
        {
            this.Update(time);
        }

        public void SetDrawPosition(int x, int y)
        {
            this.DrawPosition = new Vector2(x, y);
        }

        public void ShiftDrawPosition(int x, int y)
        {
            this.DrawPosition = new Vector2(this.DrawPosition.X + x, this.DrawPosition.Y + y);
            this.iconBounds = this.iconBounds.CloneAndOffset(x, y);
        }
    }
}
