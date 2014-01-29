using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Special;
using TacticsGame.World;

namespace TacticsGame.UI.Groups.DailyReport
{
    public class TaxesPage : DailyReportPage
    {
        public TaxesPage(UniRectangle bounds)
        {
            this.InitializeComponents(bounds);
        }

        public override void Load(DailyActivityStats stats)
        {
            this.uxArea.AddControl(new LabelControl("Daily Tax Collected") { Bounds = new UniRectangle(0, 0, 150, 20)});
            this.uxArea.AddControl(new LabelControl(stats.DailyTaxesCollected.ToString()) { Bounds = new UniRectangle(0, 0, 50, 20) });
            this.uxArea.AddControl(new LabelControl("Sales Tax Collected") { Bounds = new UniRectangle(0, 0, 150, 20) });
            this.uxArea.AddControl(new LabelControl(stats.SalesTaxesCollected.ToString()) { Bounds = new UniRectangle(0, 0, 50, 20) }, null, true);

            this.uxArea.AddControl(new LabelControl("Visitor Tax Collected") { Bounds = new UniRectangle(0, 0, 150, 20) });
            this.uxArea.AddControl(new LabelControl(stats.VisitorTaxesCollected.ToString()) { Bounds = new UniRectangle(0, 0, 50, 20) });
            this.uxArea.AddControl(new LabelControl("Work Tax Collected") { Bounds = new UniRectangle(0, 0, 150, 20) });
            this.uxArea.AddControl(new LabelControl(stats.VisitorTaxesCollected.ToString()) { Bounds = new UniRectangle(0, 0, 50, 20) }, null, true);

            this.uxArea.AddControl(new LabelControl("Items bought by units") { Bounds = new UniRectangle(0, 0, 250, 20) });
            this.uxArea.AddControl(new LabelControl("Items sold by units") { Bounds = new UniRectangle(0, 0, 130, 20) }, null, true);
            InventoryControl itemsBoughtByUnits = new InventoryControl(new UniRectangle(0, 0, 230, 70));
            itemsBoughtByUnits.SetItems(stats.ItemsBoughtByUnits);
            this.uxArea.AddControl(itemsBoughtByUnits);
            InventoryControl itemsSoldByUnits = new InventoryControl(new UniRectangle(0, 0, 230, 70));
            itemsSoldByUnits.SetItems(stats.ItemsSoldByUnits);
            this.uxArea.AddControl(itemsSoldByUnits, new Margin(20,0,0,0), true);

            this.uxArea.AddControl(new LabelControl("Items bought by shops") { Bounds = new UniRectangle(0, 0, 250, 20) });
            this.uxArea.AddControl(new LabelControl("Items sold by shops") { Bounds = new UniRectangle(0, 0, 130, 20) }, null, true);
            InventoryControl itemsBoughtByStores = new InventoryControl(new UniRectangle(0, 0, 230, 70));
            itemsBoughtByStores.SetItems(stats.ItemsBoughtByShops);
            this.uxArea.AddControl(itemsBoughtByStores);
            InventoryControl itemsSoldByStores = new InventoryControl(new UniRectangle(0, 0, 230, 70));
            itemsSoldByStores.SetItems(stats.ItemsSoldByShops);
            this.uxArea.AddControl(itemsSoldByStores, new Margin(20, 0, 0, 0), true);

            this.uxArea.AddControl(new LabelControl("Items collected by units") { Bounds = new UniRectangle(0, 0, 250, 20) });
            this.uxArea.AddControl(new LabelControl("Items crafted by shops") { Bounds = new UniRectangle(0, 0, 130, 20) }, null, true);
            InventoryControl itemsCollected = new InventoryControl(new UniRectangle(0, 0, 230, 70));
            itemsCollected.SetItems(stats.ItemsCollected);
            this.uxArea.AddControl(itemsCollected);
            InventoryControl itemsCraftedByShops = new InventoryControl(new UniRectangle(0, 0, 230, 70));
            itemsCraftedByShops.SetItems(stats.ItemsCrafted);
            this.uxArea.AddControl(itemsCraftedByShops, new Margin(20, 0, 0, 0), true);
        }

        private void InitializeComponents(UniRectangle bounds)
        {
            this.Bounds = bounds;
            this.uxArea = new FlowPanelControl(bounds.RelocateClone(0, 0));
            this.uxArea.Margin = new Margin(6, 6, 3, 3);
            this.Children.Add(this.uxArea);
        }

        FlowPanelControl uxArea = null;
    }
}
