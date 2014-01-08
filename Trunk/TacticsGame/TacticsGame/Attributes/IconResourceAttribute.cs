using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TacticsGame.Attributes
{
    public class IconResourceAttribute : Attribute
    {
        public string ResourceName { get; set; }

        public IconResourceAttribute(string textureName)
        {            
            this.ResourceName = textureName;
        }
    }
}
