using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects;
using TacticsGame.GameObjects.Units;
using TacticsGame.Attributes;
using TacticsGame.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace TacticsGame.Abilities
{
    /// <summary>
    /// Holds info about an Ability, and unit-specific metadata such as cooldown. This is each specific Unit's held ability, not an instance of a
    ///  cast ability such as a Rock Throw.
    /// </summary>
    [Serializable]    
    public class AbilityInfo : GameObject
    {
        [NonSerialized]
        private AbilityStats stats = null;

        [NonSerialized]
        private List<AbilityVisualEffectInfo> visualEffects = new List<AbilityVisualEffectInfo>();

        private int cooldown = 0;

        [NonSerialized]
        private Unit owner;
        private string ownerId;

        [NonSerialized]
        protected TextureInfo textureInfo = null;

        public AbilityInfo(string ability, Unit owner) :
            base(ability, ResourceType.Ability)
        {
            this.LoadContent();
            this.owner = owner;
            this.ownerId = owner.ID;
        }

        /// <summary>
        /// The icon for the ability
        /// </summary>
        public IconInfo Icon
        {
            get { return this.textureInfo.Icon; }            
        }

        public Unit Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        /// The AP cost of the ability
        /// </summary>
        public virtual int APCost
        {
            get { return stats.APCost; }
        }

        public bool IsSelfAbility { get { return this.Stats.Type == AbilityType.Self || this.Stats.Type == AbilityType.SelfRadialAll || this.Stats.Type == AbilityType.SelfRadialEnemy || this.Stats.Type == AbilityType.SelfRadialFriendly; } }
        public bool IsTargetAbility { get { return !this.IsSelfAbility; } }

        /// <summary>
        /// All the info about this ability
        /// </summary>
        public AbilityStats Stats { get { return this.stats; } }        

        public List<AbilityVisualEffectInfo> VisualEffects
        {
            get { return visualEffects; }
            set { visualEffects = value; }
        }

        /// <summary>
        /// The safe name for this ability. To be used in code, but not for display.
        /// </summary>        
        public string AbilityName { get { return this.ObjectName; } }

        public int Cooldown
        {
            get { return cooldown; }
            set { cooldown = value; }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            AbilityResourceInfo info = GameResourceManager.Instance.GetResourceByResourceType(this.ObjectName, ResourceType.Ability) as AbilityResourceInfo;
            this.stats = info.Stats.Clone();
            this.textureInfo = info.TextureInfo;
            this.VisualEffects = new List<AbilityVisualEffectInfo>(info.VisualEffects); // Clone each item?
        }
    }
}
