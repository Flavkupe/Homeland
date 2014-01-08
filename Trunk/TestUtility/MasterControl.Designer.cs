namespace TestUtility
{
    partial class MasterControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uxTabControl = new System.Windows.Forms.TabControl();
            this.uxEntityTab = new System.Windows.Forms.TabPage();
            this.uxEntityControl = new TestUtility.Tabs.EntityTab();
            this.uxSimulationTab = new System.Windows.Forms.TabPage();
            this.uxSimulationControl = new TestUtility.Tabs.SimulationTab();
            this.uxMetricsTab = new System.Windows.Forms.TabPage();
            this.uxMetricsControl = new TestUtility.Tabs.MetricsTab();
            this.uxChartsTab = new System.Windows.Forms.TabPage();
            this.uxChartsControl = new TestUtility.Tabs.ChartsTab();
            this.uxTabControl.SuspendLayout();
            this.uxEntityTab.SuspendLayout();
            this.uxSimulationTab.SuspendLayout();
            this.uxMetricsTab.SuspendLayout();
            this.uxChartsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxTabControl
            // 
            this.uxTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxTabControl.Controls.Add(this.uxEntityTab);
            this.uxTabControl.Controls.Add(this.uxSimulationTab);
            this.uxTabControl.Controls.Add(this.uxMetricsTab);
            this.uxTabControl.Controls.Add(this.uxChartsTab);
            this.uxTabControl.Location = new System.Drawing.Point(13, 13);
            this.uxTabControl.Name = "uxTabControl";
            this.uxTabControl.SelectedIndex = 0;
            this.uxTabControl.Size = new System.Drawing.Size(600, 418);
            this.uxTabControl.TabIndex = 0;
            // 
            // uxEntityTab
            // 
            this.uxEntityTab.Controls.Add(this.uxEntityControl);
            this.uxEntityTab.Location = new System.Drawing.Point(4, 22);
            this.uxEntityTab.Name = "uxEntityTab";
            this.uxEntityTab.Padding = new System.Windows.Forms.Padding(3);
            this.uxEntityTab.Size = new System.Drawing.Size(592, 392);
            this.uxEntityTab.TabIndex = 0;
            this.uxEntityTab.Text = "Entities";
            this.uxEntityTab.UseVisualStyleBackColor = true;
            // 
            // uxEntityControl
            // 
            this.uxEntityControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxEntityControl.Location = new System.Drawing.Point(6, 6);
            this.uxEntityControl.Name = "uxEntityControl";
            this.uxEntityControl.Size = new System.Drawing.Size(580, 380);
            this.uxEntityControl.TabIndex = 0;
            // 
            // uxSimulationTab
            // 
            this.uxSimulationTab.Controls.Add(this.uxSimulationControl);
            this.uxSimulationTab.Location = new System.Drawing.Point(4, 22);
            this.uxSimulationTab.Name = "uxSimulationTab";
            this.uxSimulationTab.Padding = new System.Windows.Forms.Padding(3);
            this.uxSimulationTab.Size = new System.Drawing.Size(592, 392);
            this.uxSimulationTab.TabIndex = 1;
            this.uxSimulationTab.Text = "Simulation";
            this.uxSimulationTab.UseVisualStyleBackColor = true;
            // 
            // uxSimulationControl
            // 
            this.uxSimulationControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxSimulationControl.Location = new System.Drawing.Point(7, 7);
            this.uxSimulationControl.Name = "uxSimulationControl";
            this.uxSimulationControl.Size = new System.Drawing.Size(579, 379);
            this.uxSimulationControl.TabIndex = 0;
            // 
            // uxMetricsTab
            // 
            this.uxMetricsTab.Controls.Add(this.uxMetricsControl);
            this.uxMetricsTab.Location = new System.Drawing.Point(4, 22);
            this.uxMetricsTab.Name = "uxMetricsTab";
            this.uxMetricsTab.Padding = new System.Windows.Forms.Padding(3);
            this.uxMetricsTab.Size = new System.Drawing.Size(592, 392);
            this.uxMetricsTab.TabIndex = 2;
            this.uxMetricsTab.Text = "Metrics";
            this.uxMetricsTab.UseVisualStyleBackColor = true;
            // 
            // uxMetricsControl
            // 
            this.uxMetricsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxMetricsControl.Location = new System.Drawing.Point(7, 7);
            this.uxMetricsControl.Name = "uxMetricsControl";
            this.uxMetricsControl.Size = new System.Drawing.Size(579, 379);
            this.uxMetricsControl.TabIndex = 0;
            // 
            // uxChartsTab
            // 
            this.uxChartsTab.Controls.Add(this.uxChartsControl);
            this.uxChartsTab.Location = new System.Drawing.Point(4, 22);
            this.uxChartsTab.Name = "uxChartsTab";
            this.uxChartsTab.Padding = new System.Windows.Forms.Padding(3);
            this.uxChartsTab.Size = new System.Drawing.Size(592, 392);
            this.uxChartsTab.TabIndex = 3;
            this.uxChartsTab.Text = "Charts";
            this.uxChartsTab.UseVisualStyleBackColor = true;
            // 
            // uxChartsControl
            // 
            this.uxChartsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxChartsControl.Location = new System.Drawing.Point(7, 7);
            this.uxChartsControl.Name = "uxChartsControl";
            this.uxChartsControl.Size = new System.Drawing.Size(579, 379);
            this.uxChartsControl.TabIndex = 0;
            // 
            // MasterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 443);
            this.Controls.Add(this.uxTabControl);
            this.Name = "MasterControl";
            this.Text = "Master Control";
            this.uxTabControl.ResumeLayout(false);
            this.uxEntityTab.ResumeLayout(false);
            this.uxSimulationTab.ResumeLayout(false);
            this.uxMetricsTab.ResumeLayout(false);
            this.uxChartsTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl uxTabControl;
        private System.Windows.Forms.TabPage uxEntityTab;
        private System.Windows.Forms.TabPage uxSimulationTab;
        private Tabs.EntityTab uxEntityControl;
        private Tabs.SimulationTab uxSimulationControl;
        private System.Windows.Forms.TabPage uxMetricsTab;
        private Tabs.MetricsTab uxMetricsControl;
        private System.Windows.Forms.TabPage uxChartsTab;
        private Tabs.ChartsTab uxChartsControl;
    }
}

