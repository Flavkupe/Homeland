using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TacticsGame
{
    /// <summary>
    /// My own version of drawable game object or whatever.
    /// These are objects that are inherently serializable and will be staying in the game in some way or another.
    /// </summary>
    [Serializable]
    public abstract class GameObject 
    {
        private string name = null;

        private ResourceType resourceType;
        
        private string objectName;

        public GameObject(string objectName, ResourceType resourceType)
        {
            this.resourceType = resourceType;
            this.objectName = objectName;
            this.name = objectName;

            this.LoadContent();
        }

        /// <summary>
        /// Resource type to know which sort of resource to look for when creating object
        /// </summary>
        protected ResourceType ResourceType { get { return resourceType; } }

        /// <summary>
        /// Object name string to identify the object
        /// </summary>
        public string ObjectName { get { return objectName; } }

        /// <summary>
        /// Name useable for displaying the object.
        /// </summary>
        public virtual string DisplayName
        {
            get { return name; }
            set { name = value; }
        }        

        /// <summary>
        /// Overrides ToString() to use the name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ObjectName;
        }

        public virtual void Draw(GameTime gameTime) {}
        public virtual void Update(GameTime gameTime) {}
        public virtual void LoadContent() {}
    }
}
