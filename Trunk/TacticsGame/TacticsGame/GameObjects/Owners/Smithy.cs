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
            
            this.Preferences.ItemPreference.SetPreference(ItemType.Armor, 5);
            this.Preferences.ItemPreference.SetPreference(ItemType.Weapon, 5);

            this.Preferences.ItemPreference.SetPreference("IronOre", 80);
            this.Preferences.ItemPreference.SetPreference("IronBar", 60);
            this.Preferences.ItemPreference.SetPreference("Wood", 20);

            this.Preferences.ItemPreference.PriceMarkupRange = 15;

            this.Preferences.ItemPreference.OnlyBuysPreferredItemTypes = true;

            this.KnownRecipes = new List<Recipe>() { new Recipe("Sword"), new Recipe("BasicDagger"), new Recipe("Spear"), new Recipe("BasicBow"), new Recipe("IronBar") };

            this.BaseStats.ActionPoints = 6;
        }
    }
}
