using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.GameObjects;
using Nuclex.UserInterface.Controls.Arcade;
using Nuclex.UserInterface.Controls;
using TacticsGame.GameObjects.Units;
using TacticsGame.UI.Controls;
using TacticsGame.UI.Groups;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Managers;

namespace TacticsGame.UI
{
    /// <summary>Dialog that demonstrates the capabilities of the GUI library</summary>
    public partial class UnitStatsPanel : FramePanelControl
    {
        private const int APDimensions = 16;
        private const int APWidthAccross = 10;

        /// <summary>Initializes a new GUI demonstration dialog</summary>
        public UnitStatsPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The continue button gets clicked.
        /// </summary>
        public event EventHandler ContinueButtonClicked;

        private void HandleContinueButtonClicked(object sender, EventArgs e)
        {
            if (this.ContinueButtonClicked != null)
            {
                this.ContinueButtonClicked(sender, e);
            }
        }        

        public void ClearUnitDisplay()
        {
            this.uxActionPointsLabel.Text = string.Empty;
            this.uxHPLabel.Text = string.Empty;
        }

        public void SetSelectedUnitDisplay(Unit unit)
        {
            this.uxActionPointsLabel.Text = "AP: ";
            this.uxHPLabel.Text = "HP:" + unit.CurrentStats.HP.ToString();

            this.uxAPGroup.Children.Clear();

            // Draw AP
            IconInfo greenOrb = TextureManager.Instance.GetIconInfo("GreenOrb");
            IconInfo blackOrb = TextureManager.Instance.GetIconInfo("BlackOrb");
            IconInfo yellowOrb = TextureManager.Instance.GetIconInfo("YellowOrb");
            int x = 0;
            int y = 0;
            for (int i = 0; i < Math.Min(unit.BaseStats.ActionPoints, unit.CurrentStats.ActionPoints); ++i)
            {
                IconControl orb = CreateOrbIcon(greenOrb, ref x, ref y);
                this.uxAPGroup.Children.Add(orb);
            }
            for (int i = 0; i < (unit.BaseStats.ActionPoints - unit.CurrentStats.ActionPoints); ++i)
            {
                IconControl orb = CreateOrbIcon(blackOrb, ref x, ref y);
                this.uxAPGroup.Children.Add(orb);
            }
            for (int i = 0; i < (unit.CurrentStats.ActionPoints - unit.BaseStats.ActionPoints); ++i)
            {
                IconControl orb = CreateOrbIcon(yellowOrb, ref x, ref y);
                this.uxAPGroup.Children.Add(orb);
            }

            this.uxStatusEffects.Clear();
            List<UnitStatusEffectInfo> effects = unit.StatusEffects.GetAllStatuses();
            foreach (UnitStatusEffectInfo effect in effects)
            {
                TooltipButtonControl status = new TooltipButtonControl();
                status.ImageTexture = TextureManager.Instance.GetTextureInfo("StatusEffect_" + effect.Effect.ToString(), ResourceType.MiscObject).Texture;
                status.Bounds = new UniRectangle(0, 0, 16, 16);
                status.TooltipText = effect.Effect.ToString();
                this.uxStatusEffects.AddControl(status);
            }            
        }

        private IconControl CreateOrbIcon(IconInfo icon, ref int x, ref int y)
        {
            IconControl orb = new IconControl();
            orb.Icon = icon;
            orb.Bounds = new UniRectangle(x, y, APDimensions, APDimensions);
            x += APDimensions;
            int maxX = APDimensions * APWidthAccross;   
            if (x >= maxX)
            {
                y += APDimensions;
                x = 0;
            }

            return orb;
        }
    }

    public partial class UnitStatsPanel
    {       
        /// <summary> 
        ///   Required method for user interface initialization -
        ///   do modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uxHPLabel = new LabelControl();
            this.uxHPLabel.Text = string.Empty;
            this.uxHPLabel.Bounds = new UniRectangle(6.0f, 36.0f, 110.0f, 30.0f);

            this.uxActionPointsLabel = new LabelControl();
            this.uxActionPointsLabel.Text = string.Empty;
            this.uxActionPointsLabel.Bounds = new UniRectangle(6.0f, 6.0f, 30.0f, 32.0f);

            this.uxAPGroup = new Control();
            this.uxAPGroup.Bounds = new UniRectangle(this.uxActionPointsLabel.Bounds.Right.Offset, this.uxActionPointsLabel.Bounds.Top.Offset, 170.0f, 32.0f);

            this.uxStatusEffects = new FlowPanelControl(new UniRectangle(3.0f, this.uxHPLabel.Bounds.Bottom + 3.0f, 200.0f, 64.0f));

            //
            // UnitStatsPanel
            //
            //this.Bounds = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(0.2f, 0.0f), 200.0f, 200.0f);

            this.Children.Add(this.uxHPLabel);
            this.Children.Add(this.uxActionPointsLabel);
            this.Children.Add(this.uxAPGroup);
            this.Children.Add(this.uxStatusEffects);
        }     
               
        protected LabelControl uxActionPointsLabel;

        protected LabelControl uxHPLabel;

        protected Control uxAPGroup;

        protected FlowPanelControl uxStatusEffects; 
    }
}
