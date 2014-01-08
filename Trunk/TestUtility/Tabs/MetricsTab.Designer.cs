namespace TestUtility.Tabs
{
    partial class MetricsTab
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
            this.uxGrid = new System.Windows.Forms.DataGridView();
            this.uxMetricColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uxObjectColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uxValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uxCollect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.uxGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // uxGrid
            // 
            this.uxGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.uxGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.uxMetricColumn,
            this.uxObjectColumn,
            this.uxValueColumn});
            this.uxGrid.Location = new System.Drawing.Point(4, 33);
            this.uxGrid.Name = "uxGrid";
            this.uxGrid.RowHeadersVisible = false;
            this.uxGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.uxGrid.Size = new System.Drawing.Size(565, 387);
            this.uxGrid.TabIndex = 0;
            // 
            // uxMetricColumn
            // 
            this.uxMetricColumn.HeaderText = "Metric Type";
            this.uxMetricColumn.Name = "uxMetricColumn";
            this.uxMetricColumn.ReadOnly = true;
            // 
            // uxObjectColumn
            // 
            this.uxObjectColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.uxObjectColumn.HeaderText = "Object";
            this.uxObjectColumn.Name = "uxObjectColumn";
            this.uxObjectColumn.ReadOnly = true;
            // 
            // uxValueColumn
            // 
            this.uxValueColumn.HeaderText = "Value";
            this.uxValueColumn.Name = "uxValueColumn";
            this.uxValueColumn.ReadOnly = true;
            // 
            // uxCollect
            // 
            this.uxCollect.Location = new System.Drawing.Point(4, 4);
            this.uxCollect.Name = "uxCollect";
            this.uxCollect.Size = new System.Drawing.Size(75, 23);
            this.uxCollect.TabIndex = 1;
            this.uxCollect.Text = "Collect";
            this.uxCollect.UseVisualStyleBackColor = true;
            this.uxCollect.Click += new System.EventHandler(this.uxCollect_Click);
            // 
            // MetricsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uxCollect);
            this.Controls.Add(this.uxGrid);
            this.Name = "MetricsTab";
            this.Size = new System.Drawing.Size(572, 423);
            ((System.ComponentModel.ISupportInitialize)(this.uxGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView uxGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn uxMetricColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn uxObjectColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn uxValueColumn;
        private System.Windows.Forms.Button uxCollect;
    }
}
