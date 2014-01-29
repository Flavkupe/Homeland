using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using TacticsGame.EntityMetadata;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.UI.Controls;
using Microsoft.Xna.Framework;
using TacticsGame.Managers;

namespace TacticsGame.UI.Groups
{
    public class UnitAttributeGroup : Control
    {
        public UnitAttributeGroup()
        {
        }

        public void SetUnitStats(Unit unit)
        {            
            UnitStats stats = unit.CurrentStats;

            this.Children.Clear();

            IconControl line = new IconControl();
            line.Bounds = new UniRectangle(210, 20, 2, 300);

            // HACK: pretend the dummy rectangle is a sheet image. Better than nothing, I suppose.
            line.Icon = new IconInfo();
            line.Icon.SheetImage = TextureManager.Instance.DummyRectangleTexture;

            line.Color = Color.Maroon;

            BetterLabelControl skillsLabel = new BetterLabelControl();
            skillsLabel.Text = "Skills:";
            skillsLabel.Bounds = new UniRectangle(20, 12, 110, 20);
            skillsLabel.LabelColor = Color.Maroon;

            BetterLabelControl traitsLabel = new BetterLabelControl();
            traitsLabel.Text = "Traits:";
            traitsLabel.Bounds = new UniRectangle(250, 12, 110, 20);
            traitsLabel.LabelColor = Color.Maroon;

            this.Children.Add(skillsLabel);
            this.Children.Add(traitsLabel);
            this.Children.Add(line);

            int x = 6;
            int y = 35;

            foreach (UnitSkill skill in stats.Skills.GetAllSkills())
            {
                LabelControl newLabel = new LabelControl();
                newLabel.Text = skill.Name;
                newLabel.Bounds = new UniRectangle(x, y, 110, 20);

                BetterLabelControl newLabel2 = new BetterLabelControl();
                newLabel2.Text = skill.GetSkillLevelDisplay();
                newLabel2.Bounds = new UniRectangle(newLabel.Bounds.Right.Offset + 10, y, 40, 20);
                newLabel2.TooltipText = skill.GetSkillProgressPercentString(true); 

                y += 23;

                this.Children.Add(newLabel);
                this.Children.Add(newLabel2);
            }

            y = 35;
            x = 240;

            foreach (UnitTrait trait in stats.Traits)
            {
                LabelControl newLabel = new LabelControl();
                //BetterLabelControl newLabel = new BetterLabelControl();
                newLabel.Text = trait.ToString();
                newLabel.Bounds = new UniRectangle(x, y, 110, 20); 
                //newLabel.TooltipText = trait.ToString();                

                y += 23;

                this.Children.Add(newLabel);
            }
        }    
    }    
}
