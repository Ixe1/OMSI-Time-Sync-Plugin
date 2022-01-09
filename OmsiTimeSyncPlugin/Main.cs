using net.r_eg.DllExport;
using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OmsiTimeSyncPlugin
{
    public class Main
    {
        private static Form frmMain;
        public static string[] tmp = new string[35];

        // OMSI plugin starting
        [DllExport(CallingConvention.StdCall)]
        public static void PluginStart(ref UIntPtr aOwner)
        {
            Application.SetCompatibleTextRenderingDefault(true);
            Application.EnableVisualStyles();

            frmMain = new frmMain();

            new System.Threading.Thread(
                    delegate ()
                    {
                        System.Threading.Thread.Sleep(4000);

                        Application.Run(frmMain);
                    }
                ).Start();
        }

        // OMSI plugin ending
        [DllExport(CallingConvention.StdCall)]
        public static void PluginFinalize()
        {
            frmMain.Close();

            OmsiTelemetry.omsiClosing = true;
        }

        // Not used at the moment
        [DllExport(CallingConvention.StdCall)]
        public static void AccessTrigger(byte variableIndex, ref bool triggerScript)
        {
            triggerScript = true;
        }

        //public static float[] vars = new float[12];

        // OMSI variables
        [DllExport(CallingConvention.StdCall)]
        public static void AccessVariable(byte variableIndex, ref float value, ref bool writeValue)
        {
            writeValue = false;

            switch (variableIndex)
            {
                case 0:
                    // Velocity (limited from 0 to 1000)
                    OmsiTelemetry.busSpeedKph = Math.Max(0.0f, Math.Min(value, 1000.0f));

                    // Fixes an issue where even though the bus is stationary the velocity variable
                    // often reports a random value that's usually somewhere between 1.0 to 1.2
                    if (OmsiTelemetry.busSpeedKph < 1.5f) { OmsiTelemetry.busSpeedKph = 0.0f; }
                    break;

                case 1:
                    // scheduleActive (either 0 or 1)
                    OmsiTelemetry.scheduleActive = value;
                    break;

                case 2:
                    OmsiTelemetry.humansCount = value;
                    break;

                case 3:
                    OmsiTelemetry.cabinAirTemp = value;
                    break;

                case 4:
                    OmsiTelemetry.cabinAirRelHum = value;
                    break;

                case 5:
                    OmsiTelemetry.cabinAirAbsHum = value;
                    break;
            }

            //vars[variableIndex] = value;
        }

        //public static string[] stringVars = new string[15];

        // Not used at the moment
        [DllExport(CallingConvention.StdCall)]
        public static unsafe void AccessStringVariable(byte variableIndex, char* firstCharacterAddress, ref bool writeValue)
        {
            writeValue = false;

            switch (variableIndex)
            {
                // IBIS Minute(s)
                case 0:
                    OmsiTelemetry.ibisDelayMinutes = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // IBIS Second(s)
                case 1:
                    OmsiTelemetry.ibisDelaySeconds = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // IBIS State (+ or -)
                case 2:
                    OmsiTelemetry.ibisDelayState = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 3:
                    OmsiTelemetry.ibis = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 4:
                    OmsiTelemetry.almexSVersp = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 5:
                    OmsiTelemetry.atronDAktDelayOmsi2 = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 6:
                    OmsiTelemetry.optimaDelay = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 7:
                    OmsiTelemetry.eurofareDelay = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 8:
                    OmsiTelemetry.iboxDelay = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 9:
                    OmsiTelemetry.faremasterDelayMinutes = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 10:
                    OmsiTelemetry.faremasterDelaySeconds = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;

                // 
                case 11:
                    OmsiTelemetry.faremasterDelayState = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
                    break;
            }

            //stringVars[variableIndex] = Marshal.PtrToStringUni((IntPtr)firstCharacterAddress);
        }

        //public static float[] sysVars = new float[2];

        // OMSI system variables
        [DllExport(CallingConvention.StdCall)]
        public static void AccessSystemVariable(byte variableIndex, ref float value, ref bool writeValue)
        {
            writeValue = false;

            switch (variableIndex)
            {
                case 0:
                    // Pause

                    // Get current value
                    OmsiTelemetry.isPaused = value;

                    // If the plugin wants to pause or resume OMSI
                    if (OmsiTelemetry.setIsPaused != -1.0f)
                    {
                        // Set the state in OMSI accordingly
                        value = OmsiTelemetry.setIsPaused;
                        writeValue = true;

                        // Reset pending action/change so it doesn't get repeatedly applied
                        OmsiTelemetry.setIsPaused = -1.0f;
                    }
                    break;
            }

            //sysVars[variableIndex] = value;
        }
    }

    // OMSI's telemetry
    static class OmsiTelemetry
    {
        public static bool omsiClosing = false;

        // Varlist
        public static float busSpeedKph = 0.0f;             // 0
        public static float scheduleActive = 0.0f;          // 1
        public static float humansCount = 0.0f;             // 2
        public static float cabinAirTemp = 0.0f;            // 3
        public static float cabinAirRelHum = 0.0f;          // 4
        public static float cabinAirAbsHum = 0.0f;          // 5

        // Systemvarlist
        public static float isPaused = 0.0f;                // 0

        // Stringvarlist
        // Group
        public static string ibisDelayMinutes = "";         // 0
        public static string ibisDelaySeconds = "";         // 1
        public static string ibisDelayState = "";           // 2
        public static string ibis = "";                     // 3
        // Group
        public static string almexSVersp = "";              // 4
        // Group
        public static string atronDAktDelayOmsi2 = "";      // 5
        // Group
        public static string optimaDelay = "";              // 6
        // Group
        public static string eurofareDelay = "";            // 7
        // Group
        public static string iboxDelay = "";                // 8
        // Group
        public static string faremasterDelayMinutes = "";   // 9
        public static string faremasterDelaySeconds = "";   // 10
        public static string faremasterDelayState = "";     // 11

        // Pending actions/changes
        // Any value other than -1.0f indicates a change is needed at the next opportunity
        public static float setIsPaused = -1.0f;

        public static string getDelay()
        {
            float currentDelay = 0.0f;

            try
            {
                if (almexSVersp.Trim() != "")
                {
                    if (float.TryParse(almexSVersp.Replace(":", "."), out currentDelay))
                    {
                        return currentDelay.ToString("0.0");
                    }
                }

                if (atronDAktDelayOmsi2.Trim() != "")
                {
                    if (float.TryParse(atronDAktDelayOmsi2.Replace(":", "."), out currentDelay))
                    {
                        return currentDelay.ToString("0.0");
                    }
                }

                if (optimaDelay.Trim() != "")
                {
                    if (float.TryParse(optimaDelay.Replace(":", "."), out currentDelay))
                    {
                        return currentDelay.ToString("0.0");
                    }
                }

                if (eurofareDelay.Trim() != "")
                {
                    if (float.TryParse(eurofareDelay.Replace(":", "."), out currentDelay))
                    {
                        return currentDelay.ToString("0.0");
                    }
                }

                if (iboxDelay.Trim() != "")
                {
                    if (float.TryParse(iboxDelay.Replace(":", "."), out currentDelay))
                    {
                        return currentDelay.ToString("0.0");
                    }
                }

                if (faremasterDelayMinutes.Trim() != "")
                {
                    if (float.TryParse(faremasterDelayMinutes + "." + faremasterDelaySeconds, out currentDelay))
                    {
                        return (faremasterDelayState == "-" ? -currentDelay : currentDelay).ToString("0.0");
                    }
                }

                if (ibisDelayMinutes.Trim() != "")
                {
                    if (ibisDelaySeconds.Trim() == "") ibisDelaySeconds = "0";

                    try
                    {
                        if (ibis.Contains("@") == false || ibis.Split('@')[1].Substring(3, 1) != "/")
                        {
                            if (float.TryParse(ibisDelayMinutes + "." + ibisDelaySeconds, out currentDelay))
                            {
                                return (ibisDelayState == "-" ? -currentDelay : currentDelay).ToString("0.0");
                            }
                        }
                    }
                    catch { }

                    if (float.TryParse(ibisDelayMinutes + "." + ibisDelaySeconds, out currentDelay))
                    {
                        return (ibisDelayState == "-" ? currentDelay : -currentDelay).ToString("0.0");
                    }
                }
            }
            catch { }

            return "-";
        }
    }
}
