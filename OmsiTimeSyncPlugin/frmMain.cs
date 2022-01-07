using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace OmsiTimeSyncPlugin
{
    public partial class frmMain : Form
    {
        [DllImport("user32")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint itemId, uint uEnable);


        public DateTime omsiTime = DateTime.MinValue;
        public DateTime systemTime;

        // Is OMSI loaded into a map?
        public bool omsiLoaded = false;

        // The version of OMSI currently running (or unknown)
        public string omsiVersion = "Unknown";

        // Is OMSI running and this tool has been successfully attached to it?
        public bool processAttached = false;

        // For accessing or writing to OMSI's memory
        public Mem m;

        // For hotkey support
        OmsiTimeSyncPluginMisc.globalKeyboardHook gkhManualSyncHotkey = new OmsiTimeSyncPluginMisc.globalKeyboardHook();

        // Hours difference for auto detecting offset time
        public double hoursDifference = 0.0;

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
                                return "2.2.32";
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
                // dd/mm/yyyy hh:mm:ss
                string dateStr = m.ReadInt(Omsi.getMemoryAddress(omsiVersion, "day")).ToString("D2") + "/" + 
                    m.ReadInt(Omsi.getMemoryAddress(omsiVersion, "month")).ToString("D2") + "/" + 
                    m.ReadInt(Omsi.getMemoryAddress(omsiVersion, "year")).ToString("D4") + " " + 
                    m.ReadByte(Omsi.getMemoryAddress(omsiVersion, "hour")).ToString("D2") + ":" + 
                    m.ReadByte(Omsi.getMemoryAddress(omsiVersion, "minute")).ToString("D2") + ":" + 
                    ((int)Math.Max(0, Math.Min(59, Math.Ceiling(m.ReadFloat(Omsi.getMemoryAddress(omsiVersion, "second")))))).ToString("D2");

                return DateTime.TryParse(dateStr, out omsiTime);
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
                            )
                           )
                        {
                            // Get current system date and time
                            DateTime newSystemTime = systemTime;

                            // This should prevent a rare scenario where BCS thinks the time has been set in the past
                            if (AppConfig.onlyResyncOmsiTimeIfBehindActualTime)
                            {
                                // If only resync OMSI time if behind actual time is enabled then:
                                // - Add three seconds to the system time retrieved a moment ago
                                newSystemTime = newSystemTime.AddSeconds(3.0);
                            }

                            // Game needs to be paused momentarily to prevent BCS thinking time is going backwards if near the end of the current minute or hour
                            // TODO: Check the following conditional statement works before implementing a brief pause

                            // If current OMSI second is less than 55 then it's safe to increment time (this is due to BCS thinking time is going backwards otherwise)
                            // Apply the new date and time in OMSI by modifying some of the addresses in memory
                            if (omsiTime.Second < 55)
                            {
                                m.WriteMemory(Omsi.getMemoryAddress(omsiVersion, "hour"), "byte", newSystemTime.Hour.ToString());
                                m.WriteMemory(Omsi.getMemoryAddress(omsiVersion, "minute"), "byte", newSystemTime.Minute.ToString());
                                m.WriteMemory(Omsi.getMemoryAddress(omsiVersion, "second"), "float", newSystemTime.Second.ToString() + "." + newSystemTime.Millisecond.ToString());

                                m.WriteMemory(Omsi.getMemoryAddress(omsiVersion, "day"), "int", newSystemTime.Day.ToString());
                                m.WriteMemory(Omsi.getMemoryAddress(omsiVersion, "month"), "int", newSystemTime.Month.ToString());
                                m.WriteMemory(Omsi.getMemoryAddress(omsiVersion, "year"), "int", newSystemTime.Year.ToString());
                            }
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

        // Timer that runs every 1 second
        private void tmrOMSI_Tick(object sender, EventArgs e)
        {
            // Search for Omsi.exe process
            int processID = Process.GetCurrentProcess().Id;

            // If process isn't already attached
            if (!processAttached)
            {
                // If a process was found
                if (processID > 0)
                {
                    // Attach to the process
                    processAttached = m.OpenProcess(processID);
                }
            }

            // If a process can't be found and one is attached
            if (processID <= 0 && processAttached)
            {
                // De-attach process
                processAttached = false;

                omsiVersion = "Unknown";

                m.CloseProcess();
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
                    return;
                }

                // Check if OMSI version is supported, if it's not then stop further code execution and explain in the UI
                if (!Omsi.isVersionSupported(omsiVersion))
                {
                    omsiLoaded = false;

                    lblOmsiTime.Text = "OMSI version '" + omsiVersion + "' is not supported";
                    lblOmsiTime.ForeColor = System.Drawing.Color.Red;

                    return;
                }
                
                // If OMSI time text isn't 'ControlText' colour then make it so again
                if (lblOmsiTime.ForeColor != System.Drawing.SystemColors.ControlText)
                {
                    lblOmsiTime.ForeColor = System.Drawing.SystemColors.ControlText;
                }

                // If getOmsiTime() is true then OMSI is loaded into a map with a valid date and time
                omsiLoaded = getOmsiTime();

                // If OMSI isn't loaded into a map
                if (!omsiLoaded)
                {
                    // Indicate that OMSI is running but isn't loaded into a map yet
                    lblOmsiTime.Text = "OMSI is running, waiting for a map to load!";

                    // Don't execute any further code yet
                    return;
                }

                // If auto sync OMSI time is enabled
                if (AppConfig.autoSyncOmsiTime)
                {
                    // Go ahead with syncing OMSI time
                    syncOmsiTime();
                }

                // State the current date and time of OMSI in the UI
                lblOmsiTime.Text = omsiTime.ToString();
            }
            else
            {
                // State that 'OMSI is not running' in the UI
                lblOmsiTime.Text = "OMSI is not running!";
                lblOmsiTime.ForeColor = System.Drawing.Color.Red;
            }
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

        // For handling the state of 'always on top'
        private void chkAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            AppConfig.alwaysOnTop = chkAlwaysOnTop.Checked;
            TopMost = chkAlwaysOnTop.Checked;
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

        // For handling the manual sync hotkey setting
        private void cmbManualSyncHotkey_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If there are already hotkeys being monitored
            if (gkhManualSyncHotkey.HookedKeys.Count > 0)
            {
                // Clear them
                gkhManualSyncHotkey.HookedKeys.Clear();
            }

            // If the hotkey preference isn't 'none'
            if ((Keys)cmbManualSyncHotkey.SelectedItem != Keys.None)
            {
                // Add the hotkey to be monitored
                gkhManualSyncHotkey.HookedKeys.Add((Keys)cmbManualSyncHotkey.SelectedItem);
            }

            // If the dropdown list is visible then apply the current choice from the dropdown list to the app's config
            if (cmbManualSyncHotkey.Visible) AppConfig.manualSyncHotkeyIndex = cmbManualSyncHotkey.SelectedIndex;
        }

        // For when the manual sync hotkey is pressed (well, released)
        private void manualSyncHotkey_KeyUp(object sender, KeyEventArgs e)
        {
            // Sync OMSI time with actual time, if possible
            syncOmsiTime();

            // Play sound to indicate that the hotkey press was acknowledged by the program, if enabled
            if (AppConfig.manualSyncHotkeySound)
            {
                SystemSounds.Asterisk.Play();
            }
        }

        // For handling the manual sync hotkey sound setting
        private void chkManualSyncHotkeySound_CheckedChanged(object sender, EventArgs e)
        {
            if (chkManualSyncHotkeySound.Checked)
            {
                chkManualSyncHotkeySound.BackgroundImage = Properties.Resources.volume;
            }
            else
            {
                chkManualSyncHotkeySound.BackgroundImage = Properties.Resources.volume_mute;
            }

            AppConfig.manualSyncHotkeySound = chkManualSyncHotkeySound.Checked;
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

            // Get a list of potential hotkeys to choose from for the manual sync hotkey
            cmbManualSyncHotkey.DataSource = Enum.GetValues(typeof(Keys));

            // Setup the dropdown lists so they are configured based on the app's config
            cmbOffsetHours.SelectedIndex = AppConfig.offsetHourIndex;
            cmbManualSyncHotkey.SelectedIndex = AppConfig.manualSyncHotkeyIndex;
            cmbAutoSyncMode.SelectedIndex = AppConfig.autoSyncModeIndex;

            // Same with checkboxes
            chkAlwaysOnTop.Checked = AppConfig.alwaysOnTop;
            chkAutoSyncOmsiTime.Checked = AppConfig.autoSyncOmsiTime;
            chkOnlyResyncOmsiTimeIfBehindActualTime.Checked = AppConfig.onlyResyncOmsiTimeIfBehindActualTime;
            chkManualSyncHotkeySound.Checked = AppConfig.manualSyncHotkeySound;
            chkAutoDetectOffsetTime.Checked = AppConfig.autoDetectOffsetHours;

            // Add 'key released' event for manual sync hotkey
            gkhManualSyncHotkey.KeyUp += new KeyEventHandler(manualSyncHotkey_KeyUp);

            // Show manual sync hotkey dropdown menu
            cmbManualSyncHotkey.Visible = true;

            // If OmsiTimeSync.cfg doesn't exist
            if (!File.Exists("OmsiTimeSync.cfg"))
            {
                // Show initial message box dialog (yes/no)
                if (MessageBox.Show(
                    "Thanks for downloading and running OMSI Time Sync.\n" +
                    "\n" +
                    "It's important that you close any games that have anti-cheat protection before pressing 'Yes'! This program performs memory editing which might be falsely flagged as a hack.\n" +
                    "\n" +
                    "This notice will not be shown again unless the 'OmsiTimeSync.cfg' file is deleted in OMSI's plugin directory. The author of this program will not be liable.\n" +
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
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("hour", "0x0046176C"));     // int (h)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("minute", "0x0046176D"));   // int (m)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("second", "0x00461770"));   // float (second.millisecond)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("year", "0x00461790"));     // int (yyyy)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("month", "0x0046178C"));    // int (m)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("day", "0x00461778"));      // int (d)

            // v2.2.032
            // Date/Time
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("hour", "0x00461768"));     // int (h)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("minute", "0x00461769"));   // int (m)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("second", "0x0046176C"));   // float (second.millisecond)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("year", "0x0046178C"));     // int (yyyy)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("month", "0x00461788"));    // int (m)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("day", "0x00461774"));      // int (d)

            // Enable the timer which does various stuff
            tmrOMSI.Enabled = true;
        }

        // Form closing event
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Set current window position in app's config
            AppConfig.windowPositionTop = Top;
            AppConfig.windowPositionLeft = Left;

            // If the timer is enabled
            if (tmrOMSI.Enabled)
            {
                // Save app config
                saveConfig();
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
