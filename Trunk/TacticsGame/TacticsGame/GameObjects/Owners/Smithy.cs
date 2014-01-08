using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;
using TacticsGame.Crafting;

namespace TacticsGame.GameObjects.Owners
{
    [Serializable]
    public class Smithy : Owner
    {
        public Smithy()
            : base("Smithy")
        {
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Appraisal, 2);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Merchant, 3);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Blacksmithing, 6);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Leathercrafting, 3);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Woodcrafting, 3);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Mining, 3);
            
            this.ItemTypePreference[ItemType.Armor] = 5;
            this.ItemTypePreference[ItemType.Weapon] = 5;

            this.ItemPreference["IronOre"] = 80;
            this.ItemPreference["IronBar"] = 60;
            this.ItemPreference["Wood"] = 20;

            this.PriceMarkupRange = 15;

            this.KnownRecipes = new List<Recipe>() { new Recipe("Sword"), new Recipe("BasicDagger"), new Recipe("Spear"), new Recipe("BasicBow"), new Recipe("IronBar") };

            this.BaseStats.ActionPoints = 6;
        }

        public override bool OnlyBuysPreferredItemTypes { get { return true; } }
    }
}
