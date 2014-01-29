using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.UI.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using TacticsGame.PlayerThings;
using System.Diagnostics;
using TacticsGame.Managers;
using Nuclex.UserInterface.Controls;
using TacticsGame.UI.Groups.DailyReport;
using TacticsGame.World;

namespace TacticsGame.UI.Dialogs
{
    public class DailyReportDialog : ModalDialogControl
    {
        private DailyActivityStats stats = null;

        private DailyReportDialog(DailyActivityStats stats)
            : base()
        {
            this.stats = stats;
            this.InitializeComponents();
        }

        public static DailyReportDialog CreateDialog(DailyActivityStats stats, bool viewByDefault = true) 
        {
            DailyReportDialog newDialog = new DailyReportDialog(stats);

            if (viewByDefault)
            {
                InterfaceManager.Instance.MakeControlVisible(newDialog, true);
            }

            return newDialog;
        }

        private void InitializeComponents()
        {
            this.Bounds = new UniRectangle(10, 10, 500, 456);
            
            this.uxClose = new ButtonControl();
            this.uxClose.Text = "Done";
            this.uxClose.Bounds = new UniRectangle(new UniVector(new UniScalar(1.0f, -66.0f), new UniScalar(1.0f, -36.0f)), new UniVector(60.0f, 30.0f));
            this.uxClose.Pressed += this.HandleCloseClicked;

            this.uxPage = new TaxesPage(new UniRectangle(0, 56, 500, 400));
            this.uxPage.Load(stats);

            this.Children.Add(this.uxPage);
            this.Children.Add(this.uxClose);
        }        

        private ButtonControl uxClose;

        private DailyReportPage uxPage;
    }
}
