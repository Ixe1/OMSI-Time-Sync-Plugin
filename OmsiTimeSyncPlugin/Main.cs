using net.r_eg.DllExport;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OmsiTimeSyncPlugin
{
    public class Main
    {
        private static Form frmMain;

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
                        System.Threading.Thread.Sleep(1000);

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

        // Not currently used
        [DllExport(CallingConvention.StdCall)]
        public static void AccessTrigger(byte variableIndex, ref bool triggerScript)
        {
            
        }

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
                    OmsiTelemetry.busStoppingLight = value;
                    break;
            }
        }

        // Not currently used
        [DllExport(CallingConvention.StdCall)]
        public static unsafe void AccessStringVariable(byte variableIndex, char* firstCharacterAddress, ref bool writeValue)
        {
            
        }


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
        }
    }

    // OMSI's telemetry
    static class OmsiTelemetry
    {
        private static DateTime paxExitLastReq = DateTime.MinValue;

        public static bool omsiClosing = false;

        // Varlist
        public static float busSpeedKph = 0.0f;             // 0
        public static float scheduleActive = 0.0f;          // 1
        public static float humansCount = 0.0f;             // 2
        public static float busStoppingLight = 0.0f;        // 3

        // Systemvarlist
        public static float isPaused = 0.0f;                // 0

        // Pending actions/changes
        // Any value other than -1.0f indicates a change is needed at the next opportunity
        public static float setIsPaused = -1.0f;
        public static bool isBusStopRequested()
        {
            return (
                busStoppingLight != 0.0f
                );
        }
    }
}
