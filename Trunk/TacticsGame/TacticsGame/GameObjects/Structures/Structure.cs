using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Obstacles;

namespace TacticsGame.GameObjects.Structures
{
    // A static object like a wall or decoration
    [Serializable]
    public abstract class Structure : Obstacle
    {
        public Structure(string objectName) 
            : base(objectName)
        {
        }

        /// <summary>
        /// For tinting
        /// </summary>
        public bool CannotBeBuilt { get; set; }
    }
}
