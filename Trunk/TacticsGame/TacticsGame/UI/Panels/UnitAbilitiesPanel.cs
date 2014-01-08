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
using TacticsGame.Abilities;
using TacticsGame.UI.Controls;
using TacticsGame.UI.Groups;

namespace TacticsGame.UI
{
    /// <summary>Dialog that demonstrates the capabilities of the GUI library</summary>
    public partial class UnitAbilitiesPanel : FramePanelControl
    {
        /// <summary>Initializes a new GUI demonstration dialog</summary>
        public UnitAbilitiesPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The continue button gets clicked.
        /// </summary>
        public event EventHandler<AbilityClickedEventArgs> AbilityClicked;
        
        private void HandleAbilityClicked(object sender, EventArgs e)
        {
            AbilityInfo ability = ((AbilityButton)sender).Ability;            
            if (ability.Cooldown == 0 && this.AbilityClicked != null)
            {
                this.AbilityClicked(sender, new AbilityClickedEventArgs(ability));
            }
        }

        public void ClearUnitDisplay()
        {
            this.Children.Clear();
        }

        public void SetSelectedUnitDisplay(Unit unit)
        {
            List<AbilityInfo> abilities = unit.KnownAbilities;        
            this.uxAbilityIcons.BringToFront();
            this.uxAbilityIcons.Clear();
            foreach (AbilityInfo ability in abilities)
            {               
                IconInfo icon = ability.Icon;
                AbilityButton newButton = new AbilityButton(ability, unit);
                newButton.Bounds = new UniRectangle(0, 0, icon.Dimensions, icon.Dimensions);
                newButton.Pressed += this.HandleAbilityClicked;
                newButton.SetIcon(icon);
                newButton.BringToFront();
                this.uxAbilityIcons.AddControl(newButton);
            }            
        }
    }

    public partial class UnitAbilitiesPanel
    {        
        /// <summary> 
        ///   Required method for user interface initialization -
        ///   do modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //
            // UnitAbilitiesPanel
            //
            this.Name = "Abilities";

            this.Bounds = new UniRectangle(0.0f, new UniScalar(0.2f, 150.0f), 200.0f, 150.0f);
            this.uxAbilityIcons = new FlowPanelControl(this.Bounds.RelocateClone(0,0), new Margin(3,3,3,3));
            
            this.Children.Add(this.uxAbilityIcons);
        }

        public class AbilityClickedEventArgs : EventArgs
        {
            public AbilityInfo ClickedAbility { get; set; }
            public AbilityClickedEventArgs(AbilityInfo clickedAbility)
            {
                this.ClickedAbility = clickedAbility;
            }
        }

        public class AbilityButton : TooltipButtonControl
        {
            public AbilityInfo Ability { get; set; }

            public AbilityButton(AbilityInfo ability, Unit unit)
            {
                this.Ability = ability;
                this.TooltipText = ability.DisplayName + ": " + ability.APCost;

                if (ability.Cooldown > 0)
                {
                    this.ProgressDisplayMode = ProgressMode.FullIconBack;
                    this.DisabledLook = true;
                    this.Progress = (float)ability.Cooldown / (float)ability.Stats.Cooldown;
                    this.Text = ability.Cooldown.ToString();
                }
                else if (unit != null)
                {
                    if (unit.CurrentStats.ActionPoints < ability.APCost)
                    {
                        this.DisabledLook = true;
                    }
                }
            }
        }

        public FlowPanelControl uxAbilityIcons;
    }
}
