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
    public class Crafter : Owner
    {
        public Crafter()
            : base("Crafter")
        {
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Appraisal, 4);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Merchant, 3);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Blacksmithing, 2);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Leathercrafting, 5);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Alchemy, 5);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Woodsman, 2);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Concentration, 3);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Woodcrafting, 5);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Mining, 2);           

            this.Preferences.ItemPreference.SetPreference("Leather", 80);
            this.Preferences.ItemPreference.SetPreference("HerbCluster", 60);
            this.Preferences.ItemPreference.SetPreference("LeafCluster", 60);
            this.Preferences.ItemPreference.SetPreference("WhiteFlowers", 60);
            this.Preferences.ItemPreference.SetPreference("Bottle", 90);
            this.Preferences.ItemPreference.SetPreference("BulbousRoot", 50);
            this.Preferences.ItemPreference.SetPreference("GreenShroom", 50);
            this.Preferences.ItemPreference.SetPreference("Root", 60);
            this.Preferences.ItemPreference.SetPreference("WildMushroom", 60);
            this.Preferences.ItemPreference.SetPreference("RedShroom", 20);
            this.Preferences.ItemPreference.SetPreference("YellowFlowers", 50);

            this.Preferences.ItemPreference.PriceMarkupRange = 15;
            this.Preferences.ItemPreference.OnlyBuysPreferredItemTypes = true;

            this.KnownRecipes = new List<Recipe>() { new Recipe("LeatherSet"), new Recipe("Balm"), new Recipe("Balm2"), new Recipe("EnergyElixer"), new Recipe("SmallTincture"), new Recipe("HealingPotion"), new Recipe("HealingPotion2"), new Recipe("HealingPotion3") };

            this.BaseStats.ActionPoints = 6;
        }
    }
}
