using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace OmsiTimeSyncPlugin
{
    public partial class frmMain : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        [DllImport("user32")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint itemId, uint uEnable);

        public DateTime omsiTime = DateTime.MinValue;
        public DateTime systemTime;

        private byte[] omsiTimeDateBytes = new byte[39];

        // Is OMSI loaded into a map?
        public bool omsiLoaded = false;

        // The version of OMSI currently running (or unknown)
        public string omsiVersion = "Unknown";

        // The map that OMSI is currently running (or unknown)
        public string omsiMap = "Unknown";

        // Is OMSI running and this tool has been successfully attached to it?
        public bool processAttached = false;

        // For accessing or writing to OMSI's memory
        public Mem m;

        // Hours difference for auto detecting offset time
        public double hoursDifference = 0.0;

        // For moving the form UI around the screen
        public bool isFormBeingDragged = false;
        public Point picPointClicked;

        // Timers on different threads from the UI
        public System.Threading.Timer tmrAutoSave;
        public System.Threading.Timer tmrBackground;

        public frmMain()
        {
            InitializeComponent();
        }

        // Get the current OMSI version
        private string getOmsiVersion()
        {
            // If process is attached
            if (processAttached)
            {
                try
                {
                    // Generate an MD5 hash of the Omsi.exe file in order to confirm the version of OMSI
                    byte[] hash;
                    using (var inputStream = File.OpenRead(m.theProc.MainModule.FileName))
                    {
                        MD5 md5 = MD5.Create();
                        hash = md5.ComputeHash(inputStream);
                        
                        // Convert the byte array to hexadecimal string
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < hash.Length; i++)
                        {
                            sb.Append(hash[i].ToString("X2"));
                        }

                        string md5Hash = sb.ToString().ToUpper();

                        switch (md5Hash)
                        {
                            // Without 4GB patch
                            // With 4GB patch
                            case "9A92C43C98ACD4C4A4E8C9F1BA56D906":
                            case "E5F61D164F4C2374513C4CA9AC1AF635":
                                return "2.3.004";

                            case "E7515B2A2124AD2BF10CA4007D7A0689":
                            case "656C9ED8E87BE60744F55E14A43CEA0E":
                                return "2.2.032";
                        }
                    }
                }
                catch { }
            }

            // If all else fails, return the version as:
            // Unknown
            return "Unknown";
        }

        // Get the current date and time in OMSI
        private bool getOmsiTime()
        {
            if (processAttached)
            {
                try
                {
                    omsiTimeDateBytes = m.ReadBytes(Omsi.getMemoryAddress(omsiVersion, "hour"), 40);

                    int i = 0;
                    string dateStr = "";
                    // HH:mm:ss.ms dd/MM/YYYY
                    dateStr = dateStr + (int)omsiTimeDateBytes[i] + ":"; i++;                       // Hour
                    dateStr = dateStr + (int)omsiTimeDateBytes[i] + ":"; i += 3;                    // Minute
                    dateStr = dateStr + BitConverter.ToSingle(omsiTimeDateBytes, i) + " "; i += 8;  // Second.Millisecond
                    dateStr = dateStr + BitConverter.ToInt32(omsiTimeDateBytes, i) + "/"; i += 20;  // Day
                    dateStr = dateStr + BitConverter.ToInt32(omsiTimeDateBytes, i) + "/"; i += 4;   // Month
                    dateStr = dateStr + BitConverter.ToInt32(omsiTimeDateBytes, i);                 // Year
                

                    return DateTime.TryParse(dateStr, out omsiTime);
                }
                catch { }
            }

            return false;
        }

        // Set the date and time in OMSI
        private bool syncOmsiTime()
        {
            try
            {
                // Is OMSI's process attached to this tool AND is OMSI loaded into a map?
                if (processAttached && omsiLoaded)
                {
                    // Get the time difference in seconds between the actual date and time and OMSI's date and time
                    double timeDifference = (systemTime - omsiTime).TotalSeconds;

                    // If either:
                    // - Only resync OMSI time if behind actual time is disabled
                    // * OR *
                    // - Only resync OMSI time if behind actual time is enabled AND the time difference is greater than 1.0 seconds
                    if (
                        (!AppConfig.onlyResyncOmsiTimeIfBehindActualTime) ||
                        (AppConfig.onlyResyncOmsiTimeIfBehindActualTime && timeDifference > 1.0)
                       )
                    {
                        // Auto Sync Mode:
                        // 0  - Always
                        // 1  - Only when bus is moving
                        // 2  - Only when bus is not moving
                        // 3  - Only when bus has a timetable
                        // 4  - Only when bus has no timetable
                        // 5  - Only when game is paused
                        if (
                            AppConfig.autoSyncModeIndex == 0 ||
                            (
                             AppConfig.autoSyncModeIndex == 1 &&
                             OmsiTelemetry.busSpeedKph > 0.0
                            ) ||
                            (
                             AppConfig.autoSyncModeIndex == 2 &&
                             OmsiTelemetry.busSpeedKph == 0.0
                            ) ||
                            (
                             AppConfig.autoSyncModeIndex == 3 &&
                             OmsiTelemetry.scheduleActive == 1
                            ) ||
                            (
                             AppConfig.autoSyncModeIndex == 4 &&
                             OmsiTelemetry.scheduleActive == 0
                            ) ||
                            (
                             AppConfig.autoSyncModeIndex == 5 &&
                             OmsiTelemetry.isPaused == 1
                            )
                           )
                        {
                            // Get current system date and time
                            DateTime newSystemTime = systemTime;

                            // This should prevent a rare scenario where BCS thinks the time has been set in the past
                            if (AppConfig.onlyResyncOmsiTimeIfBehindActualTime)
                            {
                                // If only resync OMSI time if behind actual time is enabled then:
                                // - Add two seconds to the system time retrieved a moment ago
                                newSystemTime = newSystemTime.AddSeconds(2.0);
                            }

                            // 09 39 00 00 BB 2F 15 41 09 00 00 00 09 00 00 00 00 00 00 00 94 A5 00 24 00 00 00 00 00 00 00 00 01 00 00 00 E6 07 00 00
                            // Hour, Minute, Second, Day, Month, Year
                            // Byte, Byte,   Float,  Int, Int,   Int
                            // The following should be compatible with latest OMSI, untested on 'tram'
                            int i = 0;
                            byte[] writeBytes = omsiTimeDateBytes;
                            writeBytes[i] = Convert.ToByte(newSystemTime.Hour); i++; // 0 - 1
                            writeBytes[i] = Convert.ToByte(newSystemTime.Minute); i += 3; // 1 - 4
                            BitConverter.GetBytes(Convert.ToSingle(newSystemTime.Second + "." + newSystemTime.Millisecond)).CopyTo(writeBytes, i); i += 8; // 4 - 12
                            BitConverter.GetBytes(newSystemTime.Day).CopyTo(writeBytes, i); i += 20; // 12 - 32
                            BitConverter.GetBytes(newSystemTime.Month).CopyTo(writeBytes, i); i += 4; // 32 - 36
                            BitConverter.GetBytes(newSystemTime.Year).CopyTo(writeBytes, i); // 35 - 39

                            // Set the new date and time
                            m.WriteMemory(Omsi.getMemoryAddress(omsiVersion, "hour"), "bytes", writeBytes);
                        }
                    }

                    // Get the latest date and time in OMSI again
                    return getOmsiTime();
                }
            }
            catch { }

            return false;
        }

        // Load the app config
        private bool loadConfig()
        {
            try
            {
                TextReader txtRdr = new StreamReader("OmsiTimeSync.cfg");

                AppConfig.alwaysOnTop = Convert.ToBoolean(txtRdr.ReadLine());
                AppConfig.autoSyncOmsiTime = Convert.ToBoolean(txtRdr.ReadLine());
                AppConfig.onlyResyncOmsiTimeIfBehindActualTime = Convert.ToBoolean(txtRdr.ReadLine());
                AppConfig.offsetHour = Math.Max(-23, Math.Min(23, Convert.ToInt32(txtRdr.ReadLine())));
                AppConfig.offsetHourIndex = Convert.ToInt32(txtRdr.ReadLine());
                AppConfig.windowPositionLeft = Convert.ToInt32(txtRdr.ReadLine());
                AppConfig.windowPositionTop = Convert.ToInt32(txtRdr.ReadLine());
                AppConfig.manualSyncHotkeyIndex = Convert.ToInt32(txtRdr.ReadLine());
                AppConfig.autoSyncModeIndex = Convert.ToInt32(txtRdr.ReadLine());
                AppConfig.manualSyncHotkeySound = Convert.ToBoolean(txtRdr.ReadLine());
                AppConfig.autoDetectOffsetHours = Convert.ToBoolean(txtRdr.ReadLine());

                return true;
            }
            catch
            {
                // If something goes wrong then use the default settings
                AppConfig.alwaysOnTop = AppConfigDefaults.alwaysOnTop;
                AppConfig.autoSyncOmsiTime = AppConfigDefaults.autoSyncOmsiTime;
                AppConfig.onlyResyncOmsiTimeIfBehindActualTime = AppConfigDefaults.onlyResyncOmsiTimeIfBehindActualTime;
                AppConfig.offsetHour = AppConfigDefaults.offsetHour;
                AppConfig.offsetHourIndex = AppConfigDefaults.offsetHourIndex;
                AppConfig.windowPositionLeft = AppConfigDefaults.windowPositionLeft;
                AppConfig.windowPositionTop = AppConfigDefaults.windowPositionTop;
                AppConfig.manualSyncHotkeyIndex = AppConfigDefaults.manualSyncHotkeyIndex;
                AppConfig.autoSyncModeIndex = AppConfigDefaults.autoSyncModeIndex;
                AppConfig.manualSyncHotkeySound = AppConfigDefaults.manualSyncHotkeySound;
                AppConfig.autoDetectOffsetHours = AppConfigDefaults.autoDetectOffsetHours;

                return false;
            }
        }

        // SAve the app config
        private bool saveConfig()
        {
            try
            {
                TextWriter txtWtr = new StreamWriter("OmsiTimeSync.cfg");

                txtWtr.WriteLine(AppConfig.alwaysOnTop.ToString());
                txtWtr.WriteLine(AppConfig.autoSyncOmsiTime.ToString());
                txtWtr.WriteLine(AppConfig.onlyResyncOmsiTimeIfBehindActualTime.ToString());
                txtWtr.WriteLine(AppConfig.offsetHour.ToString());
                txtWtr.WriteLine(AppConfig.offsetHourIndex.ToString());
                txtWtr.WriteLine(AppConfig.windowPositionLeft.ToString());
                txtWtr.WriteLine(AppConfig.windowPositionTop.ToString());
                txtWtr.WriteLine(AppConfig.manualSyncHotkeyIndex.ToString());
                txtWtr.WriteLine(AppConfig.autoSyncModeIndex.ToString());
                txtWtr.WriteLine(AppConfig.manualSyncHotkeySound.ToString());
                txtWtr.WriteLine(AppConfig.autoDetectOffsetHours.ToString());

                txtWtr.Close();

                return true;
            }
            catch { return false; }
        }

        // Background timer that runs every 60 seconds
        private void tmrAutoSave_Tick(object state)
        {
            saveConfig();
        }

        // Background timer that runs every 1 second
        private void tmrBackground_Tick(object state)
        {
            // If process isn't already attached
            if (!processAttached)
            {
                // Search for Omsi.exe process
                int processID = Process.GetCurrentProcess().Id;

                // If a process was found
                if (processID > 0)
                {
                    // Attach to the process
                    processAttached = m.OpenProcess(processID);
                }
            }

            // If auto detection of offset hours is enabled then:
            // 
            if (AppConfig.autoDetectOffsetHours)
            {
                if (processAttached &&
                    omsiLoaded &&
                    Omsi.isVersionSupported(omsiVersion) &&
                    systemTime == DateTime.MinValue)
                {
                    systemTime = DateTime.Now;

                    hoursDifference = Math.Round((omsiTime - systemTime).TotalHours);

                    systemTime = systemTime.AddHours(hoursDifference);
                }
                else if (!processAttached ||
                    !omsiLoaded ||
                    !Omsi.isVersionSupported(omsiVersion))
                {
                    systemTime = DateTime.MinValue;

                    lblSystemTime.Text = "Waiting for OMSI map...";
                }
            }

            if (!AppConfig.autoDetectOffsetHours)
            {
                // Adjust the actual time by the number of 'offset hours' that is set in the UI
                systemTime = DateTime.Now.AddHours(AppConfig.offsetHour);

                // Display the actual time in the UI
                lblSystemTime.Text = systemTime.ToString();
            }
            else if (AppConfig.autoDetectOffsetHours && systemTime != DateTime.MinValue)
            {
                systemTime = DateTime.Now.AddHours(hoursDifference);

                // Display the actual time in the UI
                lblSystemTime.Text = systemTime.ToString();
            }

            // If a process is attached
            if (processAttached)
            {
                // If OMSI version is currently unknown then try to identify what the version is
                if (omsiVersion == "Unknown")
                {
                    omsiVersion = getOmsiVersion();
                }
                
                // If the OMSI version is still unknown then we assume OMSI is still loading
                // Further code execution stops here until the version can be identified
                if (omsiVersion == "Unknown")
                {
                    resetFormTitleBarValues();

                    return;
                }

                // Check if OMSI version is supported, if it's not then stop further code execution and explain in the UI
                if (!Omsi.isVersionSupported(omsiVersion))
                {
                    omsiLoaded = false;

                    lblOmsiTime.Text = "OMSI version '" + omsiVersion + "' is not supported";
                    lblOmsiTime.ForeColor = Color.Red;

                    return;
                }
                
                // If OMSI time text isn't 'ControlText' colour then make it so again
                if (lblOmsiTime.ForeColor != SystemColors.ControlText)
                {
                    lblOmsiTime.ForeColor = SystemColors.ControlText;
                }

                try
                {
                    // Open logfile.txt but allow other applications to still read/write to the logfile.txt file
                    using (var fs = new FileStream("logfile.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        string line;

                        // Iterate through each line
                        while ((line = sr.ReadLine()) != null)
                        {
                            // This indicates OMSI has loaded a map
                            if (line.Contains("Information: Traffic loaded"))
                            {
                                omsiLoaded = true;
                            }
                            // This indicates OMSI has unloaded a map
                            else if (line.Contains("Information: Actual map closed!"))
                            {
                                omsiLoaded = false;
                            }
                        }

                        sr.Close();
                        fs.Close();
                    }
                }
                catch { return; }

                if (omsiLoaded)
                {
                    omsiLoaded = getOmsiTime();
                }

                // If OMSI isn't loaded into a map
                if (!omsiLoaded)
                {
                    resetFormTitleBarValues();

                    // Indicate that OMSI is running but isn't loaded into a map yet
                    lblOmsiTime.Text = "OMSI is running, waiting for a map to load!";

                    omsiMap = "Unknown";

                    // Don't execute any further code yet
                    return;
                }
                else
                {
                    // Try and identify the map that OMSI has loaded
                    try
                    {
                        int firstValue = m.ReadInt(0x00461588, true);
                        int secondValue = m.ReadInt(firstValue + 0x154, false);
                        byte[] thirdValue = m.ReadBytes(secondValue + 0x0, 64, false, true);

                        omsiMap = Encoding.UTF8.GetString(thirdValue).Replace('\\', '/').ToLower().Replace("global.cfg", "");

                        if (Directory.Exists(omsiMap))
                        {
                            // Attempt to get all of the bus stop names for the current map
                            BusStopsCfgReader.readBusStopsCfg(omsiMap + "TTData/BusStops.cfg");
                            TTPReader.readTTPFiles(omsiMap + "TTData/");
                        }
                        else
                        {
                            omsiMap = "Unknown";
                        }
                    }
                    catch { omsiMap = "Unknown"; }
                }

                // If auto sync OMSI time is enabled
                if (AppConfig.autoSyncOmsiTime && systemTime != DateTime.MinValue)
                {
                    // Go ahead with syncing OMSI time
                    syncOmsiTime();
                }
                //else
                //{
                //    getOmsiTime();
                //}

                if (formTitleBarCurrentOmsiSpeed.Text.EndsWith("MPH"))
                {
                    formTitleBarCurrentOmsiSpeed.Text = Math.Round(OmsiTelemetry.busSpeedKph * 0.6213711922).ToString().PadRight(3) + "MPH";
                }
                else if (formTitleBarCurrentOmsiSpeed.Text.EndsWith("KMH"))
                {
                    formTitleBarCurrentOmsiSpeed.Text = Math.Round(OmsiTelemetry.busSpeedKph).ToString().PadRight(3) + "KMH";
                }

                formTitleBarCurrentOmsiTime.Text = omsiTime.ToString("HH:mm:ss");

                string omsiDelayStr = "-";
                int omsiDelay = 0;

                string omsiNextBusStop = "-";

                if (OmsiTelemetry.scheduleActive == 1.0f)
                {
                    try
                    {
                        int firstValue = m.ReadInt(0x00461500, true);
                        int secondValue = m.ReadInt(firstValue + 0x6BC, false);

                        omsiDelay = secondValue;

                        TimeSpan omsiDelayTime = TimeSpan.FromSeconds(omsiDelay);

                        omsiDelayStr = string.Format("{0:D2}:{1:D2}", (int)Math.Abs(omsiDelayTime.TotalMinutes), Math.Abs(omsiDelayTime.Seconds));
                    }
                    catch { omsiDelayStr = "-"; omsiDelay = 0; }

                    try
                    {
                        int firstValue = m.ReadInt(0x00461500, true);
                        int secondValue = m.ReadInt(firstValue + 0x6B0, false);

                        string busStopName;

                        busStopName = BusStopsCfgReader.findBusStop(secondValue);

                        if (busStopName != null)
                        {
                            omsiNextBusStop = busStopName;
                        }
                        else
                        {
                            busStopName = TTPReader.findBusStop(secondValue);

                            if (busStopName != null)
                            {
                                omsiNextBusStop = busStopName;
                            }
                            else
                            {
                                omsiNextBusStop = "-";
                            }
                        }
                    }
                    catch { omsiNextBusStop = "-"; }
                }

                if (omsiDelay < 0) formTitleBarCurrentOmsiDelay.ForeColor = Color.FromArgb(192, 255, 192);
                else if (omsiDelay > 0) formTitleBarCurrentOmsiDelay.ForeColor = Color.FromArgb(255, 192, 192);
                else formTitleBarCurrentOmsiDelay.ForeColor = Color.White;

                if (omsiDelay < 0 && omsiDelayStr != "-") formTitleBarCurrentOmsiDelay.Text = "-" + omsiDelayStr;
                else if (omsiDelay >= 0 && omsiDelayStr != "-") formTitleBarCurrentOmsiDelay.Text = "+" + omsiDelayStr;
                else formTitleBarCurrentOmsiDelay.Text = "-";

                formTitleBarNextBusStop.Text = omsiNextBusStop;

                if (OmsiTelemetry.busStoppingLight != 0.0f)
                {
                    if (formTitleBarBusStopRequest.ForeColor == Color.FromArgb(64, 64, 64))
                    {
                        formTitleBarBusStopRequest.ForeColor = Color.White;
                    }
                    else
                    {
                        formTitleBarBusStopRequest.ForeColor = Color.FromArgb(64, 64, 64);
                    }
                }
                else
                {
                    formTitleBarBusStopRequest.ForeColor = Color.FromArgb(64, 64, 64);
                }

                // State the current date and time of OMSI in the UI
                lblOmsiTime.Text = omsiTime.ToString();
            }
            else
            {
                // State that 'OMSI is not running' in the UI
                lblOmsiTime.Text = "OMSI is not running!";
                lblOmsiTime.ForeColor = Color.Red;
            }
        }

        private void resetFormTitleBarValues()
        {
            if (formTitleBarCurrentOmsiSpeed.Text.EndsWith("MPH"))
            {
                formTitleBarCurrentOmsiSpeed.Text = Math.Round(OmsiTelemetry.busSpeedKph * 0.6213711922).ToString().PadRight(3) + "MPH";
            }
            else if (formTitleBarCurrentOmsiSpeed.Text.EndsWith("KMH"))
            {
                formTitleBarCurrentOmsiSpeed.Text = Math.Round(OmsiTelemetry.busSpeedKph).ToString().PadRight(3) + "KMH";
            }

            formTitleBarCurrentOmsiTime.Text = "-";
            formTitleBarCurrentOmsiDelay.Text = "-";
            formTitleBarCurrentOmsiDelay.ForeColor = Color.White;
            formTitleBarNextBusStop.Text = "-";
        }

        // For handling the state of auto syncing OMSI time
        private void chkAutoSyncOmsiTime_CheckedChanged(object sender, EventArgs e)
        {
            AppConfig.autoSyncOmsiTime = chkAutoSyncOmsiTime.Checked;

            btnManualSyncOmsiTime.Enabled = !chkAutoSyncOmsiTime.Checked;
        }

        // For handling the state of only resyncing OMSI time if it's behind the actual time
        private void chkOnlyResyncOmsiTimeIfBehindActualTime_CheckedChanged(object sender, EventArgs e)
        {
            AppConfig.onlyResyncOmsiTimeIfBehindActualTime = chkOnlyResyncOmsiTimeIfBehindActualTime.Checked;
        }

        // For handling the 'offset hours' setting
        private void cmbOffsetHours_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.offsetHour = Convert.ToInt32(cmbOffsetHours.SelectedItem);
            AppConfig.offsetHourIndex = cmbOffsetHours.SelectedIndex;
        }

        // For manually syncing OMSI's time when pressing the button on the UI
        private void btnManualSyncOmsiTime_Click(object sender, EventArgs e)
        {
            // Attempt to sync OMSI's time with the actual time, but if it fails...
            if (!syncOmsiTime())
            {
                // Show an error message stating that it failed for some reason
                MessageBox.Show("ERROR: Unable to sync OMSI time. Please check that OMSI is running and a map has been loaded.", "OMSI Time Sync - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // For handling the auto sync mode setting
        private void cmbAutoSyncMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.autoSyncModeIndex = cmbAutoSyncMode.SelectedIndex;
        }

        // For handling the auto detect offset time setting
        private void chkAutoDetectOffsetTime_CheckedChanged(object sender, EventArgs e)
        {
            // Disable combo box for manually adjusting the time offset if auto detect is enabled and vice versa
            cmbOffsetHours.Enabled = !chkAutoDetectOffsetTime.Checked;

            AppConfig.autoDetectOffsetHours = chkAutoDetectOffsetTime.Checked;
        }

        // Github link
        private void lnkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://github.com/Ixe1/OMSI-Time-Sync");
        }

        // Donate link
        private void lnkDonate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://paypal.me/ixe1");
        }

        // Form loading event
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Disable the close button
            EnableMenuItem(GetSystemMenu(Handle, false), 0xF060, 1);

            // Load app config
            loadConfig();

            // If app config has a previous window position then apply it to the UI
            if (AppConfig.windowPositionTop != -1 && AppConfig.windowPositionLeft != -1)
            {
                StartPosition = FormStartPosition.Manual;

                Top = AppConfig.windowPositionTop;
                Left = AppConfig.windowPositionLeft;
            }

            // Setup the dropdown lists so they are configured based on the app's config
            cmbOffsetHours.SelectedIndex = AppConfig.offsetHourIndex;
            cmbAutoSyncMode.SelectedIndex = AppConfig.autoSyncModeIndex;

            // Same with checkboxes
            chkAutoSyncOmsiTime.Checked = AppConfig.autoSyncOmsiTime;
            chkOnlyResyncOmsiTimeIfBehindActualTime.Checked = AppConfig.onlyResyncOmsiTimeIfBehindActualTime;
            chkAutoDetectOffsetTime.Checked = AppConfig.autoDetectOffsetHours;

            if (AppConfig.alwaysOnTop != AppConfigDefaults.alwaysOnTop)
            {
                refreshButtonAlwaysOnTop();
            }

            // Add 'key released' event for manual sync hotkey
            //gkhManualSyncHotkey.KeyUp += new KeyEventHandler(manualSyncHotkey_KeyUp);

            // If OmsiTimeSync.cfg doesn't exist
            if (!File.Exists("OmsiTimeSync.cfg"))
            {
                // Show initial message box dialog (yes/no)
                if (MessageBox.Show(
                    "Thanks for downloading and running OMSI Time Sync.\n" +
                    "\n" +
                    "It's important that you close any games that have anti-cheat protection before pressing 'Yes'! This program performs memory editing which might be falsely flagged as a hack.\n" +
                    "\n" +
                    "This notice will not be shown again unless the 'OmsiTimeSync.cfg' file is deleted in OMSI's directory. The author of this program will not be liable.\n" +
                    "\n" +
                    "While this is a free program, a donation is highly appreciated if you like this program.\n" +
                    "\n" +
                    "Do you acknowledge the above notice and agree?",
                    "OMSI Time Sync", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // If 'no' is chosen then close the app
                    this.Close();
                    Application.Exit();

                    // Don't execute further code
                    return;
                }
            }

            // For accessing and writing to OMSI's memory later on
            m = new Mem();

            // Add supported OMSI memory addresses

            // v2.3.004
            // Date/Time
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("hour", "0x0046176C"));     // byte (h)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("minute", "0x0046176D"));   // byte (m)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("second", "0x00461770"));   // float (second.millisecond)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("year", "0x00461790"));     // int (yyyy)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("month", "0x0046178C"));    // int (m)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("day", "0x00461778"));      // int (d)

            // 0x4 difference?

            // v2.2.032
            // Date/Time
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("hour", "0x00461768"));     // byte (h)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("minute", "0x00461769"));   // byte (m)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("second", "0x0046176C"));   // float (second.millisecond)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("year", "0x0046178C"));     // int (yyyy)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("month", "0x00461788"));    // int (m)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("day", "0x00461774"));      // int (d)

            // Setup the autosave timer
            tmrAutoSave = new System.Threading.Timer(new System.Threading.TimerCallback(tmrAutoSave_Tick), null, 60000, 60000);

            // Setup the background timer which does various stuff
            tmrBackground = new System.Threading.Timer(new System.Threading.TimerCallback(tmrBackground_Tick), null, 1000, 1000);
        }

        // Form closing event
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the timer is enabled
            if (tmrBackground != null)
            {
                // Save app config
                saveConfig();

                e.Cancel = true;
            }
        }

        private void frmMain_LocationChanged(object sender, EventArgs e)
        {
            // If the timer is enabled
            if (tmrBackground != null)
            {
                // Set current window position in app's config
                AppConfig.windowPositionTop = Top;
                AppConfig.windowPositionLeft = Left;
            }
        }

        private void formTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isFormBeingDragged)
            {
                Point pointMoveTo;
                pointMoveTo = this.PointToScreen(new Point(e.X, e.Y));
                pointMoveTo.Offset(-picPointClicked.X, -picPointClicked.Y);
                Location = pointMoveTo;
            }
        }

        private void formTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isFormBeingDragged = true;
                picPointClicked = new Point(e.X, e.Y);
            }
            else
            {
                isFormBeingDragged = false;
            }
        }

        private void formTitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            isFormBeingDragged = false;
            
            // If the timer is enabled
            if (tmrBackground != null)
            {
                // Set current window position in app's config
                AppConfig.windowPositionTop = Top;
                AppConfig.windowPositionLeft = Left;
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            Focus();
            BringToFront();
            TopLevel = true;
        }

        private void formTitleBarMinimise_MouseEnter(object sender, EventArgs e)
        {
            formTitleBarMinimise.ForeColor = Color.LightGray;
        }

        private void formTitleBarMinimise_MouseLeave(object sender, EventArgs e)
        {
            formTitleBarMinimise.ForeColor = Color.White;
        }

        private void formTitleBarExpandCompact_MouseEnter(object sender, EventArgs e)
        {
            formTitleBarExpandCompact.ForeColor = Color.LightGray;
        }

        private void formTitleBarExpandCompact_MouseLeave(object sender, EventArgs e)
        {
            formTitleBarExpandCompact.ForeColor = Color.White;
        }

        private void formTitleBarPinUnpin_MouseEnter(object sender, EventArgs e)
        {
            formTitleBarPinUnpin.ForeColor = Color.LightGray;
        }

        private void formTitleBarPinUnpin_MouseLeave(object sender, EventArgs e)
        {
            formTitleBarPinUnpin.ForeColor = Color.White;
        }

        private void formTitleBarMinimise_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void formTitleBarExpandCompact_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (formTitleBarExpandCompact.Text == "5")
                {
                    Height = 57;
                    formTitleBarExpandCompact.Text = "6";
                }
                else
                {
                    Height = 265;
                    formTitleBarExpandCompact.Text = "5";
                }
            }
        }

        private void formTitleBarPinUnpin_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AppConfig.alwaysOnTop = !AppConfig.alwaysOnTop;

                refreshButtonAlwaysOnTop();
            }
        }

        private void refreshButtonAlwaysOnTop()
        {
            TopMost = AppConfig.alwaysOnTop;
            TopLevel = true;

            if (AppConfig.alwaysOnTop)
            {
                formTitleBarPinUnpin.BackColor = Color.DarkGreen;
            }
            else
            {
                formTitleBarPinUnpin.BackColor = Color.DarkRed;
            }
        }

        private void formTitleBarCurrentOmsiSpeed_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (formTitleBarCurrentOmsiSpeed.Text.EndsWith("MPH"))
            {
                formTitleBarCurrentOmsiSpeed.Text = Math.Round(OmsiTelemetry.busSpeedKph).ToString().PadRight(3) + "KMH";
            }
            else if (formTitleBarCurrentOmsiSpeed.Text.EndsWith("KMH"))
            {
                formTitleBarCurrentOmsiSpeed.Text = Math.Round(OmsiTelemetry.busSpeedKph * 0.6213711922).ToString().PadRight(3) + "MPH";
            }
        }
    }

    // Struct for OMSI memory addresses
    public struct OmsiAddress
    {
        // For example, hour, month, day, minute, etc.
        public readonly string addressType;
        // For example, base+0x00000000
        public readonly string addressLocation;

        // Constructor
        public OmsiAddress(string addressType, string addressLocation)
        {
            this.addressType = addressType;
            this.addressLocation = addressLocation;
        }
    }

    // Important addresses in memory for the OMSI process
    static class Omsi
    {
        // < [OMSI_VERSION], { [ADDRESS_TYPE], "[ADDRESS_LOCATION]" } >
        // < "2.3.004", { hour, "base+0x0046176C" } >
        private static Dictionary<string, List<OmsiAddress>> omsiAddresses = new Dictionary<string, List<OmsiAddress>>();

        // Add a memory address to what's supported by this program
        public static void addMemoryAddress(string version, OmsiAddress omsiAddress)
        {
            // If version doesn't already exist then make a version entry in the dictionary with the memory address
            if (!omsiAddresses.ContainsKey(version))
            {
                omsiAddresses.Add(version, new List<OmsiAddress>());
                omsiAddresses.Last().Value.Add(new OmsiAddress(omsiAddress.addressType, omsiAddress.addressLocation));
            }
            // Else append the memory address to the existing dictionary entry
            else
            {
                omsiAddresses[version].Add(omsiAddress);
            }
        }

        // Find a memory address from what's supported by this program and return the address (if supported)
        // Otherwise return null
        public static string getMemoryAddress(string version, string addressType)
        {
            List<OmsiAddress> addresses;

            if (omsiAddresses.TryGetValue(version, out addresses))
            {
                int index = addresses.FindIndex(s => s.addressType == addressType);

                if (index >= 0)
                {
                    return addresses[index].addressLocation;
                }
            }

            return null;
        }

        // Simple check to verify if the OMSI version is supported by this program
        public static bool isVersionSupported(string version)
        {
            if (omsiAddresses.ContainsKey(version)) return true;

            return false;
        }
    }

    // This app's config
    static class AppConfig
    {
        public static bool alwaysOnTop = AppConfigDefaults.alwaysOnTop;
        public static bool autoSyncOmsiTime = AppConfigDefaults.autoSyncOmsiTime;
        public static bool onlyResyncOmsiTimeIfBehindActualTime = AppConfigDefaults.onlyResyncOmsiTimeIfBehindActualTime;
        public static int offsetHour = AppConfigDefaults.offsetHour;
        public static int offsetHourIndex = AppConfigDefaults.offsetHourIndex;
        public static int windowPositionLeft = AppConfigDefaults.windowPositionLeft;
        public static int windowPositionTop = AppConfigDefaults.windowPositionTop;
        public static int manualSyncHotkeyIndex = AppConfigDefaults.manualSyncHotkeyIndex;
        public static int autoSyncModeIndex = AppConfigDefaults.autoSyncModeIndex;
        public static bool manualSyncHotkeySound = AppConfigDefaults.manualSyncHotkeySound;
        public static bool autoDetectOffsetHours = AppConfigDefaults.autoDetectOffsetHours;
    }

    // This app's default config
    static class AppConfigDefaults
    {
        public static bool alwaysOnTop = false;
        public static bool autoSyncOmsiTime = true;
        public static bool onlyResyncOmsiTimeIfBehindActualTime = true;
        public static int offsetHour = 0;
        public static int offsetHourIndex = 23;
        public static int windowPositionLeft = -1;
        public static int windowPositionTop = -1;
        public static int manualSyncHotkeyIndex = 0;
        public static int autoSyncModeIndex = 0;
        public static bool manualSyncHotkeySound = false;
        public static bool autoDetectOffsetHours = true;
    }
}
