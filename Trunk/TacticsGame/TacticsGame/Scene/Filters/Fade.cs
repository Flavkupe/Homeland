using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TacticsGame.Managers;

namespace TacticsGame.Scene
{
    public class Fade
    {
        public Fade(float fadeTarget = 500.0f)
        {            
            this.fadeTarget = fadeTarget;
        }

        private bool fadingBackwards = false;
        private float fadeAmount = 0.0f;
        private float fadeTarget = 500.0f; // ms

        public bool IsDone
        {
            get { return this.fadingBackwards && fadeAmount <= 0.0f; }
        }

        public void Reset()
        {
            this.fadingBackwards = false;
            this.fadeAmount = 0.0f;
        }

        public void Update(GameTime gameTime)
        {
            if (this.fadingBackwards)
            {
                this.fadeAmount -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                this.fadeAmount += gameTime.ElapsedGameTime.Milliseconds;
            }

            if (this.fadeAmount > fadeTarget)
            {
                this.fadingBackwards = true;
            }
        }

        public void Draw(GameTime gameTime)
        {
            Utilities.DrawFixedRectangle(new Rectangle(0, 0, GameStateManager.Instance.CameraView.Width, GameStateManager.Instance.CameraView.Height), new Color(0.0f, 0.0f, 0.0f, fadeAmount / fadeTarget));
        }
    }
}
