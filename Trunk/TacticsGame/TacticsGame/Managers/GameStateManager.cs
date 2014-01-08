using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Scene;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Map;
using Microsoft.Xna.Framework;
using TacticsGame.Managers;
using System.Diagnostics;
using TacticsGame.World;
using TacticsGame.Utility.Classes;
using TacticsGame.PlayerThings;

namespace TacticsGame
{
    public class GameStateManager : Singleton<GameStateManager>
    {
        private Rectangle camera;

        private GameWorld world = new GameWorld();

        private GameObjective gameObjective = null;

        private GameStatus gameStatus = new GameStatus();

        Stack<SceneBase> gameStack = new Stack<SceneBase>();              

        MainGame gameReference;

        private float zoomLevel = 1.0f;

        private int panSpeed = 4;

        private GameStateManager()
        {
            this.ScrollSpeed = 4;
        }

        public MainGame GameReference 
        {
            get { return this.gameReference; }
            set
            { 
                this.gameReference = value;                
            }
        }

        public void InitCameraView()
        {
            this.CameraView = new Rectangle(0, 0, this.gameReference.GraphicsDevice.Viewport.Width, this.gameReference.GraphicsDevice.Viewport.Height);
        }

        public void PushScene(SceneBase scene)
        {
            this.gameStack.Push(scene);
        }

        public SceneBase PopScene()
        {
            return this.gameStack.Pop();
        }

        public SceneBase CurrentScene
        {
            get
            {
                return this.gameStack.Peek();
            }
        }         
       
        /// <summary>
        /// Gets or sets the speed at which the game pans.
        /// </summary>
        public int PanSpeed
        {
          get { return panSpeed; }
          set { panSpeed = value; }
        }        

        public GameWorld World
        {
            get { return world; }
            set { world = value; }
        }

        public GameStatus GameStatus
        {
            get { return gameStatus; }
            set { gameStatus = value; }
        }

        /// <summary>
        /// Gets or sets the game objective.
        /// </summary>
        public GameObjective GameObjective
        {
            get { return gameObjective; }
            set { gameObjective = value; }
        }

        public ObjectiveStatus GetObjectiveStatus()
        {
            if (this.GameObjective == null)
            {
                // No objective, or none currently set.
                return ObjectiveStatus.None;
            }

            Player player = PlayerStateManager.Instance.Player;
            if (this.GameObjective.TargetDays != null) 
            {
                // There is a time limit
                if (this.GameStatus.CurrentDay >= this.GameObjective.TargetDays)
                {
                    if (this.GameObjective.TargetGold == null)
                    {
                        return ObjectiveStatus.Succeeded;
                    }
                    else
                    {
                        return player.Inventory.Money >= this.GameObjective.TargetGold ? ObjectiveStatus.Succeeded : ObjectiveStatus.Failed; 
                    }
                }
            }

            if (this.GameObjective.TargetGold != null && player.Inventory.Money >= this.GameObjective.TargetGold)
            {
                return ObjectiveStatus.Succeeded;
            }

            return ObjectiveStatus.None;
        }

        public Rectangle CameraView
        {
            get { return this.camera; }
            set { this.camera = value; }
        }

        public void OffsetCamera(int x, int y) 
        {            
            this.camera.Offset(x, y);                                      
        }

        public void RelocateCamera(int x, int y)
        {
            this.camera = this.camera.CloneAndRelocate(x, y);
        }

        public float ZoomLevel
        {
            get { return zoomLevel; }
            set { zoomLevel = value; }
        }

        public int ScrollSpeed { get; set; }        

        /// <summary>
        /// Swaps scene with current scene.
        /// </summary>
        /// <param name="data">The scene to swap.</param>
        /// <param name="replaceState">Whether to keep the old state or whether to discard it.</param>
        public void SwapStates(SceneBase scene, bool replaceState)
        {
            SceneBase oldState;

            if (replaceState)
            {
                oldState = this.gameStack.Pop();
                oldState.Dispose();
            }

            this.gameStack.Push(scene);
        }

        #region Saving and Loading        

        public void QuickLoad(string file = "Save.bin")
        {
            WorldState data = GameSerializer.Instance.Deserialize(file) as WorldState;                        

            if (data != null)
            {
                data.LoadAll();
                this.gameStack = data.SceneStack;
            }
            else
            {
                Debug.Assert(false, "Load error!");
            }
        }

        public void QuickSave(string file = "Save.bin")
        {
            WorldState state = new WorldState();
            state.SaveGlobals();
            state.SceneStack = this.gameStack;
            GameSerializer.Instance.Serialize(state, file);
        }

        #endregion

        /// <summary>
        /// Handles zoom and returns true if zoomed.
        /// </summary>
        public bool HandleZoom(bool zoomOut)
        {
            if (zoomOut)
            {
                if (this.ZoomLevel == 1.0f)
                {
                    this.ZoomLevel = 0.75f;
                    return true;
                }
                else if (this.zoomLevel == 0.75f)
                {
                    this.ZoomLevel = 0.5f;
                    return true;
                }
            }
            else
            {
                if (this.ZoomLevel == 0.5f)
                {
                    this.ZoomLevel = 0.75f;
                    return true;
                }
                else if (this.ZoomLevel == 0.75f)
                {
                    this.ZoomLevel = 1.0f;
                    return true;
                }
            }

            return false;
        }        
    }
}
