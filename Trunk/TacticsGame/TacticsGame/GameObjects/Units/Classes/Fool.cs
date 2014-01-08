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
    public class Fool : HireableUnit
    {
        public Fool()
            : base("Fool")
        {
        }

        protected override WeaponType[] WeaponRestriction { get { return new WeaponType[] { WeaponType.Dagger }; } }

        public override string UnitClassDisplayName { get { return "Fool"; } }

        public override void LoadContent()
        {
            base.LoadContent();            
            this.pictureFrame = TextureManager.Instance.GetTextureInfo("Frame_Fool", ResourceType.MiscObject);
        }

        protected override void InitializeAbilities()
        {
            base.InitializeAbilities();
        }

        protected override void InitializeStats()
        {
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Appraisal, 2);
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Merchant, 2);
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Woodcrafting, 1);

            this.AddAbility(new AbilityInfo("Mock", this));
            this.AddAbility(new AbilityInfo("Muddle", this));

            this.BaseStats.ActionPoints = 8;            

            this.BaseStats.GenerateRandomStats(20, 40);
            this.BaseStats.Cunning += Utilities.GetRandomNumber(40, 50);
            this.BaseStats.Physical -= Utilities.GetRandomNumber(10, 20);

            this.BaseStats.WeaponTypePreference = WeaponType.Dagger;

           
            
                
            

            
        }        
    }
}
