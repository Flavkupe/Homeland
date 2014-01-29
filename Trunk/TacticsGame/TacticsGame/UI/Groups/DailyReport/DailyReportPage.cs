using Nuclex.UserInterface.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.World;

namespace TacticsGame.UI.Groups.DailyReport
{
    public abstract class DailyReportPage : Control
    {
        public abstract void Load(DailyActivityStats stats);
    }
}
