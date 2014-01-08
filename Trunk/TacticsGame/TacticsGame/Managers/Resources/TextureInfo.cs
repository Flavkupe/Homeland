using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TacticsGame
{
    /// <summary>
    /// For holding info about animated frames as well as icons corresponding to the frame.
    /// </summary>
    public class TextureInfo
    {
        public string TextureName { get; private set; }

        /// <summary>
        /// The full texture map
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// An icon to represent the texture
        /// </summary>
        public IconInfo Icon { get; set; }

        /// <summary>
        /// Whether it's just a single image (false) or multiple images (true).
        /// </summary>
        public bool IsAnimated { get; set; }

        /// <summary>
        /// How many rows are on the thing.
        /// </summary>
        public int HorizontalFrames { get; set; }

        /// <summary>
        /// How many vertical columns are on the thing.
        /// </summary>
        public int VerticalFrames { get; set; }

        /// <summary>
        /// The standard set for an animation.
        /// </summary>
        public int DefaultVertical { get; set; }

        /// <summary>
        /// Rate at which sprite animates, in milliseconds.
        /// </summary>
        public int AnimationRate { get; set; }

        /// <summary>
        /// Width of each frame.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of each frame.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Scale, as float.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Which horizontal frame is the one corresponding to standing still.
        /// </summary>
        public int StaticFrame { get; set; }

        public TextureInfo(string textureName, bool isAnimated = false, int horizontal = 1, int vertical = 1, int defaultVert = 1, int staticFrame = 1, int width = 32, int height = 32, int animationRate = 500, string iconName = null, float scale = 1.0f)
        {
            this.TextureName = textureName;
            this.IsAnimated = isAnimated;
            this.HorizontalFrames = horizontal;
            this.VerticalFrames = vertical;
            this.DefaultVertical = defaultVert;
            this.StaticFrame = staticFrame;
            this.AnimationRate = animationRate;
            this.Width = width;
            this.Height = height;            
            this.Scale = scale;
        }
    }
}
