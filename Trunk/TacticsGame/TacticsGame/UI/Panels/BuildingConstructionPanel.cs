using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.UI.Groups;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework;

namespace TacticsGame.UI.Panels
{
    public class BuildingConstructionPanel : ScrollableControlList
    {        
        public BuildingConstructionPanel()
            : base(32, 32, 3, 3, 15)
        {
            this.InitializeComponents();
        }

        public event EventHandler<BuildingToBuildIconClickedEventArgs> BuildBuildingIconClicked;

        private void InitializeComponents()
        {
            // Init bounds here. They can be overriden.
            this.Bounds = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(0.0f, 60.0f), 200.0f, 200.0f);

            // Get all buildings in the namespace with the type "Building".
            Type[] types = Utilities.GetTypesInNamespace(this.GetType().Assembly, "TacticsGame.GameObjects", typeof(IBuildable));

            List<IBuildable> buildings = new List<IBuildable>();

            foreach (Type type in types)            
            {
                buildings.AddIfNotNull(Activator.CreateInstance(type) as IBuildable);
            }

            foreach (IBuildable building in buildings)
            {                
                TooltipButtonControl button = new TooltipButtonControl();
                button.SetIcon(building.Icon);
                button.TooltipText = building.DisplayName;
                button.Tag = building;
                button.Bounds = new UniRectangle(0, 0, 32, 32);
                button.Pressed += this.HandleBuildButtonPressed;
                this.AddControl(button);
            }

            this.RefreshControls();
        }        
        
        public void HandleBuildButtonPressed(object sender, EventArgs e) 
        {
            if (this.BuildBuildingIconClicked != null) 
            {
                IBuildable currentBuilding = (IBuildable)((TooltipButtonControl)sender).Tag;

                // Generate a new one of those buildings for the next time the control gets called
                ((TooltipButtonControl)sender).Tag = Activator.CreateInstance(currentBuilding.GetType());
                
                this.BuildBuildingIconClicked(this, new BuildingToBuildIconClickedEventArgs(currentBuilding));
            } 
        }
    }

    public class BuildingToBuildIconClickedEventArgs : EventArgs
    {
        public IBuildable Building { get; set; }
        public BuildingToBuildIconClickedEventArgs(IBuildable building)
        {
            this.Building = building;
        }
    }
}
