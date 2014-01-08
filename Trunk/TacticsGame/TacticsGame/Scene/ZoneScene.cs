using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Utility;
using TacticsGame.Map;
using Microsoft.Xna.Framework.Input;
using Nuclex.UserInterface;
using TacticsGame.UI;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects;
using TacticsGame.GameObjects.Obstacles;
using System.Diagnostics;
using TacticsGame.Text;
using TacticsGame.AI;
using TacticsGame.Abilities;
using TacticsGame.GameObjects.Effects;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.UI.Panels;
using TacticsGame.GameObjects.Zones;
using Nuclex.UserInterface.Controls;
using TacticsGame.PlayerThings;
using TacticsGame.World;

namespace TacticsGame.Scene
{
    [Serializable]
    public abstract class ZoneScene : SceneBase
    {
        [NonSerialized]
        private bool uiInitialized = false;

        [NonSerialized]
        protected const int ScrollMargin = 20;

        [NonSerialized]
        protected bool scrollingLeft = false;
        [NonSerialized]
        protected bool scrollingRight = false;
        [NonSerialized]
        protected bool scrollingUp = false;
        [NonSerialized]
        protected bool scrollingDown = false;

        [NonSerialized]
        protected Control openDialog = null;

        [NonSerialized]
        protected List<FloatingText> floatingText = new List<FloatingText>();
        [NonSerialized]
        protected List<Tile> movementTiles = new List<Tile>();
        [NonSerialized]
        protected List<AnimatedEffect> visualEffects = new List<AnimatedEffect>();

        /// <summary>
        /// The active town being seen by this zone.
        /// </summary>
        protected TownState townState = new TownState();        
        
        protected List<Obstacle> obstacles = new List<Obstacle>();
        protected List<Zone> zones = new List<Zone>();
        protected TileGrid grid = null;

        /// <summary>
        /// List of all units in this town.
        /// </summary>
        protected List<Unit> Units
        {
            get { return this.townState.Units; }
            set { this.townState.Units = value; }
        }

        /// <summary>
        /// List of buildings from the active town.
        /// </summary>
        protected List<Building> Buildings
        {
            get { return this.townState.Buildings; }
            set { this.townState.Buildings = value; }
        }

        /// <summary>
        /// Whether the camera should be panning.
        /// </summary>
        protected bool IsPanning
        {
            get { return this.cameraTargetLoc != null; }
        }

        protected bool PauseForPanning
        {
            get { return this.pauseForPanning; }
        }

        [NonSerialized]
        protected Fade fade = new Fade();

        [NonSerialized]
        private Point? cameraTargetLoc = null;

        [NonSerialized]
        private bool pauseForPanning = false;

        public ZoneScene()
        {            
            InitializeUI();            

            // TEMP
            if (grid == null)
            {
                grid = new TileGrid(50, 50, 32);
                grid.LoadTiles();
            }

            grid.TileClicked += this.HandleClickedOnATile;
        }

        protected virtual void InitializeUI()
        {
            this.commandPane = InterfaceManager.Instance.CommandPane;
            this.actionFeed = InterfaceManager.Instance.ActionFeedPane;

            InterfaceManager.Instance.MakeControlVisible(this.commandPane, true);
            InterfaceManager.Instance.MakeControlVisible(this.actionFeed, true);

            this.uiInitialized = true;
        }

        /// <summary>
        /// Draws all the UI stuff.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void DrawUI(GameTime gameTime)
        {
            InterfaceManager.Instance.Gui.Draw(gameTime);
        }

        protected virtual void HandleMouseAndKeyboardInputs()
        {            
            if (this.LMBReleased)
            {
                int cameraX = GameStateManager.Instance.CameraView.X;
                int cameraY = GameStateManager.Instance.CameraView.Y;

                int gridX = this.MouseX + cameraX;
                int gridY = this.MouseY + cameraY;
                grid.MouseReleased(gridX, gridY);
            }

            if (this.MouseWheelDown)
            {
                if (GameStateManager.Instance.HandleZoom(true))
                {
                    this.SetCameraTo(AbsoluteMouseX, AbsoluteMouseY);
                }
            }
            if (this.MouseWheelUp)
            {

                if (GameStateManager.Instance.HandleZoom(false))
                {
                    this.SetCameraTo(AbsoluteMouseX, AbsoluteMouseY);
                }
            }
        }

        protected void PanCameraTo(GameEntity entity, bool pauseForPanning = false)
        {
            this.PanCameraTo(entity.DrawPosition.Left, entity.DrawPosition.Top);
        }

        protected void PanCameraTo(int x, int y, bool pauseForPanning = false)
        {
            GameStateManager instance = GameStateManager.Instance;
            float zoom = instance.ZoomLevel;
            int locX = x - (instance.CameraView.Width / 2);
            int locY = y - (instance.CameraView.Height / 2);    
            locX = locX.GetClampedValue(0, this.grid.LevelPixelWidth.Scale(zoom) - instance.CameraView.Width - 1);
            locY = locY.GetClampedValue(0, this.grid.LevelPixelHeight.Scale(zoom) - instance.CameraView.Height - 1);
            this.cameraTargetLoc = new Point(locX, locY);
            this.pauseForPanning = pauseForPanning;
        }

