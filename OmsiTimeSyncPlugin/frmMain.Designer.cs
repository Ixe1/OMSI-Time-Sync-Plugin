namespace OmsiTimeSyncPlugin
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tmrOMSI = new System.Windows.Forms.Timer(this.components);
            this.lblOmsiTime = new System.Windows.Forms.Label();
            this.lblSystemTime = new System.Windows.Forms.Label();
            this.lblHeaderOmsiTime = new System.Windows.Forms.Label();
            this.lblHeaderSystemTime = new System.Windows.Forms.Label();
            this.btnManualSyncOmsiTime = new System.Windows.Forms.Button();
            this.chkAutoSyncOmsiTime = new System.Windows.Forms.CheckBox();
            this.chkOnlyResyncOmsiTimeIfBehindActualTime = new System.Windows.Forms.CheckBox();
            this.lblHeaderOmsiOffsetHours = new System.Windows.Forms.Label();
            this.cmbOffsetHours = new System.Windows.Forms.ComboBox();
            this.chkAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblVersionAuthorInfo = new System.Windows.Forms.Label();
            this.lnkDonate = new System.Windows.Forms.LinkLabel();
            this.lnkGithub = new System.Windows.Forms.LinkLabel();
            this.cmbAutoSyncMode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.chkAutoDetectOffsetTime = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrOMSI
            // 
            this.tmrOMSI.Interval = 1000;
            this.tmrOMSI.Tick += new System.EventHandler(this.tmrOMSI_Tick);
            // 
            // lblOmsiTime
            // 
            this.lblOmsiTime.AutoSize = true;
            this.lblOmsiTime.Location = new System.Drawing.Point(126, 9);
            this.lblOmsiTime.Name = "lblOmsiTime";
            this.lblOmsiTime.Size = new System.Drawing.Size(8, 17);
            this.lblOmsiTime.TabIndex = 0;
            this.lblOmsiTime.Text = "-";
            this.lblOmsiTime.UseCompatibleTextRendering = true;
            // 
            // lblSystemTime
            // 
            this.lblSystemTime.AutoSize = true;
            this.lblSystemTime.Location = new System.Drawing.Point(126, 26);
            this.lblSystemTime.Name = "lblSystemTime";
            this.lblSystemTime.Size = new System.Drawing.Size(8, 17);
            this.lblSystemTime.TabIndex = 1;
            this.lblSystemTime.Text = "-";
            this.lblSystemTime.UseCompatibleTextRendering = true;
            // 
            // lblHeaderOmsiTime
            // 
            this.lblHeaderOmsiTime.AutoSize = true;
            this.lblHeaderOmsiTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderOmsiTime.Location = new System.Drawing.Point(12, 9);
            this.lblHeaderOmsiTime.Name = "lblHeaderOmsiTime";
            this.lblHeaderOmsiTime.Size = new System.Drawing.Size(67, 17);
            this.lblHeaderOmsiTime.TabIndex = 2;
            this.lblHeaderOmsiTime.Text = "OMSI Time:";
            this.lblHeaderOmsiTime.UseCompatibleTextRendering = true;
            // 
            // lblHeaderSystemTime
            // 
            this.lblHeaderSystemTime.AutoSize = true;
            this.lblHeaderSystemTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderSystemTime.Location = new System.Drawing.Point(12, 26);
            this.lblHeaderSystemTime.Name = "lblHeaderSystemTime";
            this.lblHeaderSystemTime.Size = new System.Drawing.Size(70, 17);
            this.lblHeaderSystemTime.TabIndex = 3;
            this.lblHeaderSystemTime.Text = "Actual Time:";
            this.lblHeaderSystemTime.UseCompatibleTextRendering = true;
            // 
            // btnManualSyncOmsiTime
            // 
            this.btnManualSyncOmsiTime.Enabled = false;
            this.btnManualSyncOmsiTime.Location = new System.Drawing.Point(416, 128);
            this.btnManualSyncOmsiTime.Name = "btnManualSyncOmsiTime";
            this.btnManualSyncOmsiTime.Size = new System.Drawing.Size(98, 23);
            this.btnManualSyncOmsiTime.TabIndex = 5;
            this.btnManualSyncOmsiTime.Text = "Sync OMSI Time";
            this.btnManualSyncOmsiTime.UseCompatibleTextRendering = true;
            this.btnManualSyncOmsiTime.UseVisualStyleBackColor = true;
            this.btnManualSyncOmsiTime.Click += new System.EventHandler(this.btnManualSyncOmsiTime_Click);
            // 
            // chkAutoSyncOmsiTime
            // 
            this.chkAutoSyncOmsiTime.AutoSize = true;
            this.chkAutoSyncOmsiTime.Checked = true;
            this.chkAutoSyncOmsiTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSyncOmsiTime.Location = new System.Drawing.Point(12, 81);
            this.chkAutoSyncOmsiTime.Name = "chkAutoSyncOmsiTime";
            this.chkAutoSyncOmsiTime.Size = new System.Drawing.Size(275, 18);
            this.chkAutoSyncOmsiTime.TabIndex = 6;
            this.chkAutoSyncOmsiTime.Text = "Automatically keep the time in OMSI synchronised";
            this.chkAutoSyncOmsiTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAutoSyncOmsiTime.UseCompatibleTextRendering = true;
            this.chkAutoSyncOmsiTime.UseVisualStyleBackColor = true;
            this.chkAutoSyncOmsiTime.CheckedChanged += new System.EventHandler(this.chkAutoSyncOmsiTime_CheckedChanged);
            // 
            // chkOnlyResyncOmsiTimeIfBehindActualTime
            // 
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.AutoSize = true;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Checked = true;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Location = new System.Drawing.Point(12, 46);
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Name = "chkOnlyResyncOmsiTimeIfBehindActualTime";
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Size = new System.Drawing.Size(298, 18);
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.TabIndex = 7;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Text = "Only re-sync OMSI time if it falls behind the actual time";
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.UseCompatibleTextRendering = true;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.UseVisualStyleBackColor = true;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.CheckedChanged += new System.EventHandler(this.chkOnlyResyncOmsiTimeIfBehindActualTime_CheckedChanged);
            // 
            // lblHeaderOmsiOffsetHours
            // 
            this.lblHeaderOmsiOffsetHours.AutoSize = true;
            this.lblHeaderOmsiOffsetHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderOmsiOffsetHours.Location = new System.Drawing.Point(12, 138);
            this.lblHeaderOmsiOffsetHours.Name = "lblHeaderOmsiOffsetHours";
            this.lblHeaderOmsiOffsetHours.Size = new System.Drawing.Size(153, 17);
            this.lblHeaderOmsiOffsetHours.TabIndex = 8;
            this.lblHeaderOmsiOffsetHours.Text = "Offset OMSI time by (hours):";
            this.lblHeaderOmsiOffsetHours.UseCompatibleTextRendering = true;
            // 
            // cmbOffsetHours
            // 
            this.cmbOffsetHours.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOffsetHours.FormattingEnabled = true;
            this.cmbOffsetHours.Items.AddRange(new object[] {
            "-23",
            "-22",
            "-21",
            "-20",
            "-19",
            "-18",
            "-17",
            "-16",
            "-15",
            "-14",
            "-13",
            "-12",
            "-11",
            "-10",
            "-9",
            "-8",
            "-7",
            "-6",
            "-5",
            "-4",
            "-3",
            "-2",
            "-1",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.cmbOffsetHours.Location = new System.Drawing.Point(185, 135);
            this.cmbOffsetHours.Name = "cmbOffsetHours";
            this.cmbOffsetHours.Size = new System.Drawing.Size(151, 21);
            this.cmbOffsetHours.TabIndex = 9;
            this.cmbOffsetHours.SelectedIndexChanged += new System.EventHandler(this.cmbOffsetHours_SelectedIndexChanged);
            // 
            // chkAlwaysOnTop
            // 
            this.chkAlwaysOnTop.AutoSize = true;
            this.chkAlwaysOnTop.Location = new System.Drawing.Point(441, 165);
            this.chkAlwaysOnTop.Name = "chkAlwaysOnTop";
            this.chkAlwaysOnTop.Size = new System.Drawing.Size(93, 18);
            this.chkAlwaysOnTop.TabIndex = 12;
            this.chkAlwaysOnTop.Text = "Always on top";
            this.chkAlwaysOnTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAlwaysOnTop.UseCompatibleTextRendering = true;
            this.chkAlwaysOnTop.UseVisualStyleBackColor = true;
            this.chkAlwaysOnTop.CheckedChanged += new System.EventHandler(this.chkAlwaysOnTop_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(32, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(232, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "* If running BCS then it\'s advised to keep this enabled";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // lblVersionAuthorInfo
            // 
            this.lblVersionAuthorInfo.AutoEllipsis = true;
            this.lblVersionAuthorInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblVersionAuthorInfo.ForeColor = System.Drawing.Color.Black;
            this.lblVersionAuthorInfo.Location = new System.Drawing.Point(415, 35);
            this.lblVersionAuthorInfo.Name = "lblVersionAuthorInfo";
            this.lblVersionAuthorInfo.Size = new System.Drawing.Size(100, 39);
            this.lblVersionAuthorInfo.TabIndex = 14;
            this.lblVersionAuthorInfo.Text = "Version 1.10\r\nCreated by Ixel";
            this.lblVersionAuthorInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVersionAuthorInfo.UseCompatibleTextRendering = true;
            // 
            // lnkDonate
            // 
            this.lnkDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkDonate.AutoSize = true;
            this.lnkDonate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkDonate.Location = new System.Drawing.Point(256, 190);
            this.lnkDonate.Name = "lnkDonate";
            this.lnkDonate.Size = new System.Drawing.Size(281, 17);
            this.lnkDonate.TabIndex = 15;
            this.lnkDonate.TabStop = true;
            this.lnkDonate.Text = "Like this program? Consider making a small donation";
            this.lnkDonate.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lnkDonate.UseCompatibleTextRendering = true;
            this.lnkDonate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDonate_LinkClicked);
            // 
            // lnkGithub
            // 
            this.lnkGithub.AutoSize = true;
            this.lnkGithub.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkGithub.Location = new System.Drawing.Point(12, 190);
            this.lnkGithub.Name = "lnkGithub";
            this.lnkGithub.Size = new System.Drawing.Size(44, 13);
            this.lnkGithub.TabIndex = 16;
            this.lnkGithub.TabStop = true;
            this.lnkGithub.Text = "Github";
            this.lnkGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGithub_LinkClicked);
            // 
            // cmbAutoSyncMode
            // 
            this.cmbAutoSyncMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutoSyncMode.FormattingEnabled = true;
            this.cmbAutoSyncMode.Items.AddRange(new object[] {
            "Always, every second",
            "When bus is moving",
            "When bus is not moving",
            "When bus has a timetable",
            "When bus has no timetable",
            "When game is paused"});
            this.cmbAutoSyncMode.Location = new System.Drawing.Point(126, 108);
            this.cmbAutoSyncMode.Name = "cmbAutoSyncMode";
            this.cmbAutoSyncMode.Size = new System.Drawing.Size(210, 21);
            this.cmbAutoSyncMode.TabIndex = 17;
            this.cmbAutoSyncMode.SelectedIndexChanged += new System.EventHandler(this.cmbAutoSyncMode_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Auto Sync Mode:";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // picLogo
            // 
            this.picLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picLogo.BackgroundImage")));
            this.picLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picLogo.Location = new System.Drawing.Point(394, 12);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(143, 138);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 4;
            this.picLogo.TabStop = false;
            // 
            // chkAutoDetectOffsetTime
            // 
            this.chkAutoDetectOffsetTime.AutoSize = true;
            this.chkAutoDetectOffsetTime.Location = new System.Drawing.Point(342, 138);
            this.chkAutoDetectOffsetTime.Name = "chkAutoDetectOffsetTime";
            this.chkAutoDetectOffsetTime.Size = new System.Drawing.Size(46, 18);
            this.chkAutoDetectOffsetTime.TabIndex = 22;
            this.chkAutoDetectOffsetTime.Text = "Auto";
            this.chkAutoDetectOffsetTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAutoDetectOffsetTime.UseCompatibleTextRendering = true;
            this.chkAutoDetectOffsetTime.UseVisualStyleBackColor = true;
            this.chkAutoDetectOffsetTime.CheckedChanged += new System.EventHandler(this.chkAutoDetectOffsetTime_CheckedChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 211);
            this.Controls.Add(this.chkAutoDetectOffsetTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbAutoSyncMode);
            this.Controls.Add(this.lnkGithub);
            this.Controls.Add(this.lnkDonate);
            this.Controls.Add(this.lblVersionAuthorInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkAlwaysOnTop);
            this.Controls.Add(this.cmbOffsetHours);
            this.Controls.Add(this.lblHeaderOmsiOffsetHours);
            this.Controls.Add(this.chkOnlyResyncOmsiTimeIfBehindActualTime);
            this.Controls.Add(this.chkAutoSyncOmsiTime);
            this.Controls.Add(this.btnManualSyncOmsiTime);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.lblHeaderSystemTime);
            this.Controls.Add(this.lblHeaderOmsiTime);
            this.Controls.Add(this.lblSystemTime);
            this.Controls.Add(this.lblOmsiTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OMSI Time Sync";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.LocationChanged += new System.EventHandler(this.frmMain_LocationChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmrOMSI;
        private System.Windows.Forms.Label lblOmsiTime;
        private System.Windows.Forms.Label lblSystemTime;
        private System.Windows.Forms.Label lblHeaderOmsiTime;
        private System.Windows.Forms.Label lblHeaderSystemTime;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnManualSyncOmsiTime;
        private System.Windows.Forms.CheckBox chkAutoSyncOmsiTime;
        private System.Windows.Forms.CheckBox chkOnlyResyncOmsiTimeIfBehindActualTime;
        private System.Windows.Forms.Label lblHeaderOmsiOffsetHours;
        private System.Windows.Forms.ComboBox cmbOffsetHours;
        private System.Windows.Forms.CheckBox chkAlwaysOnTop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblVersionAuthorInfo;
        private System.Windows.Forms.LinkLabel lnkDonate;
        private System.Windows.Forms.LinkLabel lnkGithub;
        private System.Windows.Forms.ComboBox cmbAutoSyncMode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkAutoDetectOffsetTime;
    }
}

