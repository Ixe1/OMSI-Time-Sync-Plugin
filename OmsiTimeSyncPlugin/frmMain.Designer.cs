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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblOmsiTime = new System.Windows.Forms.Label();
            this.lblSystemTime = new System.Windows.Forms.Label();
            this.lblHeaderOmsiTime = new System.Windows.Forms.Label();
            this.lblHeaderSystemTime = new System.Windows.Forms.Label();
            this.btnManualSyncOmsiTime = new System.Windows.Forms.Button();
            this.chkAutoSyncOmsiTime = new System.Windows.Forms.CheckBox();
            this.chkOnlyResyncOmsiTimeIfBehindActualTime = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblVersionAuthorInfo = new System.Windows.Forms.Label();
            this.lnkDonate = new System.Windows.Forms.LinkLabel();
            this.lnkGithub = new System.Windows.Forms.LinkLabel();
            this.cmbAutoSyncMode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.formBorder = new System.Windows.Forms.PictureBox();
            this.formTitleBar = new System.Windows.Forms.PictureBox();
            this.formTitle = new System.Windows.Forms.Label();
            this.formTitleBarMinimise = new System.Windows.Forms.Label();
            this.formTitleBarExpandCompact = new System.Windows.Forms.Label();
            this.formTitleBarIcon = new System.Windows.Forms.PictureBox();
            this.formTitleBarPinUnpin = new System.Windows.Forms.Label();
            this.formTitleBarCurrentOmsiSpeed = new System.Windows.Forms.Label();
            this.formTitleBarCurrentOmsiTime = new System.Windows.Forms.Label();
            this.formTitleBarIconCurrentOmsiSpeed = new System.Windows.Forms.Label();
            this.formTitleBarIconCurrentTime = new System.Windows.Forms.Label();
            this.formTitleBarIconCurrentOmsiDelay = new System.Windows.Forms.Label();
            this.formTitleBarCurrentOmsiDelay = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.formTitleBarNextBusStop = new System.Windows.Forms.Label();
            this.formTitleBarBusStopRequest = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formTitleBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formTitleBarIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // lblOmsiTime
            // 
            this.lblOmsiTime.AutoSize = true;
            this.lblOmsiTime.BackColor = System.Drawing.Color.White;
            this.lblOmsiTime.ForeColor = System.Drawing.Color.Black;
            this.lblOmsiTime.Location = new System.Drawing.Point(126, 68);
            this.lblOmsiTime.Name = "lblOmsiTime";
            this.lblOmsiTime.Size = new System.Drawing.Size(8, 17);
            this.lblOmsiTime.TabIndex = 0;
            this.lblOmsiTime.Text = "-";
            this.lblOmsiTime.UseCompatibleTextRendering = true;
            // 
            // lblSystemTime
            // 
            this.lblSystemTime.AutoSize = true;
            this.lblSystemTime.BackColor = System.Drawing.Color.White;
            this.lblSystemTime.ForeColor = System.Drawing.Color.Black;
            this.lblSystemTime.Location = new System.Drawing.Point(126, 85);
            this.lblSystemTime.Name = "lblSystemTime";
            this.lblSystemTime.Size = new System.Drawing.Size(8, 17);
            this.lblSystemTime.TabIndex = 1;
            this.lblSystemTime.Text = "-";
            this.lblSystemTime.UseCompatibleTextRendering = true;
            // 
            // lblHeaderOmsiTime
            // 
            this.lblHeaderOmsiTime.AutoSize = true;
            this.lblHeaderOmsiTime.BackColor = System.Drawing.Color.White;
            this.lblHeaderOmsiTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderOmsiTime.ForeColor = System.Drawing.Color.Black;
            this.lblHeaderOmsiTime.Location = new System.Drawing.Point(12, 68);
            this.lblHeaderOmsiTime.Name = "lblHeaderOmsiTime";
            this.lblHeaderOmsiTime.Size = new System.Drawing.Size(67, 17);
            this.lblHeaderOmsiTime.TabIndex = 2;
            this.lblHeaderOmsiTime.Text = "OMSI Time:";
            this.lblHeaderOmsiTime.UseCompatibleTextRendering = true;
            // 
            // lblHeaderSystemTime
            // 
            this.lblHeaderSystemTime.AutoSize = true;
            this.lblHeaderSystemTime.BackColor = System.Drawing.Color.White;
            this.lblHeaderSystemTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderSystemTime.ForeColor = System.Drawing.Color.Black;
            this.lblHeaderSystemTime.Location = new System.Drawing.Point(12, 85);
            this.lblHeaderSystemTime.Name = "lblHeaderSystemTime";
            this.lblHeaderSystemTime.Size = new System.Drawing.Size(70, 17);
            this.lblHeaderSystemTime.TabIndex = 3;
            this.lblHeaderSystemTime.Text = "Actual Time:";
            this.lblHeaderSystemTime.UseCompatibleTextRendering = true;
            // 
            // btnManualSyncOmsiTime
            // 
            this.btnManualSyncOmsiTime.Enabled = false;
            this.btnManualSyncOmsiTime.Location = new System.Drawing.Point(416, 187);
            this.btnManualSyncOmsiTime.Name = "btnManualSyncOmsiTime";
            this.btnManualSyncOmsiTime.Size = new System.Drawing.Size(98, 23);
            this.btnManualSyncOmsiTime.TabIndex = 5;
            this.btnManualSyncOmsiTime.Text = "Sync OMSI Time";
            this.btnManualSyncOmsiTime.UseCompatibleTextRendering = true;
            this.btnManualSyncOmsiTime.UseVisualStyleBackColor = false;
            this.btnManualSyncOmsiTime.Click += new System.EventHandler(this.btnManualSyncOmsiTime_Click);
            // 
            // chkAutoSyncOmsiTime
            // 
            this.chkAutoSyncOmsiTime.AutoSize = true;
            this.chkAutoSyncOmsiTime.BackColor = System.Drawing.Color.White;
            this.chkAutoSyncOmsiTime.Checked = true;
            this.chkAutoSyncOmsiTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSyncOmsiTime.ForeColor = System.Drawing.Color.Black;
            this.chkAutoSyncOmsiTime.Location = new System.Drawing.Point(12, 140);
            this.chkAutoSyncOmsiTime.Name = "chkAutoSyncOmsiTime";
            this.chkAutoSyncOmsiTime.Size = new System.Drawing.Size(275, 18);
            this.chkAutoSyncOmsiTime.TabIndex = 6;
            this.chkAutoSyncOmsiTime.Text = "Automatically keep the time in OMSI synchronised";
            this.chkAutoSyncOmsiTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAutoSyncOmsiTime.UseCompatibleTextRendering = true;
            this.chkAutoSyncOmsiTime.UseVisualStyleBackColor = false;
            this.chkAutoSyncOmsiTime.CheckedChanged += new System.EventHandler(this.chkAutoSyncOmsiTime_CheckedChanged);
            // 
            // chkOnlyResyncOmsiTimeIfBehindActualTime
            // 
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.AutoSize = true;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.BackColor = System.Drawing.Color.White;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Checked = true;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.ForeColor = System.Drawing.Color.Black;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Location = new System.Drawing.Point(12, 105);
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Name = "chkOnlyResyncOmsiTimeIfBehindActualTime";
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Size = new System.Drawing.Size(298, 18);
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.TabIndex = 7;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.Text = "Only re-sync OMSI time if it falls behind the actual time";
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.UseCompatibleTextRendering = true;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.UseVisualStyleBackColor = false;
            this.chkOnlyResyncOmsiTimeIfBehindActualTime.CheckedChanged += new System.EventHandler(this.chkOnlyResyncOmsiTimeIfBehindActualTime_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(32, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(232, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "* If running BCS then it\'s advised to keep this enabled";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // lblVersionAuthorInfo
            // 
            this.lblVersionAuthorInfo.AutoEllipsis = true;
            this.lblVersionAuthorInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblVersionAuthorInfo.ForeColor = System.Drawing.Color.Black;
            this.lblVersionAuthorInfo.Location = new System.Drawing.Point(415, 94);
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
            this.lnkDonate.BackColor = System.Drawing.Color.White;
            this.lnkDonate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkDonate.Location = new System.Drawing.Point(321, 243);
            this.lnkDonate.Name = "lnkDonate";
            this.lnkDonate.Size = new System.Drawing.Size(217, 17);
            this.lnkDonate.TabIndex = 15;
            this.lnkDonate.TabStop = true;
            this.lnkDonate.Text = "Please consider making a small donation";
            this.lnkDonate.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.lnkDonate.UseCompatibleTextRendering = true;
            this.lnkDonate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDonate_LinkClicked);
            // 
            // lnkGithub
            // 
            this.lnkGithub.AutoSize = true;
            this.lnkGithub.BackColor = System.Drawing.Color.White;
            this.lnkGithub.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkGithub.Location = new System.Drawing.Point(12, 243);
            this.lnkGithub.Name = "lnkGithub";
            this.lnkGithub.Size = new System.Drawing.Size(44, 13);
            this.lnkGithub.TabIndex = 16;
            this.lnkGithub.TabStop = true;
            this.lnkGithub.Text = "Github";
            this.lnkGithub.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lnkGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGithub_LinkClicked);
            // 
            // cmbAutoSyncMode
            // 
            this.cmbAutoSyncMode.BackColor = System.Drawing.Color.White;
            this.cmbAutoSyncMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutoSyncMode.FormattingEnabled = true;
            this.cmbAutoSyncMode.Items.AddRange(new object[] {
            "Always, every second",
            "When bus is moving",
            "When bus is not moving",
            "When bus has a timetable",
            "When bus has no timetable",
            "When game is paused"});
            this.cmbAutoSyncMode.Location = new System.Drawing.Point(126, 167);
            this.cmbAutoSyncMode.Name = "cmbAutoSyncMode";
            this.cmbAutoSyncMode.Size = new System.Drawing.Size(210, 21);
            this.cmbAutoSyncMode.TabIndex = 17;
            this.cmbAutoSyncMode.SelectedIndexChanged += new System.EventHandler(this.cmbAutoSyncMode_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Auto Sync Mode:";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.White;
            this.picLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picLogo.BackgroundImage")));
            this.picLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picLogo.Location = new System.Drawing.Point(394, 71);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(143, 138);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 4;
            this.picLogo.TabStop = false;
            // 
            // formBorder
            // 
            this.formBorder.BackColor = System.Drawing.Color.White;
            this.formBorder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.formBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formBorder.Location = new System.Drawing.Point(0, 0);
            this.formBorder.Name = "formBorder";
            this.formBorder.Size = new System.Drawing.Size(550, 265);
            this.formBorder.TabIndex = 23;
            this.formBorder.TabStop = false;
            // 
            // formTitleBar
            // 
            this.formTitleBar.BackColor = System.Drawing.Color.Black;
            this.formTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.formTitleBar.Location = new System.Drawing.Point(0, 0);
            this.formTitleBar.Name = "formTitleBar";
            this.formTitleBar.Size = new System.Drawing.Size(550, 57);
            this.formTitleBar.TabIndex = 24;
            this.formTitleBar.TabStop = false;
            this.formTitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitle
            // 
            this.formTitle.AutoSize = true;
            this.formTitle.BackColor = System.Drawing.Color.Black;
            this.formTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formTitle.ForeColor = System.Drawing.Color.White;
            this.formTitle.Location = new System.Drawing.Point(42, 8);
            this.formTitle.Name = "formTitle";
            this.formTitle.Size = new System.Drawing.Size(92, 17);
            this.formTitle.TabIndex = 25;
            this.formTitle.Text = "OMSI Time Sync";
            this.formTitle.UseCompatibleTextRendering = true;
            this.formTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarMinimise
            // 
            this.formTitleBarMinimise.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.formTitleBarMinimise.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.formTitleBarMinimise.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.formTitleBarMinimise.ForeColor = System.Drawing.Color.White;
            this.formTitleBarMinimise.Location = new System.Drawing.Point(525, 19);
            this.formTitleBarMinimise.Name = "formTitleBarMinimise";
            this.formTitleBarMinimise.Size = new System.Drawing.Size(17, 20);
            this.formTitleBarMinimise.TabIndex = 26;
            this.formTitleBarMinimise.Text = "0";
            this.formTitleBarMinimise.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.formTitleBarMinimise.UseCompatibleTextRendering = true;
            this.formTitleBarMinimise.MouseClick += new System.Windows.Forms.MouseEventHandler(this.formTitleBarMinimise_MouseClick);
            this.formTitleBarMinimise.MouseEnter += new System.EventHandler(this.formTitleBarMinimise_MouseEnter);
            this.formTitleBarMinimise.MouseLeave += new System.EventHandler(this.formTitleBarMinimise_MouseLeave);
            // 
            // formTitleBarExpandCompact
            // 
            this.formTitleBarExpandCompact.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.formTitleBarExpandCompact.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.formTitleBarExpandCompact.Font = new System.Drawing.Font("Webdings", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.formTitleBarExpandCompact.ForeColor = System.Drawing.Color.White;
            this.formTitleBarExpandCompact.Location = new System.Drawing.Point(502, 19);
            this.formTitleBarExpandCompact.Name = "formTitleBarExpandCompact";
            this.formTitleBarExpandCompact.Size = new System.Drawing.Size(17, 20);
            this.formTitleBarExpandCompact.TabIndex = 27;
            this.formTitleBarExpandCompact.Text = "5";
            this.formTitleBarExpandCompact.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.formTitleBarExpandCompact.UseCompatibleTextRendering = true;
            this.formTitleBarExpandCompact.MouseClick += new System.Windows.Forms.MouseEventHandler(this.formTitleBarExpandCompact_MouseClick);
            this.formTitleBarExpandCompact.MouseEnter += new System.EventHandler(this.formTitleBarExpandCompact_MouseEnter);
            this.formTitleBarExpandCompact.MouseLeave += new System.EventHandler(this.formTitleBarExpandCompact_MouseLeave);
            // 
            // formTitleBarIcon
            // 
            this.formTitleBarIcon.BackColor = System.Drawing.Color.Black;
            this.formTitleBarIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("formTitleBarIcon.BackgroundImage")));
            this.formTitleBarIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.formTitleBarIcon.Location = new System.Drawing.Point(6, 0);
            this.formTitleBarIcon.Name = "formTitleBarIcon";
            this.formTitleBarIcon.Size = new System.Drawing.Size(32, 32);
            this.formTitleBarIcon.TabIndex = 28;
            this.formTitleBarIcon.TabStop = false;
            this.formTitleBarIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarPinUnpin
            // 
            this.formTitleBarPinUnpin.BackColor = System.Drawing.Color.DarkGreen;
            this.formTitleBarPinUnpin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.formTitleBarPinUnpin.Font = new System.Drawing.Font("Webdings", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.formTitleBarPinUnpin.ForeColor = System.Drawing.Color.White;
            this.formTitleBarPinUnpin.Location = new System.Drawing.Point(479, 19);
            this.formTitleBarPinUnpin.Name = "formTitleBarPinUnpin";
            this.formTitleBarPinUnpin.Size = new System.Drawing.Size(17, 20);
            this.formTitleBarPinUnpin.TabIndex = 29;
            this.formTitleBarPinUnpin.Text = "N";
            this.formTitleBarPinUnpin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.formTitleBarPinUnpin.UseCompatibleTextRendering = true;
            this.formTitleBarPinUnpin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.formTitleBarPinUnpin_MouseClick);
            this.formTitleBarPinUnpin.MouseEnter += new System.EventHandler(this.formTitleBarPinUnpin_MouseEnter);
            this.formTitleBarPinUnpin.MouseLeave += new System.EventHandler(this.formTitleBarPinUnpin_MouseLeave);
            // 
            // formTitleBarCurrentOmsiSpeed
            // 
            this.formTitleBarCurrentOmsiSpeed.BackColor = System.Drawing.Color.Black;
            this.formTitleBarCurrentOmsiSpeed.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formTitleBarCurrentOmsiSpeed.ForeColor = System.Drawing.Color.White;
            this.formTitleBarCurrentOmsiSpeed.Location = new System.Drawing.Point(185, 8);
            this.formTitleBarCurrentOmsiSpeed.Name = "formTitleBarCurrentOmsiSpeed";
            this.formTitleBarCurrentOmsiSpeed.Size = new System.Drawing.Size(67, 17);
            this.formTitleBarCurrentOmsiSpeed.TabIndex = 30;
            this.formTitleBarCurrentOmsiSpeed.Text = "0   MPH";
            this.formTitleBarCurrentOmsiSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.formTitleBarCurrentOmsiSpeed.UseCompatibleTextRendering = true;
            this.formTitleBarCurrentOmsiSpeed.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.formTitleBarCurrentOmsiSpeed_MouseDoubleClick);
            this.formTitleBarCurrentOmsiSpeed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarCurrentOmsiSpeed.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarCurrentOmsiSpeed.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarCurrentOmsiTime
            // 
            this.formTitleBarCurrentOmsiTime.BackColor = System.Drawing.Color.Black;
            this.formTitleBarCurrentOmsiTime.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formTitleBarCurrentOmsiTime.ForeColor = System.Drawing.Color.White;
            this.formTitleBarCurrentOmsiTime.Location = new System.Drawing.Point(290, 8);
            this.formTitleBarCurrentOmsiTime.Name = "formTitleBarCurrentOmsiTime";
            this.formTitleBarCurrentOmsiTime.Size = new System.Drawing.Size(75, 17);
            this.formTitleBarCurrentOmsiTime.TabIndex = 31;
            this.formTitleBarCurrentOmsiTime.Text = "-";
            this.formTitleBarCurrentOmsiTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.formTitleBarCurrentOmsiTime.UseCompatibleTextRendering = true;
            this.formTitleBarCurrentOmsiTime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarCurrentOmsiTime.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarCurrentOmsiTime.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarIconCurrentOmsiSpeed
            // 
            this.formTitleBarIconCurrentOmsiSpeed.BackColor = System.Drawing.Color.Black;
            this.formTitleBarIconCurrentOmsiSpeed.Font = new System.Drawing.Font("Webdings", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.formTitleBarIconCurrentOmsiSpeed.ForeColor = System.Drawing.Color.Orange;
            this.formTitleBarIconCurrentOmsiSpeed.Location = new System.Drawing.Point(161, 8);
            this.formTitleBarIconCurrentOmsiSpeed.Name = "formTitleBarIconCurrentOmsiSpeed";
            this.formTitleBarIconCurrentOmsiSpeed.Size = new System.Drawing.Size(18, 18);
            this.formTitleBarIconCurrentOmsiSpeed.TabIndex = 32;
            this.formTitleBarIconCurrentOmsiSpeed.Text = "v";
            this.formTitleBarIconCurrentOmsiSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.formTitleBarIconCurrentOmsiSpeed.UseCompatibleTextRendering = true;
            this.formTitleBarIconCurrentOmsiSpeed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarIconCurrentOmsiSpeed.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarIconCurrentOmsiSpeed.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarIconCurrentTime
            // 
            this.formTitleBarIconCurrentTime.BackColor = System.Drawing.Color.Black;
            this.formTitleBarIconCurrentTime.Font = new System.Drawing.Font("Wingdings", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.formTitleBarIconCurrentTime.ForeColor = System.Drawing.Color.Orange;
            this.formTitleBarIconCurrentTime.Location = new System.Drawing.Point(266, 9);
            this.formTitleBarIconCurrentTime.Name = "formTitleBarIconCurrentTime";
            this.formTitleBarIconCurrentTime.Size = new System.Drawing.Size(18, 18);
            this.formTitleBarIconCurrentTime.TabIndex = 33;
            this.formTitleBarIconCurrentTime.Text = "º";
            this.formTitleBarIconCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.formTitleBarIconCurrentTime.UseCompatibleTextRendering = true;
            this.formTitleBarIconCurrentTime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarIconCurrentTime.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarIconCurrentTime.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarIconCurrentOmsiDelay
            // 
            this.formTitleBarIconCurrentOmsiDelay.BackColor = System.Drawing.Color.Black;
            this.formTitleBarIconCurrentOmsiDelay.Font = new System.Drawing.Font("Wingdings", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.formTitleBarIconCurrentOmsiDelay.ForeColor = System.Drawing.Color.Orange;
            this.formTitleBarIconCurrentOmsiDelay.Location = new System.Drawing.Point(379, 9);
            this.formTitleBarIconCurrentOmsiDelay.Name = "formTitleBarIconCurrentOmsiDelay";
            this.formTitleBarIconCurrentOmsiDelay.Size = new System.Drawing.Size(18, 18);
            this.formTitleBarIconCurrentOmsiDelay.TabIndex = 35;
            this.formTitleBarIconCurrentOmsiDelay.Text = "6";
            this.formTitleBarIconCurrentOmsiDelay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.formTitleBarIconCurrentOmsiDelay.UseCompatibleTextRendering = true;
            this.formTitleBarIconCurrentOmsiDelay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarIconCurrentOmsiDelay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarIconCurrentOmsiDelay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarCurrentOmsiDelay
            // 
            this.formTitleBarCurrentOmsiDelay.BackColor = System.Drawing.Color.Black;
            this.formTitleBarCurrentOmsiDelay.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formTitleBarCurrentOmsiDelay.ForeColor = System.Drawing.Color.White;
            this.formTitleBarCurrentOmsiDelay.Location = new System.Drawing.Point(400, 8);
            this.formTitleBarCurrentOmsiDelay.Name = "formTitleBarCurrentOmsiDelay";
            this.formTitleBarCurrentOmsiDelay.Size = new System.Drawing.Size(64, 17);
            this.formTitleBarCurrentOmsiDelay.TabIndex = 36;
            this.formTitleBarCurrentOmsiDelay.Text = "-";
            this.formTitleBarCurrentOmsiDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.formTitleBarCurrentOmsiDelay.UseCompatibleTextRendering = true;
            this.formTitleBarCurrentOmsiDelay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarCurrentOmsiDelay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarCurrentOmsiDelay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(437, 226);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 37;
            this.label1.Text = "Like this program?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label1.UseCompatibleTextRendering = true;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Black;
            this.label6.Font = new System.Drawing.Font("Wingdings", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label6.ForeColor = System.Drawing.Color.Orange;
            this.label6.Location = new System.Drawing.Point(7, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 18);
            this.label6.TabIndex = 40;
            this.label6.Text = "é";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label6.UseCompatibleTextRendering = true;
            this.label6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.label6.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.label6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarNextBusStop
            // 
            this.formTitleBarNextBusStop.BackColor = System.Drawing.Color.Black;
            this.formTitleBarNextBusStop.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formTitleBarNextBusStop.ForeColor = System.Drawing.Color.White;
            this.formTitleBarNextBusStop.Location = new System.Drawing.Point(32, 34);
            this.formTitleBarNextBusStop.Name = "formTitleBarNextBusStop";
            this.formTitleBarNextBusStop.Size = new System.Drawing.Size(333, 17);
            this.formTitleBarNextBusStop.TabIndex = 41;
            this.formTitleBarNextBusStop.Text = "-";
            this.formTitleBarNextBusStop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.formTitleBarNextBusStop.UseCompatibleTextRendering = true;
            this.formTitleBarNextBusStop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarNextBusStop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarNextBusStop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // formTitleBarBusStopRequest
            // 
            this.formTitleBarBusStopRequest.BackColor = System.Drawing.Color.Black;
            this.formTitleBarBusStopRequest.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formTitleBarBusStopRequest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.formTitleBarBusStopRequest.Location = new System.Drawing.Point(400, 34);
            this.formTitleBarBusStopRequest.Name = "formTitleBarBusStopRequest";
            this.formTitleBarBusStopRequest.Size = new System.Drawing.Size(64, 17);
            this.formTitleBarBusStopRequest.TabIndex = 43;
            this.formTitleBarBusStopRequest.Text = "- STOP -";
            this.formTitleBarBusStopRequest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.formTitleBarBusStopRequest.UseCompatibleTextRendering = true;
            this.formTitleBarBusStopRequest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseDown);
            this.formTitleBarBusStopRequest.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseMove);
            this.formTitleBarBusStopRequest.MouseUp += new System.Windows.Forms.MouseEventHandler(this.formTitleBar_MouseUp);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(550, 265);
            this.Controls.Add(this.formTitleBarBusStopRequest);
            this.Controls.Add(this.formTitleBarNextBusStop);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.formTitleBarIcon);
            this.Controls.Add(this.lnkDonate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.formTitleBarCurrentOmsiDelay);
            this.Controls.Add(this.formTitleBarIconCurrentOmsiDelay);
            this.Controls.Add(this.formTitleBarIconCurrentTime);
            this.Controls.Add(this.formTitleBarIconCurrentOmsiSpeed);
            this.Controls.Add(this.formTitleBarCurrentOmsiTime);
            this.Controls.Add(this.formTitleBarCurrentOmsiSpeed);
            this.Controls.Add(this.formTitleBarPinUnpin);
            this.Controls.Add(this.formTitleBarExpandCompact);
            this.Controls.Add(this.formTitleBarMinimise);
            this.Controls.Add(this.formTitle);
            this.Controls.Add(this.formTitleBar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbAutoSyncMode);
            this.Controls.Add(this.lnkGithub);
            this.Controls.Add(this.lblVersionAuthorInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkOnlyResyncOmsiTimeIfBehindActualTime);
            this.Controls.Add(this.chkAutoSyncOmsiTime);
            this.Controls.Add(this.btnManualSyncOmsiTime);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.lblHeaderSystemTime);
            this.Controls.Add(this.lblHeaderOmsiTime);
            this.Controls.Add(this.lblSystemTime);
            this.Controls.Add(this.lblOmsiTime);
            this.Controls.Add(this.formBorder);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OMSI Time Sync";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.LocationChanged += new System.EventHandler(this.frmMain_LocationChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formTitleBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formTitleBarIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblOmsiTime;
        private System.Windows.Forms.Label lblSystemTime;
        private System.Windows.Forms.Label lblHeaderOmsiTime;
        private System.Windows.Forms.Label lblHeaderSystemTime;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnManualSyncOmsiTime;
        private System.Windows.Forms.CheckBox chkAutoSyncOmsiTime;
        private System.Windows.Forms.CheckBox chkOnlyResyncOmsiTimeIfBehindActualTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblVersionAuthorInfo;
        private System.Windows.Forms.LinkLabel lnkDonate;
        private System.Windows.Forms.LinkLabel lnkGithub;
        private System.Windows.Forms.ComboBox cmbAutoSyncMode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox formBorder;
        private System.Windows.Forms.PictureBox formTitleBar;
        private System.Windows.Forms.Label formTitle;
        private System.Windows.Forms.Label formTitleBarMinimise;
        private System.Windows.Forms.Label formTitleBarExpandCompact;
        private System.Windows.Forms.PictureBox formTitleBarIcon;
        private System.Windows.Forms.Label formTitleBarPinUnpin;
        private System.Windows.Forms.Label formTitleBarCurrentOmsiSpeed;
        private System.Windows.Forms.Label formTitleBarCurrentOmsiTime;
        private System.Windows.Forms.Label formTitleBarIconCurrentOmsiSpeed;
        private System.Windows.Forms.Label formTitleBarIconCurrentTime;
        private System.Windows.Forms.Label formTitleBarIconCurrentOmsiDelay;
        private System.Windows.Forms.Label formTitleBarCurrentOmsiDelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label formTitleBarNextBusStop;
        private System.Windows.Forms.Label formTitleBarBusStopRequest;
    }
}

