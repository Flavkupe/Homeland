using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Managers;
using System.Diagnostics;

namespace TacticsGame.Edicts
{
    [Serializable]
    public class Edict : GameObject
    {
        [NonSerialized]
        protected IconInfo iconInfo = null;

        private EdictType type;        

        public Edict(EdictType type)
            : this(type.ToString())
        {
            this.type = type;
        }

        public Edict(string type)
            : base(type, ResourceType.Edict)
        {
            this.type = (EdictType)Enum.Parse(typeof(EdictType), type, true);
        }

        public EdictType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Icon to represent item
        /// </summary>
        public IconInfo Icon { get { return iconInfo; } }

        /// <summary>
        /// Loads all the graphical stuff and other hard-to-serialize crap
        /// </summary>
        public override void LoadContent()
        {
            EdictResourceInfo info = GameResourceManager.Instance.GetResourceByResourceType(this.ObjectName, ResourceType.Edict) as EdictResourceInfo;

            Debug.Assert(info != null, "Could not find resource!");

            this.iconInfo = info.TextureInfo.Icon;
            this.DisplayName = info.DisplayName;            

            Debug.Assert(this.iconInfo != null, "Edict with no texture!");
        }
    }
}
