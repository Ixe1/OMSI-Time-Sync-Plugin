using net.r_eg.DllExport;
using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OmsiTimeSyncPlugin
{
    public class Main
    {
        [DllExport(CallingConvention.StdCall)]
        public static void PluginStart(ref UIntPtr aOwner)
        {
            Task.Factory.StartNew(() => RunServer());
        }

        [DllExport(CallingConvention.StdCall)]
        public static void PluginFinalize()
        {
            OmsiTelemetry.omsiClosing = true;
        }

        [DllExport(CallingConvention.StdCall)]
        public static void AccessTrigger(byte variableIndex, ref bool triggerScript)
        {
            
        }

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

        [DllExport(CallingConvention.StdCall)]
        public static unsafe void AccessStringVariable(byte variableIndex, char* firstCharacterAddress, ref bool writeValue)
        {
            
        }

        [DllExport(CallingConvention.StdCall)]
        public static void AccessSystemVariable(byte variableIndex, ref float value, ref bool writeValue)
        {

        }
        static void RunServer()
        {
            while (!OmsiTelemetry.omsiClosing)
            {
                try
                {
                    using (var pipeServer = new NamedPipeServerStream("OmsiTimeSyncTelemetryPlugin", PipeDirection.InOut))
                    {
                        using (var reader = new StreamReader(pipeServer))
                        {
                            using (var writer = new StreamWriter(pipeServer))
                            {
                                if (!pipeServer.IsConnected)
                                {
                                    pipeServer.WaitForConnection();
                                }

                                while (!OmsiTelemetry.omsiClosing)
                                {
                                    var message = reader.ReadLine();

                                    switch (message)
                                    {
                                        case "telemetry":
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

    static class OmsiTelemetry
    {
        public static bool omsiClosing = false;

        public static float busSpeedKph = 0.0f;
        public static float scheduleActive = 0.0f;
    }
}
