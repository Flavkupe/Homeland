using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.UI.Groups;
using Nuclex.UserInterface;
using TacticsGame.GameObjects.EntityMetadata;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TacticsGame.GameObjects.Buildings.Types;
using TacticsGame.Managers;
using TacticsGame.Items;

namespace TacticsGame.UI.Panels
{
    public class BuildingDetailsPanel : FramePanelControl
    {
        public BuildingDetailsPanel()
        {
            this.InitializeComponents();
        }

        public void SetSelectedBuilding(Building building)
        {
            this.uxFlowPanel.Clear();

            if (building.IsBuildingWithOwner)
            {
                this.uxOwnerControl = new TooltipButtonAndTextControl(building.Owner.GetEntityIcon(), building.Owner.DisplayName, 198);
                this.uxOwnerControl.Tag = building.Owner;
                this.uxOwnerControl.TooltipText = "Building Owner";

                this.uxFlowPanel.AddControl(this.uxOwnerControl);
            }

            if (building is GuildHouse)
            {
                int capacity = PlayerStateManager.Instance.ActiveTown.GetUnitCapacity();
                int currentUnits = PlayerStateManager.Instance.ActiveTown.DecisionMakingUnits.Count;

                IconInfo houseIcon = TextureManager.Instance.GetTextureAsIconInfo("Building_WoodHouse", ResourceType.GameObject);                
                TooltipButtonAndTextControl capacityDisplay = new TooltipButtonAndTextControl(houseIcon, string.Format("{0} / {1}", currentUnits, capacity), 198, 32);
                capacityDisplay.TooltipText = "Unit housing capacity";

                this.uxFlowPanel.AddControl(capacityDisplay);
            }
        }

        public void SetBuildingForConstruction(IBuildable building, Inventory playerInventory)
        {
            this.uxFlowPanel.Clear();

            IconInfo coinIcon = TextureManager.Instance.GetIconInfo("Coin");
            TooltipButtonAndTextControl moneyCostControl = new TooltipButtonAndTextControl(coinIcon, building.MoneyCost.ToString(), 100);
            if (playerInventory.Money < building.MoneyCost)
            {
                moneyCostControl.TextColor = Color.Red;
            }
            
            this.uxFlowPanel.AddControl(moneyCostControl);

            foreach (ObjectValuePair<string> resourceCostPair in building.ResourceCost)
            {
                string resourceName = resourceCostPair.Object;
                int resourceCost = resourceCostPair.Value;

                IconInfo resourceIcon = TextureManager.Instance.GetIconInfo(resourceName);
                TooltipButtonAndTextControl resourceControl = new TooltipButtonAndTextControl(resourceIcon, resourceCost.ToString(), 100);
                if (playerInventory.GetItemCount(resourceName) < resourceCost) 
                {
                    resourceControl.TextColor = Color.Red;
                }

                this.uxFlowPanel.AddControl(resourceControl);
            }                        
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(0.0f, 260.0f), 200.0f, 180.0f);
            this.uxFlowPanel = new FlowPanelControl(this.Bounds.RelocateClone(0, 0), new Margin(0,0,0,0));

            this.Children.Add(this.uxFlowPanel);
        }

        private FlowPanelControl uxFlowPanel;
        private TooltipButtonAndTextControl uxOwnerControl;
    }
}
