using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using TacticsGame.UI.Groups;
using Nuclex.UserInterface;
using TacticsGame.Items;
using TacticsGame.Items.SpecialStats;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TacticsGame.Managers;

namespace TacticsGame.UI.Panels
{
    public class ItemStatsPanel : FramePanelControl
    {
        public ItemStatsPanel()
        {
            this.InitializeComponent();
        }

        public void SetItemProperties(Item item)
        {
            this.uxGroup.Clear();

            if (item.Stats is WeaponStats)
            {
                WeaponStats stats = (WeaponStats)item.Stats;
                IconInfo powIcon = TextureManager.Instance.GetIconInfo("PowerIcon");
                TooltipButtonAndTextControl power = new TooltipButtonAndTextControl(powIcon, stats.Attack.ToString(), 60);

                IconInfo rngIcon = TextureManager.Instance.GetIconInfo("RangeIcon");
                TooltipButtonAndTextControl range = new TooltipButtonAndTextControl(rngIcon, stats.RangeMax.ToString(), 60);

                IconInfo apIcon = TextureManager.Instance.GetIconInfo("RunnyGuyIcon");
                TooltipButtonAndTextControl ap = new TooltipButtonAndTextControl(apIcon, stats.APCost.ToString(), 60);

                this.uxGroup.AddControl(power);
                this.uxGroup.AddControl(range);
                this.uxGroup.AddControl(ap);
            }
            else if (item.Stats is ArmorStats)
            {
                ArmorStats stats = (ArmorStats)item.Stats;
                IconInfo defIcon = TextureManager.Instance.GetIconInfo("ShieldIcon");
                TooltipButtonAndTextControl def = new TooltipButtonAndTextControl(defIcon, stats.Defense.ToString(), 60);

                IconInfo armorTypeIcon = TextureManager.Instance.GetIconInfo("TreasureChestIcon");
                TooltipButtonAndTextControl armorType = new TooltipButtonAndTextControl(armorTypeIcon, stats.ArmorType.ToString(), 100);

                this.uxGroup.AddControl(def);
                this.uxGroup.AddControl(armorType);                
            }
            
            ItemStats itemStats = item.Stats;

            IconInfo rarityIcon = TextureManager.Instance.GetIconInfo("CrownIcon");
            TooltipButtonAndTextControl rarity = new TooltipButtonAndTextControl(rarityIcon, itemStats.Rarity.ToString(), 100);

            IconInfo typeIcon = TextureManager.Instance.GetIconInfo("QuestionMarkIcon");
            TooltipButtonAndTextControl type = new TooltipButtonAndTextControl(typeIcon, itemStats.Type.ToString(), 100);

            this.uxGroup.AddControl(rarity);
            this.uxGroup.AddControl(type);  

            if (!string.IsNullOrWhiteSpace(item.Stats.Description)) 
            { 
                BetterLabelControl description = new BetterLabelControl();
                description.LabelColor = Color.SaddleBrown;
                description.Text = item.Stats.Description;
                description.Bounds = new UniRectangle(0, 0, 150, 20);
                this.uxGroup.AddControl(description, new Margin(6, 3, 0, 0));
            }          
        }

        private void InitializeComponent()
        {
            this.Bounds = new UniRectangle(0,0,200,200);
            this.uxGroup = new FlowPanelControl(this.Bounds.RelocateClone(0,0));

            this.Children.Add(this.uxGroup);
        }

        FlowPanelControl uxGroup; 
    }
}
