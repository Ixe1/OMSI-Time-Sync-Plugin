using net.r_eg.DllExport;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmsiTimeSyncPlugin
{
    public class Main
    {
        // OMSI plugin starting
        [DllExport(CallingConvention.StdCall)]
        public static void PluginStart(ref UIntPtr aOwner)
        {
            //Task.Factory.StartNew(() => RunServer());

            Form frmMain = new frmMain();

            frmMain.Show();
        }

        // OMSI plugin ending
        [DllExport(CallingConvention.StdCall)]
        public static void PluginFinalize()
        {
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
        static void RunServer()
        {
            // While loop if OMSI isn't closing
            while (!OmsiTelemetry.omsiClosing)
            {
                try
                {
                    // Setup a new named pipe server for communication between this plugin and the OMSI Time Sync program
                    using (var pipeServer = new NamedPipeServerStream("OmsiTimeSyncTelemetryPlugin", PipeDirection.InOut))
                    {
                        // Setup a stream reader and writer for receiving and sending communication
                        using (var reader = new StreamReader(pipeServer))
                        {
                            using (var writer = new StreamWriter(pipeServer))
                            {
                                // If named pipe server has no connection then
                                if (!pipeServer.IsConnected)
                                {
                                    // Wait here for a connection, don't execute further code yet
                                    pipeServer.WaitForConnection();
                                }

                                // While loop again for this section of code
                                while (!OmsiTelemetry.omsiClosing)
                                {
                                    // Read message sent from the OMSI Time Sync program
                                    var message = reader.ReadLine();

                                    switch (message)
                                    {
                                        // If the message is 'telemetry'
                                        case "telemetry":
                                            // Send telemetry
                                            writer.WriteLine(
                                                OmsiTelemetry.busSpeedKph + "*" +
                                                OmsiTelemetry.scheduleActive
                                                ); ;
                                            writer.Flush();
                                            break;
                                    }

                                    pipeServer.WaitForPipeDrain();
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }
    }

    // OMSI's telemetry
    static class OmsiTelemetry
    {
        public static bool omsiClosing = false;

        public static float busSpeedKph = 0.0f;
        public static float scheduleActive = 0.0f;
    }
}
