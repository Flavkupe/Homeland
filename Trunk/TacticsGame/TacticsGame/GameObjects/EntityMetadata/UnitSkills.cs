using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TacticsGame;
using System.Diagnostics;

namespace TacticsGame.GameObjects.EntityMetadata
{
    [Serializable]
    public class UnitSkills
    {
        private Dictionary<SkillType, UnitSkill> skills = new Dictionary<SkillType, UnitSkill>();

        public UnitSkills()
        {
            SkillType[] enums = (SkillType[])Enum.GetValues(typeof(SkillType));
            foreach (SkillType value in enums)
            {
                skills.Add(value, new UnitSkill(value.ToString()));
            }
        }

        public UnitSkill GetSkill(SkillType skill)
        {
            return skills[skill];
        }

        /// <summary>
        /// Gets the skill level of the skill, as an int. 0 is novice, etc.
        /// </summary>
        public int GetSkillLevel(SkillType skill)
        {
            return (int)skills[skill].SkillLevel;
        }

        public List<UnitSkill> GetAllSkills()
        {
            return skills.Values.ToList<UnitSkill>();
        }      

        public void LevelSkill(SkillType skill, int levels)
        {
            Debug.Assert(skills.ContainsKey(skill));
            this.skills[skill].LevelupSkill(levels);
        }

        public void ImproveSkill(SkillType skill, int progress)
        {
            Debug.Assert(skills.ContainsKey(skill));
            this.skills[skill].GainProgress(progress);
        }

        public enum SkillType
        {
            Appraisal,
            Merchant,
            Concentration,
            Woodsman,
            Mining,
            Woodcrafting,
            Blacksmithing,
            Leathercrafting,
            Instructing,
            Alchemy,
            Herbalism,
        }
    }
}
