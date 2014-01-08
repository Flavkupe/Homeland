using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using TacticsGame.GameObjects.Units;
using TacticsGame.EntityMetadata;
using TacticsGame.UI.Controls;

namespace TacticsGame.UI.Groups
{
    public class UnitStatsGroup : Control
    {
        public UnitStatsGroup()
        {
            InitializeComponents();
        }

        public void SetUnitStats(Unit unit)
        {
            UnitStats maxStats = unit.BaseStats;
            UnitStats stats = unit.CurrentStats;

            this.uxCharacterIcon.ImageTexture = unit.PictureFrame.Texture;

            this.uxName.Text = unit.DisplayName;
            this.uxClass.Text = unit.UnitClassDisplayName;
            this.uxHP.Text = "HP: " + stats.HP + " / " + maxStats.HP;            
            this.uxMorale.Text = "Morale: " + stats.Morale + " / " + maxStats.Morale;
            
            this.uxPhysicalLabel.Text = "Physical: " + maxStats.Physical;
            this.uxMentalLabel.Text = "Mental: " + maxStats.Mental;
            this.uxCunningLabel.Text = "Cunning: " + maxStats.Cunning;
            
            this.uxLoyalty.Text = "Loyalty: " + stats.Loyalty;
            this.uxAP.Text = "AP: " + maxStats.ActionPoints;
            this.uxBaseAttack.Text = "Base Attack: " + stats.BaseAttack;
            this.uxBaseAttackAP.Text = "Base Attack AP: " + stats.BaseAttackAP;
            this.uxBaseAttackRange.Text = "Base Attack Range: " + stats.BaseAttackRange;
            this.uxPreferredWeapon.Text = "Preferred Weapon: " + stats.WeaponTypePreference.ToString();

            // Draw AP
            //TextureInfo greenOrb = TextureManager.Instance.GetTextureInfo("GreenOrb", ResourceType.MiscObject);
            //TextureInfo blackOrb = TextureManager.Instance.GetTextureInfo("BlackOrb", ResourceType.MiscObject);
            //TextureInfo yellowOrb = TextureManager.Instance.GetTextureInfo("YellowOrb", ResourceType.MiscObject);
            //int x = (int)this.uxAP.Bounds.Right.Offset;
            //int y = (int)this.uxAP.Bounds.Top.Offset;
            //for (int i = 0; i < Math.Min(unit.BaseStats.ActionPoints, unit.CurrentStats.ActionPoints); ++i)
            //{
            //    IconControl orb = CreateOrbIcon(greenOrb, ref x, ref y);
            //    this.Children.Add(orb);
            //}
            //for (int i = 0; i < (unit.BaseStats.ActionPoints - unit.CurrentStats.ActionPoints); ++i)
            //{
            //    IconControl orb = CreateOrbIcon(blackOrb, ref x, ref y);
            //    this.Children.Add(orb);
            //}
            //for (int i = 0; i < (unit.CurrentStats.ActionPoints - unit.BaseStats.ActionPoints); ++i)
            //{
            //    IconControl orb = CreateOrbIcon(yellowOrb, ref x, ref y);
            //    this.Children.Add(orb);
            //}
        }

        //private IconControl CreateOrbIcon(IconInfo icon, ref int x, ref int y)
        //{
        //    IconControl orb = new IconControl();
        //    orb.Icon = icon;
        //    orb.Bounds = new UniRectangle(x, y, 8, 8);
        //    x += 8;
        //    if (x > 56)
        //    {
        //        y += 8;
        //        x = 4;
        //    }

        //    return orb;
        //}

        private void InitializeComponents()
        {
            uxCharacterIcon = new ButtonControl();
            uxCharacterIcon.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 6.0f), 96, 96);
            uxCharacterIcon.Enabled = false;

            uxName = new LabelControl();
            uxName.Bounds = new UniRectangle(new UniScalar(0.0f, 108.0f), new UniScalar(0.0f, 10.0f), 100, 20);
            uxClass = new LabelControl();
            uxClass.Bounds = new UniRectangle(new UniScalar(0.0f, 108.0f), new UniScalar(0.0f, 30.0f), 100, 20);
            uxHP = new LabelControl();
            uxHP.Bounds = new UniRectangle(new UniScalar(0.0f, 108.0f), new UniScalar(0.0f, 50.0f), 100, 20);
            uxMorale = new LabelControl();
            uxMorale.Bounds = new UniRectangle(new UniScalar(0.0f, 108.0f), new UniScalar(0.0f, 70.0f), 100, 20);
            uxLoyalty = new LabelControl();
            uxLoyalty.Bounds = new UniRectangle(new UniScalar(0.0f, 108.0f), new UniScalar(0.0f, 90.0f), 100, 20);

            uxPhysicalLabel = new LabelControl();
            uxPhysicalLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 120.0f), 100, 20);                
            uxMentalLabel = new LabelControl();
            uxMentalLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 140.0f), 100, 20);
            uxCunningLabel = new LabelControl();
            uxCunningLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 160.0f), 100, 20);
            
            uxAP = new LabelControl();
            uxAP.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 210.0f), 100, 20);
            uxBaseAttackAP = new LabelControl();
            uxBaseAttackAP.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 230.0f), 100, 20);
            uxBaseAttack = new LabelControl();
            uxBaseAttack.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 250.0f), 100, 20);
            uxBaseAttackRange = new LabelControl();
            uxBaseAttackRange.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 270.0f), 100, 20);
            uxPreferredWeapon = new LabelControl();
            uxPreferredWeapon.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.0f, 290.0f), 100, 20);

            this.Children.Add(this.uxCharacterIcon);
            this.Children.Add(this.uxName);
            this.Children.Add(this.uxClass);
            this.Children.Add(this.uxHP);
            this.Children.Add(this.uxMorale);
            this.Children.Add(this.uxPhysicalLabel);
            this.Children.Add(this.uxMentalLabel);
            this.Children.Add(this.uxCunningLabel);
            this.Children.Add(this.uxLoyalty);
            this.Children.Add(this.uxAP);
            this.Children.Add(this.uxBaseAttackAP);
            this.Children.Add(this.uxBaseAttack);
            this.Children.Add(this.uxBaseAttackRange);
            this.Children.Add(this.uxPreferredWeapon);
        }

        private ButtonControl uxCharacterIcon;

        private LabelControl uxName;
        private LabelControl uxClass;       
        private LabelControl uxHP;
        
        private LabelControl uxPhysicalLabel;
        private LabelControl uxMentalLabel;
        private LabelControl uxCunningLabel;

        private LabelControl uxLoyalty;
        private LabelControl uxMorale;

        private LabelControl uxAP;
        private LabelControl uxBaseAttackAP;
        private LabelControl uxBaseAttack;
        private LabelControl uxBaseAttackRange;
        private LabelControl uxPreferredWeapon;
    }
}
