namespace TestUtility.Tabs
{
    partial class EntityTab
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
            this.uxObjectList = new System.Windows.Forms.ListBox();
            this.uxTypeCombo = new System.Windows.Forms.ComboBox();
            this.uxObjectTypeLabel = new System.Windows.Forms.Label();
            this.uxObjectLabel = new System.Windows.Forms.Label();
            this.uxObjectCombo = new System.Windows.Forms.ComboBox();
            this.uxGenerateButton = new System.Windows.Forms.Button();
            this.uxDeleteSelected = new System.Windows.Forms.Button();
            this.uxSaveBox = new System.Windows.Forms.TextBox();
            this.uxGroup = new System.Windows.Forms.GroupBox();
            this.uxRestore = new System.Windows.Forms.Button();
            this.uxSave = new System.Windows.Forms.Button();
            this.uxItem = new System.Windows.Forms.TextBox();
            this.uxRandom = new System.Windows.Forms.Button();
            this.uxGive = new System.Windows.Forms.Button();
            this.uxItemBox = new System.Windows.Forms.ListBox();
            this.uxGoldLabel = new System.Windows.Forms.Label();
            this.uxGiveMoney = new System.Windows.Forms.Button();
            this.uxMoney = new System.Windows.Forms.TextBox();
            this.uxReset = new System.Windows.Forms.Button();
            this.uxEntityTabs = new System.Windows.Forms.TabControl();
            this.uxInventoryTab = new System.Windows.Forms.TabPage();
            this.uxEquipmentTab = new System.Windows.Forms.TabPage();
            this.uxHelmItem = new System.Windows.Forms.TextBox();
            this.uxBootsItem = new System.Windows.Forms.TextBox();
            this.uxChestItem = new System.Windows.Forms.TextBox();
            this.uxRightHandItem = new System.Windows.Forms.TextBox();
            this.uxLeftHandItem = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.uxGroup.SuspendLayout();
            this.uxEntityTabs.SuspendLayout();
            this.uxInventoryTab.SuspendLayout();
            this.uxEquipmentTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxObjectList
            // 
            this.uxObjectList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.uxObjectList.FormattingEnabled = true;
            this.uxObjectList.Location = new System.Drawing.Point(3, 85);
            this.uxObjectList.Name = "uxObjectList";
            this.uxObjectList.Size = new System.Drawing.Size(251, 264);
            this.uxObjectList.TabIndex = 0;
            this.uxObjectList.SelectedIndexChanged += new System.EventHandler(this.uxObjectList_SelectedIndexChanged);
            // 
            // uxTypeCombo
            // 
            this.uxTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxTypeCombo.FormattingEnabled = true;
            this.uxTypeCombo.Location = new System.Drawing.Point(53, 0);
            this.uxTypeCombo.Name = "uxTypeCombo";
            this.uxTypeCombo.Size = new System.Drawing.Size(201, 21);
            this.uxTypeCombo.TabIndex = 1;
            this.uxTypeCombo.SelectedIndexChanged += new System.EventHandler(this.uxTypeCombo_SelectedIndexChanged);
            // 
            // uxObjectTypeLabel
            // 
            this.uxObjectTypeLabel.AutoSize = true;
            this.uxObjectTypeLabel.Location = new System.Drawing.Point(3, 3);
            this.uxObjectTypeLabel.Name = "uxObjectTypeLabel";
            this.uxObjectTypeLabel.Size = new System.Drawing.Size(37, 13);
            this.uxObjectTypeLabel.TabIndex = 2;
            this.uxObjectTypeLabel.Text = " Type:";
            // 
            // uxObjectLabel
            // 
            this.uxObjectLabel.AutoSize = true;
            this.uxObjectLabel.Location = new System.Drawing.Point(3, 30);
            this.uxObjectLabel.Name = "uxObjectLabel";
            this.uxObjectLabel.Size = new System.Drawing.Size(44, 13);
            this.uxObjectLabel.TabIndex = 4;
            this.uxObjectLabel.Text = " Object:";
            // 
            // uxObjectCombo
            // 
            this.uxObjectCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxObjectCombo.FormattingEnabled = true;
            this.uxObjectCombo.Location = new System.Drawing.Point(53, 27);
            this.uxObjectCombo.Name = "uxObjectCombo";
            this.uxObjectCombo.Size = new System.Drawing.Size(201, 21);
            this.uxObjectCombo.TabIndex = 3;
            // 
            // uxGenerateButton
            // 
            this.uxGenerateButton.Location = new System.Drawing.Point(3, 54);
            this.uxGenerateButton.Name = "uxGenerateButton";
            this.uxGenerateButton.Size = new System.Drawing.Size(75, 23);
            this.uxGenerateButton.TabIndex = 5;
            this.uxGenerateButton.Text = "Generate";
            this.uxGenerateButton.UseVisualStyleBackColor = true;
            this.uxGenerateButton.Click += new System.EventHandler(this.uxGenerateButton_Click);
            // 
            // uxDeleteSelected
            // 
            this.uxDeleteSelected.Location = new System.Drawing.Point(84, 54);
            this.uxDeleteSelected.Name = "uxDeleteSelected";
            this.uxDeleteSelected.Size = new System.Drawing.Size(75, 23);
            this.uxDeleteSelected.TabIndex = 6;
            this.uxDeleteSelected.Text = "Delete";
            this.uxDeleteSelected.UseVisualStyleBackColor = true;
            this.uxDeleteSelected.Click += new System.EventHandler(this.uxDeleteSelected_Click);
            // 
            // uxSaveBox
            // 
            this.uxSaveBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxSaveBox.Location = new System.Drawing.Point(6, 19);
            this.uxSaveBox.Name = "uxSaveBox";
            this.uxSaveBox.Size = new System.Drawing.Size(310, 20);
            this.uxSaveBox.TabIndex = 7;
            this.uxSaveBox.Text = "SimData.bin";
            // 
            // uxGroup
            // 
            this.uxGroup.Controls.Add(this.uxRestore);
            this.uxGroup.Controls.Add(this.uxSave);
            this.uxGroup.Controls.Add(this.uxSaveBox);
            this.uxGroup.Location = new System.Drawing.Point(261, 4);
            this.uxGroup.Name = "uxGroup";
            this.uxGroup.Size = new System.Drawing.Size(322, 73);
            this.uxGroup.TabIndex = 8;
            this.uxGroup.TabStop = false;
            this.uxGroup.Text = "Save/Load";
            // 
            // uxRestore
            // 
            this.uxRestore.Location = new System.Drawing.Point(87, 45);
            this.uxRestore.Name = "uxRestore";
            this.uxRestore.Size = new System.Drawing.Size(75, 23);
            this.uxRestore.TabIndex = 9;
            this.uxRestore.Text = "Restore";
            this.uxRestore.UseVisualStyleBackColor = true;
            this.uxRestore.Click += new System.EventHandler(this.uxRestore_Click);
            // 
            // uxSave
            // 
            this.uxSave.Location = new System.Drawing.Point(6, 44);
            this.uxSave.Name = "uxSave";
            this.uxSave.Size = new System.Drawing.Size(75, 23);
            this.uxSave.TabIndex = 8;
            this.uxSave.Text = "Save";
            this.uxSave.UseVisualStyleBackColor = true;
            this.uxSave.Click += new System.EventHandler(this.uxSave_Click);
            // 
            // uxItem
            // 
            this.uxItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxItem.Location = new System.Drawing.Point(6, 6);
            this.uxItem.Name = "uxItem";
            this.uxItem.Size = new System.Drawing.Size(131, 20);
            this.uxItem.TabIndex = 10;
            this.uxItem.Text = "Wood";
            // 
            // uxRandom
            // 
            this.uxRandom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxRandom.Location = new System.Drawing.Point(143, 4);
            this.uxRandom.Name = "uxRandom";
            this.uxRandom.Size = new System.Drawing.Size(75, 23);
            this.uxRandom.TabIndex = 11;
            this.uxRandom.Text = "Random";
            this.uxRandom.UseVisualStyleBackColor = true;
            this.uxRandom.Click += new System.EventHandler(this.uxRandom_Click);
            // 
            // uxGive
            // 
            this.uxGive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxGive.Location = new System.Drawing.Point(224, 4);
            this.uxGive.Name = "uxGive";
            this.uxGive.Size = new System.Drawing.Size(75, 23);
            this.uxGive.TabIndex = 12;
            this.uxGive.Text = "Give";
            this.uxGive.UseVisualStyleBackColor = true;
            this.uxGive.Click += new System.EventHandler(this.uxGive_Click);
            // 
            // uxItemBox
            // 
            this.uxItemBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxItemBox.FormattingEnabled = true;
            this.uxItemBox.Location = new System.Drawing.Point(6, 71);
            this.uxItemBox.Name = "uxItemBox";
            this.uxItemBox.Size = new System.Drawing.Size(293, 121);
            this.uxItemBox.TabIndex = 16;
            // 
            // uxGoldLabel
            // 
            this.uxGoldLabel.AutoSize = true;
            this.uxGoldLabel.Location = new System.Drawing.Point(6, 55);
            this.uxGoldLabel.Name = "uxGoldLabel";
            this.uxGoldLabel.Size = new System.Drawing.Size(32, 13);
            this.uxGoldLabel.TabIndex = 15;
            this.uxGoldLabel.Text = "Gold:";
            // 
            // uxGiveMoney
            // 
            this.uxGiveMoney.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uxGiveMoney.Location = new System.Drawing.Point(143, 30);
            this.uxGiveMoney.Name = "uxGiveMoney";
            this.uxGiveMoney.Size = new System.Drawing.Size(75, 23);
            this.uxGiveMoney.TabIndex = 14;
            this.uxGiveMoney.Text = "Give";
            this.uxGiveMoney.UseVisualStyleBackColor = true;
            this.uxGiveMoney.Click += new System.EventHandler(this.GiveMoneyClick);
            // 
            // uxMoney
            // 
            this.uxMoney.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxMoney.Location = new System.Drawing.Point(6, 32);
            this.uxMoney.Name = "uxMoney";
            this.uxMoney.Size = new System.Drawing.Size(131, 20);
            this.uxMoney.TabIndex = 13;
            this.uxMoney.Text = "100";
            // 
            // uxReset
            // 
            this.uxReset.Location = new System.Drawing.Point(179, 54);
            this.uxReset.Name = "uxReset";
            this.uxReset.Size = new System.Drawing.Size(75, 23);
            this.uxReset.TabIndex = 14;
            this.uxReset.Text = "Reset";
            this.uxReset.UseVisualStyleBackColor = true;
            this.uxReset.Click += new System.EventHandler(this.uxReset_Click);
            // 
            // uxEntityTabs
            // 
            this.uxEntityTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxEntityTabs.Controls.Add(this.uxInventoryTab);
            this.uxEntityTabs.Controls.Add(this.uxEquipmentTab);
            this.uxEntityTabs.Location = new System.Drawing.Point(261, 85);
            this.uxEntityTabs.Name = "uxEntityTabs";
            this.uxEntityTabs.SelectedIndex = 0;
            this.uxEntityTabs.Size = new System.Drawing.Size(322, 264);
            this.uxEntityTabs.TabIndex = 17;
            // 
            // uxInventoryTab
            // 
            this.uxInventoryTab.Controls.Add(this.uxGiveMoney);
            this.uxInventoryTab.Controls.Add(this.uxItemBox);
            this.uxInventoryTab.Controls.Add(this.uxGive);
            this.uxInventoryTab.Controls.Add(this.uxRandom);
            this.uxInventoryTab.Controls.Add(this.uxGoldLabel);
            this.uxInventoryTab.Controls.Add(this.uxItem);
            this.uxInventoryTab.Controls.Add(this.uxMoney);
            this.uxInventoryTab.Location = new System.Drawing.Point(4, 22);
            this.uxInventoryTab.Name = "uxInventoryTab";
            this.uxInventoryTab.Padding = new System.Windows.Forms.Padding(3);
            this.uxInventoryTab.Size = new System.Drawing.Size(314, 238);
            this.uxInventoryTab.TabIndex = 0;
            this.uxInventoryTab.Text = "Inventory";
            this.uxInventoryTab.UseVisualStyleBackColor = true;
            // 
            // uxEquipmentTab
            // 
            this.uxEquipmentTab.Controls.Add(this.uxHelmItem);
            this.uxEquipmentTab.Controls.Add(this.uxBootsItem);
            this.uxEquipmentTab.Controls.Add(this.uxChestItem);
            this.uxEquipmentTab.Controls.Add(this.uxRightHandItem);
            this.uxEquipmentTab.Controls.Add(this.uxLeftHandItem);
            this.uxEquipmentTab.Controls.Add(this.label5);
            this.uxEquipmentTab.Controls.Add(this.label4);
            this.uxEquipmentTab.Controls.Add(this.label3);
            this.uxEquipmentTab.Controls.Add(this.label2);
            this.uxEquipmentTab.Controls.Add(this.label1);
            this.uxEquipmentTab.Location = new System.Drawing.Point(4, 22);
            this.uxEquipmentTab.Name = "uxEquipmentTab";
            this.uxEquipmentTab.Padding = new System.Windows.Forms.Padding(3);
            this.uxEquipmentTab.Size = new System.Drawing.Size(314, 238);
            this.uxEquipmentTab.TabIndex = 1;
            this.uxEquipmentTab.Text = "Equipment";
            this.uxEquipmentTab.UseVisualStyleBackColor = true;
            // 
            // uxHelmItem
            // 
            this.uxHelmItem.Location = new System.Drawing.Point(104, 108);
            this.uxHelmItem.Name = "uxHelmItem";
            this.uxHelmItem.ReadOnly = true;
            this.uxHelmItem.Size = new System.Drawing.Size(204, 20);
            this.uxHelmItem.TabIndex = 9;
            // 
            // uxBootsItem
            // 
            this.uxBootsItem.Location = new System.Drawing.Point(104, 82);
            this.uxBootsItem.Name = "uxBootsItem";
            this.uxBootsItem.ReadOnly = true;
            this.uxBootsItem.Size = new System.Drawing.Size(204, 20);
            this.uxBootsItem.TabIndex = 8;
            // 
            // uxChestItem
            // 
            this.uxChestItem.Location = new System.Drawing.Point(104, 56);
            this.uxChestItem.Name = "uxChestItem";
            this.uxChestItem.ReadOnly = true;
            this.uxChestItem.Size = new System.Drawing.Size(204, 20);
            this.uxChestItem.TabIndex = 7;
            // 
            // uxRightHandItem
            // 
            this.uxRightHandItem.Location = new System.Drawing.Point(104, 30);
            this.uxRightHandItem.Name = "uxRightHandItem";
            this.uxRightHandItem.ReadOnly = true;
            this.uxRightHandItem.Size = new System.Drawing.Size(204, 20);
            this.uxRightHandItem.TabIndex = 6;
            // 
            // uxLeftHandItem
            // 
            this.uxLeftHandItem.Location = new System.Drawing.Point(104, 4);
            this.uxLeftHandItem.Name = "uxLeftHandItem";
            this.uxLeftHandItem.ReadOnly = true;
            this.uxLeftHandItem.Size = new System.Drawing.Size(204, 20);
            this.uxLeftHandItem.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Boots";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Helm";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Chest";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Right Hand";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Left Hand";
            // 
            // EntityTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uxEntityTabs);
            this.Controls.Add(this.uxReset);
            this.Controls.Add(this.uxGroup);
            this.Controls.Add(this.uxDeleteSelected);
            this.Controls.Add(this.uxGenerateButton);
            this.Controls.Add(this.uxObjectLabel);
            this.Controls.Add(this.uxObjectCombo);
            this.Controls.Add(this.uxObjectTypeLabel);
            this.Controls.Add(this.uxTypeCombo);
            this.Controls.Add(this.uxObjectList);
            this.Name = "EntityTab";
            this.Size = new System.Drawing.Size(586, 365);
            this.uxGroup.ResumeLayout(false);
            this.uxGroup.PerformLayout();
            this.uxEntityTabs.ResumeLayout(false);
            this.uxInventoryTab.ResumeLayout(false);
            this.uxInventoryTab.PerformLayout();
            this.uxEquipmentTab.ResumeLayout(false);
            this.uxEquipmentTab.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox uxObjectList;
        private System.Windows.Forms.ComboBox uxTypeCombo;
        private System.Windows.Forms.Label uxObjectTypeLabel;
        private System.Windows.Forms.Label uxObjectLabel;
        private System.Windows.Forms.ComboBox uxObjectCombo;
        private System.Windows.Forms.Button uxGenerateButton;
        private System.Windows.Forms.Button uxDeleteSelected;
        private System.Windows.Forms.TextBox uxSaveBox;
        private System.Windows.Forms.GroupBox uxGroup;
        private System.Windows.Forms.Button uxRestore;
        private System.Windows.Forms.Button uxSave;
        private System.Windows.Forms.TextBox uxItem;
        private System.Windows.Forms.Button uxRandom;
        private System.Windows.Forms.Button uxGive;
        private System.Windows.Forms.Button uxGiveMoney;
        private System.Windows.Forms.TextBox uxMoney;
        private System.Windows.Forms.ListBox uxItemBox;
        private System.Windows.Forms.Label uxGoldLabel;
        private System.Windows.Forms.Button uxReset;
        private System.Windows.Forms.TabControl uxEntityTabs;
        private System.Windows.Forms.TabPage uxInventoryTab;
        private System.Windows.Forms.TabPage uxEquipmentTab;
        private System.Windows.Forms.TextBox uxLeftHandItem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox uxHelmItem;
        private System.Windows.Forms.TextBox uxBootsItem;
        private System.Windows.Forms.TextBox uxChestItem;
        private System.Windows.Forms.TextBox uxRightHandItem;
    }
}
