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
                    OmsiTelemetry.cabinAirTemp = value;
                    break;

                case 4:
                    OmsiTelemetry.cabinAirRelHum = value;
                    break;

                case 5:
                    OmsiTelemetry.cabinAirAbsHum = value;
                    break;
            }
        }

        // Not used at the moment
        [DllExport(CallingConvention.StdCall)]
        public static unsafe void AccessStringVariable(byte variableIndex, char* firstCharacterAddress, ref bool writeValue)
        {
            
        }

        // Not used at the moment
        [DllExport(CallingConvention.StdCall)]
        public static void AccessSystemVariable(byte variableIndex, ref float value, ref bool writeValue)
        {

        }
    }

    // OMSI's telemetry
    static class OmsiTelemetry
    {
        public static bool omsiClosing = false;

        public static float busSpeedKph = 0.0f;
        public static float scheduleActive = 0.0f;
        public static float humansCount = 0.0f;
        public static float cabinAirTemp = 0.0f;
        public static float cabinAirRelHum = 0.0f;
        public static float cabinAirAbsHum = 0.0f;
    }
}