        protected void SetCameraTo(int x, int y)
        {
            GameStateManager instance = GameStateManager.Instance;
            float zoom = instance.ZoomLevel;
            int locX = x - (instance.CameraView.Width / 2);
            int locY = y - (instance.CameraView.Height / 2);
            locX = locX.GetClampedValue(0, this.grid.LevelPixelWidth.Scale(zoom) - instance.CameraView.Width - 1);
            locY = locY.GetClampedValue(0, this.grid.LevelPixelHeight.Scale(zoom) - instance.CameraView.Height - 1);
            GameStateManager.Instance.RelocateCamera(locX, locY);
        }

        protected void HandleCameraPanning()
        {
            if (this.cameraTargetLoc != null)
            {
                Rectangle camera = GameStateManager.Instance.CameraView;
                int panSpeed = GameStateManager.Instance.PanSpeed;
                int targetX = this.cameraTargetLoc.Value.X;
                int targetY = this.cameraTargetLoc.Value.Y;
                if (camera.X == targetX && camera.Y == targetY)
                {
                    this.cameraTargetLoc = null;
                    this.pauseForPanning = false;
                    return;
                }

                int offsetX = targetX > camera.X ? 1 : targetX < camera.X ? -1 : 0;
                int offsetY = targetY > camera.Y ? 1 : targetY < camera.Y ? -1 : 0;

                int magnitudeX = Math.Min(panSpeed, Math.Abs(targetX - camera.X));
                int magnitudeY = Math.Min(panSpeed, Math.Abs(targetY - camera.Y));

                GameStateManager.Instance.CameraView = new Rectangle(camera.X + (offsetX * magnitudeX), camera.Y + (offsetY * magnitudeY), camera.Width, camera.Height);
            }
        }

        protected virtual void HandleScrollInputs() 
        {            
            Rectangle clientArea = GameStateManager.Instance.CameraView;
            KeyboardState keystate = Keyboard.GetState();

            this.scrollingLeft = this.MouseX < ScrollMargin || keystate.IsKeyDown(Keys.Left) || keystate.IsKeyDown(Keys.A);            
            this.scrollingRight = this.MouseX > clientArea.Width - 4 || keystate.IsKeyDown(Keys.Right) || keystate.IsKeyDown(Keys.D);
            this.scrollingUp = this.MouseY < ScrollMargin || keystate.IsKeyDown(Keys.Up) || keystate.IsKeyDown(Keys.W);            
            this.scrollingDown = this.MouseY > clientArea.Height - 4 || keystate.IsKeyDown(Keys.S) || keystate.IsKeyDown(Keys.Down);
        }

        protected virtual void HandleClickedOnATile(object sender, TileGrid.TileClickedEventArgs e)
        { }

        protected abstract void ClearUIHandlers();

        protected abstract void SetUIHandlers();

        public override void QuickLoad()
        {
            this.ClearUIHandlers();
            base.QuickLoad();
        }

        public override void QuickSave()
        {
            base.QuickSave();
        }

        protected void HandleScrolling()
        {
            Rectangle camera = GameStateManager.Instance.CameraView;
            int scrollSpeed = GameStateManager.Instance.ScrollSpeed;
            float zoom = GameStateManager.Instance.ZoomLevel;

            if (this.scrollingLeft && camera.Left > 0)
            {
                GameStateManager.Instance.OffsetCamera(-scrollSpeed, 0);
            }

            if (this.scrollingRight && camera.Right < grid.LevelPixelWidth.Scale(zoom) + this.commandPane.Width - scrollSpeed)
            {
                GameStateManager.Instance.OffsetCamera(scrollSpeed, 0);
            }

            if (this.scrollingUp && camera.Top > 0)
            {
                GameStateManager.Instance.OffsetCamera(0, -scrollSpeed);
            }

            if (this.scrollingDown && camera.Bottom < grid.LevelPixelHeight.Scale(zoom) - scrollSpeed)
            {
                GameStateManager.Instance.OffsetCamera(0, scrollSpeed);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            // TEMP: eventually, put this initialization elsewhere, when creating the scene for the first time.
            if (grid == null)
            {                
                grid = new TileGrid(50, 50, 32);
                grid.LoadTiles();
            }
            
            grid.TileClicked += this.HandleClickedOnATile;

            if (!this.uiInitialized)
            {
                this.InitializeUI();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            grid.Draw(gameTime);

            // Draw zones
            foreach (Zone zone in this.zones)
            {
                zone.Draw(gameTime);
            }

            // Draw buildings
            foreach (Building building in this.Buildings)
            {
                building.Draw(gameTime);
            }

            // Draw units
            foreach (Unit unit in this.Units)
            {
                unit.Draw(gameTime);
            }

            // Draw obstacles
            foreach (Obstacle obst in this.obstacles)
            {
                obst.Draw(gameTime);
            }

            // Draw effect animations
            foreach (AnimatedEffect effect in this.visualEffects)
            {
                effect.Draw(gameTime);
            }

            // Draw floating text
            foreach (FloatingText text in this.floatingText)
            {
                text.Draw(gameTime);
            }            

            // Draw debug crap
            if (grid.SelectedTileCoords.HasValue)
            {
                Utilities.DrawDebugText(grid.SelectedTileCoords.Value.X + ", " + grid.SelectedTileCoords.Value.Y, new Vector2(10, 0));
            }

            Utilities.DrawDebugText(GameStateManager.Instance.CameraView.X + ", " + GameStateManager.Instance.CameraView.Y, new Vector2(10, 15));

            Utilities.DrawDebugText("FPS: " + this.GetFPS(gameTime), new Vector2(10, 30));

            Utilities.DrawDebugText(this.MouseX + ", " + this.MouseY, new Vector2(10, 45));            
        }
    }
}
