using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TacticsGame.Metrics;

namespace TestUtility.Tabs
{
    public partial class MetricsTab : UserControl
    {
        public MetricsTab()
        {
            InitializeComponent();
        }

        private void uxCollect_Click(object sender, EventArgs e)
        {
            this.uxGrid.Rows.Clear();

            foreach (string type in MetricsLogger.Instance.Metrics.Keys)
            {
                foreach (string obj in MetricsLogger.Instance.Metrics[type].Keys) 
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.uxGrid, type.ToString(), obj, MetricsLogger.Instance.Metrics[type][obj].ToString());
                    this.uxGrid.Rows.Add(row);
                }
            }

            foreach (string type in MetricsLogger.Instance.Stats.Keys)
            {
                foreach (string obj in MetricsLogger.Instance.Stats[type].Keys)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.uxGrid, type.ToString(), obj, string.Format("{0} avg", MetricsLogger.Instance.Stats[type][obj].Average.ToString()));
                    this.uxGrid.Rows.Add(row);
                }
            }

            foreach (string type in MetricsLogger.Instance.MetricsByUnit.Keys)
            {
                foreach (string obj in MetricsLogger.Instance.MetricsByUnit[type].Keys)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(this.uxGrid, type.ToString(), obj, string.Format("{0}", MetricsLogger.Instance.MetricsByUnit[type][obj].ToString()));
                    this.uxGrid.Rows.Add(row);
                }
            }
        }
    }
}
