namespace TestUtility.Tabs
{
    partial class SimulationTab
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
            this.uxStartSimulation = new System.Windows.Forms.Button();
            this.uxNextTurn = new System.Windows.Forms.Button();
            this.uxFeed = new System.Windows.Forms.ListBox();
            this.uxClearFeed = new System.Windows.Forms.Button();
            this.uxReset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // uxStartSimulation
            // 
            this.uxStartSimulation.Location = new System.Drawing.Point(4, 4);
            this.uxStartSimulation.Name = "uxStartSimulation";
            this.uxStartSimulation.Size = new System.Drawing.Size(75, 23);
            this.uxStartSimulation.TabIndex = 0;
            this.uxStartSimulation.Text = "Start Sim";
            this.uxStartSimulation.UseVisualStyleBackColor = true;
            this.uxStartSimulation.Click += new System.EventHandler(this.uxStartSimulation_Click);
            // 
            // uxNextTurn
            // 
            this.uxNextTurn.Location = new System.Drawing.Point(86, 4);
            this.uxNextTurn.Name = "uxNextTurn";
            this.uxNextTurn.Size = new System.Drawing.Size(75, 23);
            this.uxNextTurn.TabIndex = 1;
            this.uxNextTurn.Text = "Next Turn";
            this.uxNextTurn.UseVisualStyleBackColor = true;
            this.uxNextTurn.Click += new System.EventHandler(this.uxNextTurn_Click);
            // 
            // uxFeed
            // 
            this.uxFeed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxFeed.FormattingEnabled = true;
            this.uxFeed.Location = new System.Drawing.Point(4, 33);
            this.uxFeed.Name = "uxFeed";
            this.uxFeed.Size = new System.Drawing.Size(320, 316);
            this.uxFeed.TabIndex = 2;
            // 
            // uxClearFeed
            // 
            this.uxClearFeed.Location = new System.Drawing.Point(168, 4);
            this.uxClearFeed.Name = "uxClearFeed";
            this.uxClearFeed.Size = new System.Drawing.Size(75, 23);
            this.uxClearFeed.TabIndex = 3;
            this.uxClearFeed.Text = "Clear Feed";
            this.uxClearFeed.UseVisualStyleBackColor = true;
            this.uxClearFeed.Click += new System.EventHandler(this.uxClearFeed_Click);
            // 
            // uxReset
            // 
            this.uxReset.Location = new System.Drawing.Point(249, 4);
            this.uxReset.Name = "uxReset";
            this.uxReset.Size = new System.Drawing.Size(75, 23);
            this.uxReset.TabIndex = 4;
            this.uxReset.Text = "Reset";
            this.uxReset.UseVisualStyleBackColor = true;
            this.uxReset.Click += new System.EventHandler(this.uxResetClick);
            // 
            // SimulationTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uxReset);
            this.Controls.Add(this.uxClearFeed);
            this.Controls.Add(this.uxFeed);
            this.Controls.Add(this.uxNextTurn);
            this.Controls.Add(this.uxStartSimulation);
            this.Name = "SimulationTab";
            this.Size = new System.Drawing.Size(538, 357);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uxStartSimulation;
        private System.Windows.Forms.Button uxNextTurn;
        private System.Windows.Forms.ListBox uxFeed;
        private System.Windows.Forms.Button uxClearFeed;
        private System.Windows.Forms.Button uxReset;
    }
}
