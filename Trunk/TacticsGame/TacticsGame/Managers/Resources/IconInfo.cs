using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TacticsGame
{
    public class IconInfo
    {
        private string sheetName;
        private int coordX;
        private int coordY;
        private int dimensions = 32;
        private Texture2D sheetImage;
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Dimensions
        {
            get { return dimensions; }            
        }

        public int CoordY
        {
            get { return coordY; }
        }

        public int CoordX
        {
            get { return coordX; }
        }

        public string SheetName
        {
            get { return sheetName; }
        }        

        public Texture2D SheetImage
        {
            get { return sheetImage; }
            set { sheetImage = value; }
        }

        private Rectangle? clip = null;

        public Rectangle? Clip
        {
            get { return clip; }            
        }

        public IconInfo()
        { 
        }

        /// <summary>
        /// Create an iconInfo object
        /// </summary>
        /// <param name="sheetName">Name of sheet file.</param>
        /// <param name="coordX">The coord in units of 32. Ie the icon on the first row, 2nd column is 1.</param>
        /// <param name="coordY">The coord in units of 32. Ie the icon on the first row, 2nd column is 0.</param>
        /// <param name="dimensions">Dimension of the icon. Affects how it will be drawn usually, and how to clip it.</param>
        /// <param name="noClip">If true, then the image is used whole (a "sheet" of just one icon) and there is no clip (the clip rect is set to null)</param>
        public IconInfo(string sheetName, int coordX, int coordY, int dimensions = 32, bool noClip = false)
        {            
            this.sheetName = sheetName;
            this.dimensions = dimensions;
            if (!noClip)
            {
                this.coordX = coordX;
                this.coordY = coordY;
                this.clip = new Rectangle(coordX * dimensions, coordY * dimensions, dimensions, dimensions);
            }            
        }

        /// <summary>
        /// Create an iconInfo object for an already known image.
        /// </summary>
        /// <param name="sheetImage">The loaded image Texture2D.</param>
        /// <param name="dimensions">Dimension of the icon. Affects how it will be drawn usually, and how to clip it.</param>        
        public IconInfo(Texture2D image, int dimensions = 32)
        {
            this.sheetImage = image;
            this.dimensions = dimensions;            
            this.coordX = 0;
            this.coordY = 0;
            this.clip = null;        
        }
    }
}
