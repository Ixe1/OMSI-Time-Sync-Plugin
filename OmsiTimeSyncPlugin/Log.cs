using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public static class Log
{
    public static void Reset()
    {
        try
        {
            using (var fs = new FileStream("OmsiTimeSyncLog.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.Write("");

                sw.Flush();

                sw.Close();
                fs.Close();
            }
        }
        catch { return; }
    }

    public static void Save(string type, string message)
    {
        try
        {
            using (var fs = new FileStream("OmsiTimeSyncLog.txt", FileMode.Append, FileAccess.ReadWrite, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + type.ToUpper().PadRight(12) + ": " + message);

                sw.Flush();

                sw.Close();
                fs.Close();
            }
        }
        catch { return; }
    }
}
