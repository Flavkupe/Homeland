using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.Abilities;

namespace TacticsGame.GameObjects.Units.Classes
{
    [Serializable]
    public class Footman : HireableUnit
    {
        public Footman()
            : base ("Footman")
        {
        }

        public override string UnitClassDisplayName { get { return "Footman"; } }

        public override void LoadContent()
        {
            base.LoadContent();
            this.pictureFrame = TextureManager.Instance.GetTextureInfo("Frame_Footsoldier", ResourceType.MiscObject);
        }

        protected override void InitializeAbilities()
        {
            this.AddAbility(new AbilityInfo("Charge", this));
            this.AddAbility(new AbilityInfo("FocusedStrike", this));
            this.AddAbility(new AbilityInfo("Battlecry", this));
            base.InitializeAbilities();
        }

        protected override void InitializeStats()
        {
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Mining, 3);
            this.BaseStats.Skills.LevelSkill(EntityMetadata.UnitSkills.SkillType.Blacksmithing, 2);

            this.BaseStats.ActionPoints = 7;

            this.BaseStats.GenerateRandomStats(20, 40);
            this.BaseStats.Cunning -= Utilities.GetRandomNumber(5, 10);
            this.BaseStats.Mental -= Utilities.GetRandomNumber(10, 15);
            this.BaseStats.Physical += Utilities.GetRandomNumber(40, 60);

            this.BaseStats.WeaponTypePreference = new WeaponType[] { WeaponType.Sword, WeaponType.Spear, WeaponType.Axe }.GetLeftSkewedRandomItem();
        }        
    }
}
