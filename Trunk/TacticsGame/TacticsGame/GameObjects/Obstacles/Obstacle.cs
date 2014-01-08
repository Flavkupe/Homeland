using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.GameObjects.Obstacles
{
    [Serializable]
    public class Obstacle : GameEntity
    {        
        public Obstacle(string objectName)
            : base(objectName, ResourceType.GameObject)
        {            
        }
    }
}
