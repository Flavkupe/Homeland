using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.Abilities;
using TacticsGame.Managers;

namespace TacticsGame.GameObjects.Units.Classes
{
    [Serializable]
    public class Ranger : HireableUnit
    {
        public Ranger()
            : base("Ranger")
        {
        }

        protected override WeaponType[] WeaponRestriction { get { return new WeaponType[] { WeaponType.Bow }; } }

        public override string UnitClassDisplayName { get { return "Ranger"; } }

        public override void LoadContent()
        {
            base.LoadContent();
            this.pictureFrame = TextureManager.Instance.GetTextureInfo("Frame_Ranger", ResourceType.MiscObject);
        }

        protected override void InitializeAbilities()
        {
            base.InitializeAbilities();
        }

        protected override void InitializeStats()
        {
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Leathercrafting, 1);
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Woodsman, 2);
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Herbalism, 2);
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Concentration, 2);

            this.AddAbility(new AbilityInfo("QuickShot", this));
            this.AddAbility(new AbilityInfo("AimedShot", this));

            this.BaseStats.ActionPoints = 9;

            this.BaseStats.GenerateRandomStats(20, 40);
            this.BaseStats.Cunning += Utilities.GetRandomNumber(20, 30);
            this.BaseStats.Physical += Utilities.GetRandomNumber(10, 20);

            this.BaseStats.WeaponTypePreference = WeaponType.Bow;
        }        
        
    }
}
