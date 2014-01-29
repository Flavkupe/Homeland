using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TacticsGame.Utility;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.Managers;

namespace TacticsGame.Items
{
    [Serializable]
    public class Item : GameObject
    {
        [NonSerialized]
        private ItemStats stats = null;        

        [NonSerialized]
        protected TextureInfo textureInfo = null;

        public Item(ResourceId.Items itemType)
            : this(itemType.ToString())
        {
        }

        public Item(string itemType)
            : base(itemType, ResourceType.Item)
        {                        
        }

        /// <summary>
        /// Stats associated with the item
        /// </summary>
        public ItemStats Stats
        {
            get { return stats; }
            set { stats = value; }
        }

        /// <summary>
        /// Icon to represent item
        /// </summary>
        public IconInfo Icon { get { return textureInfo.Icon; } }

        /// <summary>
        /// Overriden ToString() to return Name of the item.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
           return this.ObjectName;
        }

        public bool HasMetadata(ItemMetadata metadata) { return this.Stats.Metadata.HasFlag(metadata); }

        /// <summary>
        /// Loads all the graphical stuff and other hard-to-serialize crap
        /// </summary>
        public override void LoadContent()
        {
            ItemResourceInfo info = GameResourceManager.Instance.GetResourceByResourceType(this.ObjectName, ResourceType.Item) as ItemResourceInfo;

            Debug.Assert(info != null, "Could not find resource!");

            this.textureInfo = info.TextureInfo;
            this.DisplayName = info.DisplayName;

            this.stats = info.Stats.Clone();

            Debug.Assert(this.textureInfo != null, "Item with no texture!");
        }

        public Item Clone()
        {
            return new Item(this.ObjectName);            
        }

        public override void Draw(GameTime gameTime)
        {            
        }

        public override void Update(GameTime gameTime)
        {            
        }
    }
}
