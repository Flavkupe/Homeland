using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TacticsGame.Managers;
using TacticsGame.World;

namespace TacticsGame.Scene
{
    public class DaylightFilter
    {
        public DaylightFilter()
        {            
        }

        private float tintAmount = 0.0f;

        private float maxTint = 0.4f;

        public void Update(GameTime gameTime)
        {
            DateTime time = GameManager.World.WorldTime;

            int timeRange = GameWorld.DayEndHour - GameWorld.DayStartHour;
            int hoursPassed = time.Hour - GameWorld.DayStartHour;
            this.tintAmount = ((float)hoursPassed / (float)timeRange) * maxTint;
        }

        public void Draw(GameTime gameTime)
        {
            Utilities.DrawFixedRectangle(new Rectangle(0, 0, GameStateManager.Instance.CameraView.Width, GameStateManager.Instance.CameraView.Height), new Color(0.0f, 0.0f, 0.0f, this.tintAmount));
        }
    }
}
