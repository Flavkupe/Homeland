using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.PlayerThings;
using TacticsGame.Scene;
using TacticsGame.Managers;
using System.Diagnostics;

namespace TacticsGame.World
{
    /// <summary>
    /// A big serializable class that contains info about the whole world... for serialization
    /// </summary>
    [Serializable]
    public class WorldState
    {
        private Player player = null;

        private Stack<SceneBase> sceneStack = null;

        /// <summary>
        /// Prepares all global values like Player for serialization
        /// </summary>
        public void SaveGlobals()
        {
            this.player = PlayerStateManager.Instance.Player;
        }

        /// <summary>
        /// Sets all the global values like Player from serialized state
        /// </summary>
        private void LoadGlobals()
        {            
            this.player.LoadContent();

            PlayerStateManager.Instance.Player = this.player;            
        }

        /// <summary>
        /// Loads content of all scenes and entities therein by calling SceneBase::LoadContent() on each scene.
        /// </summary>
        private void LoadScenes()
        {
            Debug.Assert(this.sceneStack != null && this.sceneStack.Count > 0);
            foreach (SceneBase scene in this.sceneStack)
            {
                scene.LoadContent();
            }
        }

        /// <summary>
        /// Gets or sets the scene stack, for serializing
        /// </summary>
        public Stack<SceneBase> SceneStack
        {
            get { return sceneStack; }
            set { sceneStack = value; }
        }

        /// <summary>
        /// Loads all relevant data after deserializing
        /// </summary>
        public void LoadAll()
        {
            this.LoadGlobals();
            this.LoadScenes();
        }
    }
}
