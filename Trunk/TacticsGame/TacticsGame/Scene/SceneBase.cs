using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TacticsGame.UI;
using TacticsGame.UI.Panels;
using TacticsGame.Managers;
using System.Diagnostics;
using TacticsGame.GameObjects.Units;
using TacticsGame.PlayerThings;

namespace TacticsGame.Scene
{
    [Serializable]
    public abstract class SceneBase : IDisposable
    {
        [NonSerialized]
        protected CommandPane commandPane;

        [NonSerialized]
        protected ActionFeedPanel actionFeed;
        
        // For serialization
        protected int cameraX;
        protected int cameraY;

        public SceneBase()
        {
        }

        [NonSerialized]
        private int framesCounted = 0;

        [NonSerialized]
        private int millisecondsPassed = 0;

        [NonSerialized]
        private string fps = string.Empty;

        [NonSerialized]
        private ButtonState priorStateLMB = ButtonState.Released;
        [NonSerialized]
        private ButtonState priorStateRMB = ButtonState.Released;
        
        [NonSerialized]
        private int lastScrollWheelValue = 0;

        public string GetFPS(GameTime gameTime) 
        {
            millisecondsPassed += gameTime.ElapsedGameTime.Milliseconds;
            framesCounted++;

            if (millisecondsPassed > 1000)
            {
                millisecondsPassed = 0;
                fps = framesCounted.ToString();
                framesCounted = 0;
            }

            return fps;
        }

        public abstract void Update(GameTime gameTime);                       

        public abstract void Draw(GameTime gameTime);

        public abstract void DrawUI(GameTime gameTime);

        public virtual void LoadContent()
        {
            GameStateManager.Instance.RelocateCamera(this.cameraX, this.cameraY);
        }     
        
        protected bool LMBReleased { get; private set; }
        protected bool LMBPressed { get; private set; }
        protected bool LMBHeld { get; private set; }
        protected bool RMBReleased { get; private set; }
        protected bool RMBPressed { get; private set; }
        protected bool MouseWheelUp { get; private set; }
        protected bool MouseWheelDown { get; private set; }

        protected int MouseX { get; private set; }
        protected int MouseY { get; private set; }
        protected int AbsoluteMouseX { get; private set; }
        protected int AbsoluteMouseY { get; private set; }
        

        /// <summary>
        /// Updates the mouse click status. Call after every Update!
        /// </summary>
        protected void UpdateMouseInput() 
        {
            MouseState state = Mouse.GetState();            
            
            this.LMBReleased = priorStateLMB == ButtonState.Pressed && state.LeftButton == ButtonState.Released;
            this.LMBPressed = priorStateLMB == ButtonState.Released && state.LeftButton == ButtonState.Pressed;
            this.LMBHeld = priorStateLMB == ButtonState.Pressed && state.LeftButton == ButtonState.Pressed;
           
            this.MouseWheelUp = state.ScrollWheelValue > this.lastScrollWheelValue;
            this.MouseWheelDown = state.ScrollWheelValue < this.lastScrollWheelValue;
            this.lastScrollWheelValue = state.ScrollWheelValue;

            this.RMBReleased = priorStateRMB == ButtonState.Pressed && state.RightButton == ButtonState.Released;
            this.RMBPressed = priorStateRMB == ButtonState.Released && state.RightButton == ButtonState.Pressed;

            this.MouseX = state.X;
            this.MouseY = state.Y;

            this.AbsoluteMouseX = GameStateManager.Instance.CameraView.X + this.MouseX;
            this.AbsoluteMouseY = GameStateManager.Instance.CameraView.Y + this.MouseY;

            this.priorStateLMB = state.LeftButton;
            this.priorStateRMB = state.RightButton;
        }

        public virtual void QuickLoad()
        {            
            GameStateManager.Instance.QuickLoad();
        }

        public virtual void QuickSave()
        {           
            GameStateManager.Instance.QuickSave(); 
        }

        public virtual void Dispose()
        {            
        }        
    }
}
