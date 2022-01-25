using System;
using System.IO;
using System.Text;
public static class Log
{
    public static void Reset()
    {
        try
        {
            using (var fs = new FileStream("OmsiTimeSyncLog.txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.Write("");

                sw.Close();
                fs.Close();
            }
        }
        catch { return; }
    }

    public static void Save(string type, string message, int loggingLevelTypeThreshold)
    {
        try
        {
            if (loggingLevelTypeThreshold == 0) return;

            switch (type.ToUpper())
            {
                case "DEBUG":
                    if (loggingLevelTypeThreshold < 4) return;
                    break;

                case "INFO":
                    if (loggingLevelTypeThreshold < 3) return;
                    break;

                case "WARN":
                    if (loggingLevelTypeThreshold < 2) return;
                    break;

                case "ERROR":
                    if (loggingLevelTypeThreshold < 1) return;
                    break;

                default:
                    return;
            }

            using (var fs = new FileStream("OmsiTimeSyncLog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + type.ToUpper().PadRight(10) + ": " + message);

                sw.Close();
                fs.Close();
            }
        }
        catch { return; }
    }
}
