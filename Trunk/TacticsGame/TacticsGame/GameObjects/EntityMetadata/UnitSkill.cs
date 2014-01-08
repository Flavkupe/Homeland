using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.GameObjects.EntityMetadata
{
    [Serializable]
    public class UnitSkill
    {
        private SkillLevel skillLevel;

        /// <summary>
        /// The enum representing the unit's skill in a certain area.
        /// </summary>
        public SkillLevel SkillLevel
        {
            get { return skillLevel; }
        }

        private int progress = 0;
        private string name;
        
        /// <summary>
        /// Name of the skill.
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Display name of the skill.</param>
        public UnitSkill(string name)
        {            
            this.name = name;
        }

        public int RequiredForLevelUp
        {
            get
            {
                int nextSkillLevel = (((int)skillLevel) + 1);
                int requiredForLevelup = nextSkillLevel * 100;
                return requiredForLevelup;
            }
        }

        public void GainProgress(int progress) 
        {
            if (this.skillLevel == SkillLevel.Legendary)
            {
                return;
            }

            this.progress += progress;



            if (progress > RequiredForLevelUp)
            {
                this.progress = 0;
                this.skillLevel += 1;
            }
        }

        public UnitSkill Clone()
        {
            return (UnitSkill)this.MemberwiseClone();
        }

        public string GetSkillLevelDisplay()
        {
            return this.SkillLevel.ToString();
        }

        public string GetSkillProgressPercentString(bool includePercentSign)
        {
            string format = includePercentSign ? "{0}%" : "{0}";
            return string.Format(format, (int)(((float)this.progress / (float)this.RequiredForLevelUp) * 100.0f));            
        }

        public void LevelupSkill(int levels)
        {
            this.skillLevel += levels;
            this.skillLevel = (SkillLevel)Math.Min((int)this.skillLevel, (int)SkillLevel.Legendary);
        }
    }

    public enum SkillLevel
    {
        Novice = 0,
        Beginner = 1,
        Apprentice = 2,
        Layman = 3,            
        Intermediate = 4,
        Advanced = 5,
        Skilled = 6,        
        Expert = 7,
        Master = 8,
        Grandmaster = 9,
        Legendary = 10,
    }
}
