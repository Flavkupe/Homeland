using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Attributes
{
    public class TextureResourceAttribute : Attribute
    {        
        public TextureInfo TextureInformation { get; set; } 

        public TextureResourceAttribute(string textureName)
        {            
            this.TextureInformation = new TextureInfo(textureName, false);
        }

        /// <summary>
        /// Attribute to set a texture to the enum type.
        /// </summary>
        /// <param name="textureName">The name of the texture resource file.</param>
        /// <param name="isAnimated">Whether the texture is animated.</param>
        /// <param name="verticalColumns">Number of columns on spritemap.</param>
        /// <param name="horizontalRows">Number of rows on spritemap.</param>
        /// <param name="defaultVertical">Default row of the sprite map.</param>
        /// <param name="staticColumn">Column for "standing still"</param>
        /// <param name="width">Width of each sprite.</param>
        /// <param name="height">Height of each sprite.</param>
        /// <param name="animationRate">How fast, in milliseconds, to animate.</param>
        public TextureResourceAttribute(string textureName, bool isAnimated, int verticalColumns = 1, int horizontalRows = 1, int defaultVertical = 1, int staticColumn = 1, int width = 32, int height = 32, int animationRate = 500, string iconName = null)
        {            
            this.TextureInformation = new TextureInfo(textureName, isAnimated, verticalColumns, horizontalRows, defaultVertical, staticColumn, width, height, animationRate, iconName);
        }
    }
}
