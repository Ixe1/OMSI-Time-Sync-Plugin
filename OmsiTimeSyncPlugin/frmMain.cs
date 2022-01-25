using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
                // Force a drop shadow of the UI
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

                        Log.Save("DEBUG", "OMSI 2's MD5 hash identified as " + md5Hash, AppConfig.loggingLevel);

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
                    Log.Save("INFO", "Reading OMSI time from memory...", AppConfig.loggingLevel);

                    omsiTimeDateBytes = m.ReadBytes(Omsi.getMemoryAddress(omsiVersion, "datetime"), 40);

                    Log.Save("DEBUG", "Read 40 bytes successfully", AppConfig.loggingLevel);

                    int i = 0;
                    string dateStr = "";
                    // H:m:s.f[ffffff] d/M/YYYY
                    // H:m:s,f[ffffff] d/M/YYYY
                    dateStr = dateStr + (int)omsiTimeDateBytes[i] + ":"; i++;                       // Hour
                    dateStr = dateStr + (int)omsiTimeDateBytes[i] + ":"; i += 3;                    // Minute
                    dateStr = dateStr + BitConverter.ToSingle(omsiTimeDateBytes, i) + " "; i += 8;  // Second.Millisecond
                    dateStr = dateStr + BitConverter.ToInt32(omsiTimeDateBytes, i) + "/"; i += 20;  // Day
                    dateStr = dateStr + BitConverter.ToInt32(omsiTimeDateBytes, i) + "/"; i += 4;   // Month
                    dateStr = dateStr + BitConverter.ToInt32(omsiTimeDateBytes, i);                 // Year

                    Log.Save("INFO", "OMSI date and time read as " + dateStr, AppConfig.loggingLevel);

                    string[] acceptableDateTimeFormats = new string[] {
                        "H:m:s.f d/M/yyyy",
                        "H:m:s.ff d/M/yyyy",
                        "H:m:s.fff d/M/yyyy",
                        "H:m:s.ffff d/M/yyyy",
                        "H:m:s.fffff d/M/yyyy",
                        "H:m:s.ffffff d/M/yyyy",
                        "H:m:s.fffffff d/M/yyyy",
                        "H:m:s,f d/M/yyyy",
                        "H:m:s,ff d/M/yyyy",
                        "H:m:s,fff d/M/yyyy",
                        "H:m:s,ffff d/M/yyyy",
                        "H:m:s,fffff d/M/yyyy",
                        "H:m:s,ffffff d/M/yyyy",
                        "H:m:s,fffffff d/M/yyyy"
                    };

                    return DateTime.TryParseExact(dateStr, acceptableDateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out omsiTime);
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

                    Log.Save("DEBUG",
                        "AppConfig.onlyResyncOmsiTimeIfBehindActualTime = " + AppConfig.onlyResyncOmsiTimeIfBehindActualTime.ToString() + ", " +
                        "timeDifference = " + timeDifference.ToString() + ", " +
                        "AppConfig.autoSyncModeIndex = " + AppConfig.autoSyncModeIndex.ToString(),
                        AppConfig.loggingLevel
                        );

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

                            Log.Save("INFO", "Setting new OMSI time to " + newSystemTime.ToString("dd/MM/yyyy HH:mm:ss"), AppConfig.loggingLevel);

                            // 09 39 00 00 BB 2F 15 41 09 00 00 00 09 00 00 00 00 00 00 00 94 A5 00 24 00 00 00 00 00 00 00 00 01 00 00 00 E6 07 00 00
                            // Hour, Minute, Second, Day, Month, Year
                            // Byte, Byte,   Float,  Int, Int,   Int
                            // The following should be compatible
                            int i = 0;
                            byte[] writeBytes = omsiTimeDateBytes;
                            writeBytes[i] = Convert.ToByte(newSystemTime.Hour); i++; // 0 - 1
                            writeBytes[i] = Convert.ToByte(newSystemTime.Minute); i += 3; // 1 - 4
                            BitConverter.GetBytes(Convert.ToSingle(newSystemTime.Second + "." + newSystemTime.Millisecond, CultureInfo.GetCultureInfo("en-GB"))).CopyTo(writeBytes, i); i += 8; // 4 - 12
                            BitConverter.GetBytes(newSystemTime.Day).CopyTo(writeBytes, i); i += 20; // 12 - 32
                            BitConverter.GetBytes(newSystemTime.Month).CopyTo(writeBytes, i); i += 4; // 32 - 36
                            BitConverter.GetBytes(newSystemTime.Year).CopyTo(writeBytes, i); // 35 - 39

                            // Set the new date and time
                            bool success = m.WriteMemory(Omsi.getMemoryAddress(omsiVersion, "datetime"), "bytes", writeBytes);

                            if (success)
                                Log.Save("INFO", "Successfully set new OMSI time", AppConfig.loggingLevel);
                            else
                                Log.Save("ERROR", "Failed to set new OMSI time", AppConfig.loggingLevel);
                        }
                    }

                    // Get the latest date and time in OMSI again
                    return getOmsiTime();
                }
            }
            catch { Log.Save("ERROR", "Caught exception in syncOmsiTime()", AppConfig.loggingLevel); }

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
                _ = Math.Max(-23, Math.Min(23, Convert.ToInt32(txtRdr.ReadLine())));                        // Formely offsetHour
                _ = Convert.ToInt32(txtRdr.ReadLine());                                                     // Formely offsetHourIndex
                AppConfig.windowPositionLeft = Convert.ToInt32(txtRdr.ReadLine());
                AppConfig.windowPositionTop = Convert.ToInt32(txtRdr.ReadLine());
                _ = Convert.ToInt32(txtRdr.ReadLine());                                                     // Formely manualSyncHotkeyIndex
                AppConfig.autoSyncModeIndex = Convert.ToInt32(txtRdr.ReadLine());
                _ = Convert.ToBoolean(txtRdr.ReadLine());                                                   // Formely manualSyncHotkeySound
                _ = Convert.ToBoolean(txtRdr.ReadLine());                                                   // Formely autoDetectOffsetHours
                AppConfig.loggingLevel = Convert.ToInt32(txtRdr.ReadLine());

                Log.Save("INFO", "Configuration loaded", AppConfig.loggingLevel);

                return true;
            }
            catch
            {
                // If something goes wrong then use the default settings
                AppConfig.alwaysOnTop = AppConfigDefaults.alwaysOnTop;
                AppConfig.autoSyncOmsiTime = AppConfigDefaults.autoSyncOmsiTime;
                AppConfig.onlyResyncOmsiTimeIfBehindActualTime = AppConfigDefaults.onlyResyncOmsiTimeIfBehindActualTime;
                AppConfig.windowPositionLeft = AppConfigDefaults.windowPositionLeft;
                AppConfig.windowPositionTop = AppConfigDefaults.windowPositionTop;
                AppConfig.autoSyncModeIndex = AppConfigDefaults.autoSyncModeIndex;
                AppConfig.loggingLevel = AppConfigDefaults.loggingLevel;

                Log.Save("ERROR", "Configuration could not be loaded", AppConfig.loggingLevel);

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
                txtWtr.WriteLine(0);                                                                        // Formely offsetHour
                txtWtr.WriteLine(0);                                                                        // Formely offsetHourIndex
                txtWtr.WriteLine(AppConfig.windowPositionLeft.ToString());
                txtWtr.WriteLine(AppConfig.windowPositionTop.ToString());
                txtWtr.WriteLine(0);                                                                        // Formely manualSyncHotkeyIndex
                txtWtr.WriteLine(AppConfig.autoSyncModeIndex.ToString());
                txtWtr.WriteLine(false);                                                                    // Formely manualSyncHotkeySound
                txtWtr.WriteLine(true);                                                                     // Formely autoDetectOffsetHours
                txtWtr.WriteLine(AppConfig.loggingLevel.ToString());

                txtWtr.Close();

                Log.Save("INFO", "Configuration saved", AppConfig.loggingLevel);

                return true;
            }
            catch { Log.Save("ERROR", "Configuration could not be saved", AppConfig.loggingLevel); return false; }
        }

        // Background timer that runs every 60 seconds which auto saves the app's config
        private void tmrAutoSave_Tick(object state)
        {
            Log.Save("INFO", "Auto save requested", AppConfig.loggingLevel);

            saveConfig();
        }

        // Background timer that runs every 1 second in regards to OMSI stuff
        private void tmrBackground_Tick(object state)
        {
            // If process isn't already attached
            if (!processAttached)
            {
                Log.Save("INFO", "OMSI process not currently attached, attempting to find OMSI process", AppConfig.loggingLevel);

                // Search for Omsi.exe process
                int processID = Process.GetCurrentProcess().Id;

                // If a process was found
                if (processID > 0)
                {
                    Log.Save("INFO", "OMSI process found (PID " + processID + "), attaching", AppConfig.loggingLevel);

                    // Attach to the process
                    processAttached = m.OpenProcess(processID);
                }
            }

            // Auto detect hours offset
            if (processAttached &&
                omsiLoaded &&
                Omsi.isVersionSupported(omsiVersion) &&
                systemTime == DateTime.MinValue)
            {
                Log.Save("INFO", "OMSI process attached, supported and loaded. Detecting time difference...", AppConfig.loggingLevel);

                systemTime = DateTime.Now;

                hoursDifference = Math.Round((omsiTime - systemTime).TotalHours);

                systemTime = systemTime.AddHours(hoursDifference);

                Log.Save("INFO", "Time difference detected as " + hoursDifference + " hour(s)", AppConfig.loggingLevel);
            }
            else if (!processAttached ||
                !omsiLoaded ||
                !Omsi.isVersionSupported(omsiVersion))
            {
                systemTime = DateTime.MinValue;

                lblSystemTime.Text = "Waiting for OMSI map...";

                Log.Save("WARN", "Process is either not attached, not loaded into a map or unsupported version:", AppConfig.loggingLevel);
                Log.Save("DEBUG",
                    "processAttached = " + processAttached.ToString() + ", " +
                    "omsiLoaded = " + omsiLoaded.ToString() + ", " +
                    "Omsi.isVersionSupported(" + omsiVersion + ") = " + Omsi.isVersionSupported(omsiVersion),
                    AppConfig.loggingLevel
                    );
            }

            if (systemTime != DateTime.MinValue)
            {
                systemTime = DateTime.Now.AddHours(hoursDifference);

                // Display the actual time in the UI
                lblSystemTime.Text = systemTime.ToString();

                Log.Save("INFO", "Set actual time to " + systemTime.ToString() + " based on " + hoursDifference + " hour(s) difference", AppConfig.loggingLevel);
            }

            // If a process is attached
            if (processAttached)
            {
                Log.Save("INFO", "Process attached", AppConfig.loggingLevel);

                // If OMSI version is currently unknown then try to identify what the version is
                if (omsiVersion == "Unknown")
                {
                    omsiVersion = getOmsiVersion();

                    Log.Save("INFO", "OMSI version currently unknown, getOmsiVersion() returned " + omsiVersion, AppConfig.loggingLevel);
                }
                
                // If the OMSI version is still unknown then we assume OMSI is still loading
                // Further code execution stops here until the version can be identified
                if (omsiVersion == "Unknown")
                {
                    Log.Save("ERROR", "OMSI version remains unknown or unsupported", AppConfig.loggingLevel);

                    resetFormTitleBarValues();

                    return;
                }

                // Check if OMSI version is supported, if it's not then stop further code execution and explain in the UI
                if (!Omsi.isVersionSupported(omsiVersion))
                {
                    Log.Save("ERROR", "OMSI version " + omsiVersion + " is unsupported", AppConfig.loggingLevel);

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
                    Log.Save("INFO", "Checking logfile.txt for certain entries...", AppConfig.loggingLevel);

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

                Log.Save("DEBUG", "omsiLoaded = " + omsiLoaded.ToString(), AppConfig.loggingLevel);

                if (omsiLoaded)
                {
                    omsiLoaded = getOmsiTime();
                }

                // If OMSI isn't loaded into a map
                if (!omsiLoaded)
                {
                    Log.Save("INFO", "OMSI is not currently loaded into a map", AppConfig.loggingLevel);

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
                        Log.Save("INFO", "OMSI is loaded into a map, identifying map name...", AppConfig.loggingLevel);

                        int firstValue = m.ReadInt(Omsi.getMemoryAddress(omsiVersion, "ptr1_to_map_path"), true);
                        int secondValue = m.ReadInt(firstValue + Omsi.getMemoryAddress(omsiVersion, "ptr2_to_map_path"), false);
                        byte[] thirdValue = m.ReadBytes(secondValue + 0x0, 64, false, true);

                        omsiMap = Encoding.UTF8.GetString(thirdValue).Replace('\\', '/').ToLower().Replace("global.cfg", "");

                        Log.Save("INFO", "Map identified as '" + omsiMap + "', verifying map folder exists...", AppConfig.loggingLevel);

                        if (Directory.Exists(omsiMap))
                        {
                            Log.Save("INFO", "Map folder exists!", AppConfig.loggingLevel);

                            // Attempt to get all of the bus stop names for the current map
                            BusStopsCfgReader.readBusStopsCfg(omsiMap + "TTData/BusStops.cfg");
                            TTPReader.readTTPFiles(omsiMap + "TTData/");
                        }
                        else
                        {
                            Log.Save("ERROR", "Map folder doesn't exist!", AppConfig.loggingLevel);
                            omsiMap = "Unknown";
                        }
                    }
                    catch { Log.Save("ERROR", "Caught exception while trying to identify the map!", AppConfig.loggingLevel); omsiMap = "Unknown"; }
                }

                // If auto sync OMSI time is enabled
                if (AppConfig.autoSyncOmsiTime && systemTime != DateTime.MinValue)
                {
                    Log.Save("INFO", "Attempting to auto sync time...", AppConfig.loggingLevel);

                    // Go ahead with syncing OMSI time
                    syncOmsiTime();
                }

                // Show MPH or KMH in the UI's title bar
                if (formTitleBarCurrentOmsiSpeed.Text.EndsWith("MPH"))
                {
                    formTitleBarCurrentOmsiSpeed.Text = Math.Round(OmsiTelemetry.busSpeedKph * 0.6213711922).ToString().PadRight(3) + "MPH";
                }
                else if (formTitleBarCurrentOmsiSpeed.Text.EndsWith("KMH"))
                {
                    formTitleBarCurrentOmsiSpeed.Text = Math.Round(OmsiTelemetry.busSpeedKph).ToString().PadRight(3) + "KMH";
                }

                // Show current OMSI time in UI's title bar
                formTitleBarCurrentOmsiTime.Text = omsiTime.ToString("HH:mm:ss");

                string omsiDelayStr = "-";
                int omsiDelay = 0;

                string omsiNextBusStop = "-";

                // If a timetable schedule is active
                if (OmsiTelemetry.scheduleActive == 1.0f)
                {
                    // Try to get the current delay (in seconds) of how early or late it currently is
                    try
                    {
                        int firstValue = m.ReadInt(Omsi.getMemoryAddress(omsiVersion, "timetable_manager"), true);
                        int secondValue = m.ReadInt(firstValue + Omsi.getMemoryAddress(omsiVersion, "ttm_delay"), false);

                        omsiDelay = secondValue;

                        TimeSpan omsiDelayTime = TimeSpan.FromSeconds(omsiDelay);

                        omsiDelayStr = string.Format("{0:D2}:{1:D2}", (int)Math.Abs(omsiDelayTime.TotalMinutes), Math.Abs(omsiDelayTime.Seconds));
                    }
                    catch { omsiDelayStr = "-"; omsiDelay = 0; }

                    // Try to also get the next bus stop ID and compare with what's in the BusStops.cfg file or TTP files for the current map
                    try
                    {
                        int firstValue = m.ReadInt(Omsi.getMemoryAddress(omsiVersion, "timetable_manager"), true);
                        int secondValue = m.ReadInt(firstValue + Omsi.getMemoryAddress(omsiVersion, "ttm_next_bus_stop_id"), false);

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

                // Set the text colour based on early, late or other/on time on the UI
                if (omsiDelay < 0) formTitleBarCurrentOmsiDelay.ForeColor = Color.FromArgb(192, 255, 192);
                else if (omsiDelay > 0) formTitleBarCurrentOmsiDelay.ForeColor = Color.FromArgb(255, 192, 192);
                else formTitleBarCurrentOmsiDelay.ForeColor = Color.White;

                // Add a - or + displayed 'delay', or just '-' if delay is unknown
                if (omsiDelay < 0 && omsiDelayStr != "-") formTitleBarCurrentOmsiDelay.Text = "-" + omsiDelayStr;
                else if (omsiDelay >= 0 && omsiDelayStr != "-") formTitleBarCurrentOmsiDelay.Text = "+" + omsiDelayStr;
                else formTitleBarCurrentOmsiDelay.Text = "-";

                // Specify next bus stop name in the UI, if known
                formTitleBarNextBusStop.Text = omsiNextBusStop;

                // If bus stop has been requested by someone
                if (OmsiTelemetry.busStoppingLight != 0.0f)
                {
                    // Flash the '- STOP -' text on the title bar on the UI
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
                    // Otherwise keep it 'dark gray' if nobody has
                    formTitleBarBusStopRequest.ForeColor = Color.FromArgb(64, 64, 64);
                }

                // State the current date and time of OMSI in the UI
                lblOmsiTime.Text = omsiTime.ToString();
            }
            else
            {
                Log.Save("WARN", "OMSI is not currently running!", AppConfig.loggingLevel);

                // State that 'OMSI is not running' in the UI
                lblOmsiTime.Text = "OMSI is not running!";
                lblOmsiTime.ForeColor = Color.Red;
            }
        }

        // Reset title bar values to default
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

        // For manually syncing OMSI's time when pressing the button on the UI
        private void btnManualSyncOmsiTime_Click(object sender, EventArgs e)
        {
            // Attempt to sync OMSI's time with the actual time, but if it fails...
            if (!syncOmsiTime())
            {
                // Show an error message stating that it failed for some reason
                MessageBox.Show("ERROR: Unable to sync OMSI time. Please check that a map has been loaded.", "OMSI Time Sync - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // For handling the auto sync mode setting
        private void cmbAutoSyncMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.autoSyncModeIndex = cmbAutoSyncMode.SelectedIndex;
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

            // Clean log file
            Log.Reset();

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
            cmbAutoSyncMode.SelectedIndex = AppConfig.autoSyncModeIndex;
            cmbLoggingLevel.SelectedIndex = AppConfig.loggingLevel;

            // Same with checkboxes
            chkAutoSyncOmsiTime.Checked = AppConfig.autoSyncOmsiTime;
            chkOnlyResyncOmsiTimeIfBehindActualTime.Checked = AppConfig.onlyResyncOmsiTimeIfBehindActualTime;
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

        // For moving the UI window around
        private void frmMain_LocationChanged(object sender, EventArgs e)
        {
            // If the timer is enabled
            if (tmrBackground != null && WindowState != FormWindowState.Minimized)
            {
                // Set current window position in app's config
                AppConfig.windowPositionTop = Top;
                AppConfig.windowPositionLeft = Left;
            }
        }

        // For moving the UI window around
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

        // For moving the UI window around
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

        // For moving the UI window around
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

        // Bring to front and focused as well as applying the 'always on top' state
        private void frmMain_Shown(object sender, EventArgs e)
        {
            Activate();
            BringToFront();
            Focus();

            refreshButtonAlwaysOnTop();

            Log.Save("INFO", "UI shown", AppConfig.loggingLevel);

            // If OmsiTimeSync.cfg doesn't exist
            if (!File.Exists("OmsiTimeSync.cfg"))
            {
                Log.Save("INFO", "OmsiTimeSync.cfg file does not exist, showing first time message...", AppConfig.loggingLevel);

                // Show initial message box dialog (yes/no)
                if (MessageBox.Show(
                    "Thanks for downloading OMSI Time Sync.\n" +
                    "\n" +
                    "It's important that you close any games that have anti-cheat protection before pressing 'Yes'! This plugin performs memory editing which might be falsely flagged as a hack.\n" +
                    "\n" +
                    "This notice will not be shown again unless the 'OmsiTimeSync.cfg' file is deleted in OMSI's directory. The author of this plugin will not be liable.\n" +
                    "\n" +
                    "While this is a free OMSI plugin, a donation is highly appreciated if you like this plugin.\n" +
                    "\n" +
                    "Do you acknowledge the above notice and agree?",
                    "OMSI Time Sync", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    Log.Save("INFO", "First time message was declined, terminating plugin...", AppConfig.loggingLevel);

                    // If 'no' is chosen then close the app
                    this.Close();
                    Application.Exit();

                    // Don't execute further code
                    return;
                }

                Log.Save("INFO", "First time message was accepted, continuing...", AppConfig.loggingLevel);
            }

            Log.Save("INFO", "Initialising Mem()...", AppConfig.loggingLevel);

            // For accessing and writing to OMSI's memory later on
            m = new Mem();

            // Add supported OMSI memory addresses
            Log.Save("INFO", "Adding OMSI 2's recognised memory addresses...", AppConfig.loggingLevel);

            // v2.3.004
            // Date/Time
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("datetime", 0x0046176C));                      // array of bytes
            // Timetable Manager
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("timetable_manager", 0x00461500));             // int (memory address for timetable stuff)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("ttm_next_bus_stop_id", 0x6B0));               // offset - int (next bus stop ID)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("ttm_delay", 0x6BC));                          // offset - int (delay in seconds)
            // Environment ???
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("ptr1_to_map_path", 0x00461588));              // int (memory address pointer leading to map path)
            Omsi.addMemoryAddress("2.3.004", new OmsiAddress("ptr2_to_map_path", 0x154));                   // int (memory address offset leading to map path)

            // v2.2.032
            // Date/Time
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("datetime", 0x00461768));                      // array of bytes
            // Timetable Manager
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("timetable_manager", 0x004614FC));             // int (memory address for timetable stuff)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("ttm_next_bus_stop_id", 0x6B0));               // offset - int (next bus stop ID)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("ttm_delay", 0x6BC));                          // offset - int (delay in seconds)
            // Environment ???
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("ptr1_to_map_path", 0x00461584));              // int (memory address pointer leading to map path)
            Omsi.addMemoryAddress("2.2.032", new OmsiAddress("ptr2_to_map_path", 0x154));                   // int (memory address offset leading to map path)

            Log.Save("INFO", "Setting up autosave timer...", AppConfig.loggingLevel);

            // Setup the autosave timer
            tmrAutoSave = new System.Threading.Timer(new System.Threading.TimerCallback(tmrAutoSave_Tick), null, 60000, 60000);

            Log.Save("INFO", "Setting up background timer...", AppConfig.loggingLevel);

            // Setup the background timer which does various stuff
            tmrBackground = new System.Threading.Timer(new System.Threading.TimerCallback(tmrBackground_Tick), null, 1000, 1000);
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

        // Minimise the UI
        private void formTitleBarMinimise_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        // Expand or compact the UI
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

        // Toggle 'always on top' for the UI
        private void formTitleBarPinUnpin_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AppConfig.alwaysOnTop = !AppConfig.alwaysOnTop;

                refreshButtonAlwaysOnTop();
            }
        }

        // Update state of 'always on top'
        private void refreshButtonAlwaysOnTop()
        {
            TopMost = AppConfig.alwaysOnTop;

            if (AppConfig.alwaysOnTop)
            {
                formTitleBarPinUnpin.BackColor = Color.DarkGreen;
            }
            else
            {
                formTitleBarPinUnpin.BackColor = Color.DarkRed;
            }
        }

        // Toggle between MPH or KPH as shown on the title bar if the text is double clicked
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

        // Quickly preheat the bus (engine temperature, cabin temperature and absolute humidity)
        private void btnPreheatBus_Click(object sender, EventArgs e)
        {
            // Prevent quick preheat during a tour/scheduled route
            if (OmsiTelemetry.scheduleActive == 1.0f)
            {
                MessageBox.Show("Quickly preheating the bus is only possible while there's no active tour/schedule.", "OMSI Time Sync", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set bus engine temperature to 80C
            OmsiTelemetry.setBusEngineTemperature = 80.0f;

            // Set bus cabin temperature to 19C
            OmsiTelemetry.setBusCabinTemperature = 19.0f;

            // Set bus absolute humidity
            OmsiTelemetry.setBusCabinAbsHumidity = 8.0f;
        }

        // Logging level
        private void cmbLoggingLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.loggingLevel = cmbLoggingLevel.SelectedIndex;
        }
    }

    // Struct for OMSI memory addresses
    public struct OmsiAddress
    {
        // For example, hour, month, day, minute, etc.
        public readonly string addressType;
        // For example, base+0x00000000
        public readonly int addressLocation;

        // Constructor
        public OmsiAddress(string addressType, int addressLocation)
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
        public static int getMemoryAddress(string version, string addressType)
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

            return -1;
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
        public static int windowPositionLeft = AppConfigDefaults.windowPositionLeft;
        public static int windowPositionTop = AppConfigDefaults.windowPositionTop;
        public static int autoSyncModeIndex = AppConfigDefaults.autoSyncModeIndex;
        public static int loggingLevel = AppConfigDefaults.loggingLevel;
    }

    // This app's default config
    static class AppConfigDefaults
    {
        public static bool alwaysOnTop = true;
        public static bool autoSyncOmsiTime = true;
        public static bool onlyResyncOmsiTimeIfBehindActualTime = true;
        public static int windowPositionLeft = -1;
        public static int windowPositionTop = -1;
        public static int autoSyncModeIndex = 0;
        public static int loggingLevel = 3; // Info level
    }
}
