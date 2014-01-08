using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TacticsGame.Metrics;
using System.Windows.Forms.DataVisualization.Charting;

namespace TestUtility.Tabs
{
    public partial class ChartsTab : UserControl
    {
        public ChartsTab()
        {
            InitializeComponent();
        }

        private void uxDataCombo_SelectedIndexChanged(object sender, EventArgs e)
        {            
            this.uxChart.Series.Clear();
            this.uxChart.ChartAreas.Clear();

            if (!(this.uxDataCombo.SelectedItem == null))
            {
                if (this.uxAbsoluteDataRadio.Checked)
                {
                    this.ChartAbsoluteData(MetricsLogger.Instance.Metrics[(string)this.uxDataCombo.SelectedItem]);
                }
                else if (this.uxPerUnitRadio.Checked)
                {
                    this.ChartAbsoluteData(MetricsLogger.Instance.MetricsByUnit[(string)this.uxDataCombo.SelectedItem]);
                }
                else if (this.uxStatsDataRadio.Checked)
                {
                    this.ChartStatsData();
                }
                else if (this.uxTimeDataRadio.Checked)
                {
                    this.ChartTimeData();
                }                
            }
        }

        private void ChartTimeData()
        {
            ChartArea area = new ChartArea(this.uxDataCombo.SelectedItem.ToString());
            area.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
            this.uxChart.ChartAreas.Add(area);

            Dictionary<string, Queue<Statistic>> metrics = MetricsLogger.Instance.TimeStats[(string)this.uxDataCombo.SelectedItem];
            foreach (string key in metrics.Keys)
            {
                Queue<Statistic> point = metrics[key];                
                Series series = new Series(key);
                int index = 0;
                foreach (Statistic stat in metrics[key])
                {                    
                    series.Points.AddXY(index, stat.Average);
                    series.ChartType = SeriesChartType.Line;                    
                    ++index;
                }

                this.uxChart.Series.Add(series);
            }

            this.uxChart.Show();
        }

        private void ChartStatsData()
        {
            ChartArea area = new ChartArea(this.uxDataCombo.SelectedItem.ToString());
            area.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
            this.uxChart.ChartAreas.Add(area);

            Dictionary<string, Statistic> metrics = MetricsLogger.Instance.Stats[(string)this.uxDataCombo.SelectedItem];
            foreach (string key in metrics.Keys)
            {
                Series series = new Series(key);
                series.Points.AddY(metrics[key].Average);
                series.ChartType = SeriesChartType.Column;
                this.uxChart.Series.Add(series);
            }

            this.uxChart.Show();
        }

        private void ChartAbsoluteData(Dictionary<string,double> data)
        {
            ChartArea area = new ChartArea(this.uxDataCombo.SelectedItem.ToString());
            area.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
            this.uxChart.ChartAreas.Add(area);

            Dictionary<string, double> metrics = data;
            foreach (string key in metrics.Keys)
            {
                Series series = new Series(key, (int)metrics[key]);
                series.Points.AddY(metrics[key]);
                series.ChartType = SeriesChartType.Column;
                this.uxChart.Series.Add(series);
            }

            this.uxChart.Show();
        }

        private void RadioCheckedChanged(object sender, EventArgs e)
        {
            this.RefreshDropDown();
        }

        private void RefreshDropDown()
        {
            this.uxDataCombo.Items.Clear();
            if (this.uxAbsoluteDataRadio.Checked)
            {
                MetricsLogger.Instance.Metrics.Keys.ToList<string>().ForEach(a => this.uxDataCombo.Items.Add(a));
            }
            else if (this.uxStatsDataRadio.Checked)
            {
                MetricsLogger.Instance.Stats.Keys.ToList<string>().ForEach(a => this.uxDataCombo.Items.Add(a));
            }
            else if (this.uxTimeDataRadio.Checked)
            {
                MetricsLogger.Instance.TimeStats.Keys.ToList<string>().ForEach(a => this.uxDataCombo.Items.Add(a));
            }
            else
            {
                MetricsLogger.Instance.MetricsByUnit.Keys.ToList<string>().ForEach(a => this.uxDataCombo.Items.Add(a));
            }
        }

        private void uxDataCombo_DropDown(object sender, EventArgs e)
        {
            if (this.uxDataCombo.Items.Count == 0)
            {
                this.RefreshDropDown();
            }
        }
    }
}
