namespace TestUtility.Tabs
{
    partial class ChartsTab
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.uxChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.uxDataCombo = new System.Windows.Forms.ComboBox();
            this.uxLabel = new System.Windows.Forms.Label();
            this.uxAbsoluteDataRadio = new System.Windows.Forms.RadioButton();
            this.uxTimeDataRadio = new System.Windows.Forms.RadioButton();
            this.uxStatsDataRadio = new System.Windows.Forms.RadioButton();
            this.uxPerUnitRadio = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.uxChart)).BeginInit();
            this.SuspendLayout();
            // 
            // uxChart
            // 
            this.uxChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.uxChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.uxChart.Legends.Add(legend2);
            this.uxChart.Location = new System.Drawing.Point(3, 129);
            this.uxChart.Name = "uxChart";
            this.uxChart.Size = new System.Drawing.Size(614, 273);
            this.uxChart.TabIndex = 0;
            this.uxChart.Text = "chart1";
            // 
            // uxDataCombo
            // 
            this.uxDataCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxDataCombo.FormattingEnabled = true;
            this.uxDataCombo.Location = new System.Drawing.Point(42, 3);
            this.uxDataCombo.Name = "uxDataCombo";
            this.uxDataCombo.Size = new System.Drawing.Size(278, 21);
            this.uxDataCombo.TabIndex = 1;
            this.uxDataCombo.DropDown += new System.EventHandler(this.uxDataCombo_DropDown);
            this.uxDataCombo.SelectedIndexChanged += new System.EventHandler(this.uxDataCombo_SelectedIndexChanged);
            // 
            // uxLabel
            // 
            this.uxLabel.AutoSize = true;
            this.uxLabel.Location = new System.Drawing.Point(3, 6);
            this.uxLabel.Name = "uxLabel";
            this.uxLabel.Size = new System.Drawing.Size(33, 13);
            this.uxLabel.TabIndex = 2;
            this.uxLabel.Text = "Data:";
            // 
            // uxAbsoluteDataRadio
            // 
            this.uxAbsoluteDataRadio.AutoSize = true;
            this.uxAbsoluteDataRadio.Checked = true;
            this.uxAbsoluteDataRadio.Location = new System.Drawing.Point(6, 30);
            this.uxAbsoluteDataRadio.Name = "uxAbsoluteDataRadio";
            this.uxAbsoluteDataRadio.Size = new System.Drawing.Size(92, 17);
            this.uxAbsoluteDataRadio.TabIndex = 3;
            this.uxAbsoluteDataRadio.TabStop = true;
            this.uxAbsoluteDataRadio.Text = "Absolute Data";
            this.uxAbsoluteDataRadio.UseVisualStyleBackColor = true;
            this.uxAbsoluteDataRadio.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
            // 
            // uxTimeDataRadio
            // 
            this.uxTimeDataRadio.AutoSize = true;
            this.uxTimeDataRadio.Location = new System.Drawing.Point(6, 53);
            this.uxTimeDataRadio.Name = "uxTimeDataRadio";
            this.uxTimeDataRadio.Size = new System.Drawing.Size(74, 17);
            this.uxTimeDataRadio.TabIndex = 4;
            this.uxTimeDataRadio.Text = "Time Data";
            this.uxTimeDataRadio.UseVisualStyleBackColor = true;
            this.uxTimeDataRadio.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
            // 
            // uxStatsDataRadio
            // 
            this.uxStatsDataRadio.AutoSize = true;
            this.uxStatsDataRadio.Location = new System.Drawing.Point(6, 76);
            this.uxStatsDataRadio.Name = "uxStatsDataRadio";
            this.uxStatsDataRadio.Size = new System.Drawing.Size(75, 17);
            this.uxStatsDataRadio.TabIndex = 5;
            this.uxStatsDataRadio.Text = "Stats Data";
            this.uxStatsDataRadio.UseVisualStyleBackColor = true;
            this.uxStatsDataRadio.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
            // 
            // uxPerUnit
            // 
            this.uxPerUnitRadio.AutoSize = true;
            this.uxPerUnitRadio.Location = new System.Drawing.Point(6, 99);
            this.uxPerUnitRadio.Name = "uxPerUnit";
            this.uxPerUnitRadio.Size = new System.Drawing.Size(89, 17);
            this.uxPerUnitRadio.TabIndex = 6;
            this.uxPerUnitRadio.Text = "Per-Unit Data";
            this.uxPerUnitRadio.UseVisualStyleBackColor = true;
            this.uxPerUnitRadio.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
            // 
            // ChartsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uxPerUnitRadio);
            this.Controls.Add(this.uxStatsDataRadio);
            this.Controls.Add(this.uxTimeDataRadio);
            this.Controls.Add(this.uxAbsoluteDataRadio);
            this.Controls.Add(this.uxLabel);
            this.Controls.Add(this.uxDataCombo);
            this.Controls.Add(this.uxChart);
            this.Name = "ChartsTab";
            this.Size = new System.Drawing.Size(620, 405);
            ((System.ComponentModel.ISupportInitialize)(this.uxChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart uxChart;
        private System.Windows.Forms.ComboBox uxDataCombo;
        private System.Windows.Forms.Label uxLabel;
        private System.Windows.Forms.RadioButton uxAbsoluteDataRadio;
        private System.Windows.Forms.RadioButton uxTimeDataRadio;
        private System.Windows.Forms.RadioButton uxStatsDataRadio;
        private System.Windows.Forms.RadioButton uxPerUnitRadio;

    }
}
