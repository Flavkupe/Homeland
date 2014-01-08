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

            this.ItemPreference["Leather"] = 80;
            this.ItemPreference["HerbCluster"] = 60;
            this.ItemPreference["LeafCluster"] = 60;
            this.ItemPreference["WhiteFlowers"] = 60;
            this.ItemPreference["Bottle"] = 90;
            this.ItemPreference["BulbousRoot"] = 50;
            this.ItemPreference["GreenShroom"] = 50;
            this.ItemPreference["Root"] = 60;
            this.ItemPreference["WildMushroom"] = 60;
            this.ItemPreference["RedShroom"] = 20;
            this.ItemPreference["YellowFlowers"] = 50;

            this.PriceMarkupRange = 15;

            this.KnownRecipes = new List<Recipe>() { new Recipe("LeatherSet"), new Recipe("Balm"), new Recipe("Balm2"), new Recipe("EnergyElixer"), new Recipe("SmallTincture"), new Recipe("HealingPotion"), new Recipe("HealingPotion2"), new Recipe("HealingPotion3") };

            this.BaseStats.ActionPoints = 6;
        }

        public override bool OnlyBuysPreferredItemTypes { get { return true; } }
    }
}
