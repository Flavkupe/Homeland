using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.AI.MaintenanceMode;

namespace TacticsGame.Simulation
{
    public class UnitActivityUpdateStatus
    {
        private bool doneWithTurn = false;

        private bool shouldAnnounceActivityChange = false;

        public UnitActivityUpdateStatus()
        {
        }

        public UnitActivityUpdateStatus(UnitManagementActivity activity)
        {
            this.Activity = activity;
        }

        public bool ShouldUpdateActivityStatus
        {
            get { return shouldAnnounceActivityChange; }
            set { shouldAnnounceActivityChange = value; }
        }

        private List<string> announcements = new List<string>();

        public List<string> Announcements
        {
          get { return announcements; }
          set { announcements = value; }
        }

        public bool DoneForTurn
        {
            get { return doneWithTurn; }
            set { doneWithTurn = value; }
        }

        public bool ShouldAnnounceActivityChange
        {
            get { return shouldAnnounceActivityChange; }
            set { shouldAnnounceActivityChange = value; }
        }

        public bool ShouldAnnounceActivityResults
        {
            get { return this.Results != null; }
        }     

        public UnitManagementActivity Activity { get; set; }

        public ActivityResult Results { get; set; }

        public int? ChangeInPlayerMoney { get; set; }


    }
}
