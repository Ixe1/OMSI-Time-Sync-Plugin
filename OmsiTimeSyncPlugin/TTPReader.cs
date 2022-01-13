using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public static class TTPReader
{
    private static List<string> busStopsData;

    public static bool readTTPFiles(string folderLocation)
    {
        try
        {
            busStopsData = new List<string>();

            DirectoryInfo ttDataFolder = new DirectoryInfo(folderLocation);
            FileInfo[] ttDataFiles = ttDataFolder.GetFiles("*.ttp", SearchOption.TopDirectoryOnly);

            foreach (FileInfo ttDataFile in ttDataFiles)
            {
                TextReader reader = new StreamReader(ttDataFile.FullName);

                string fileContents = reader.ReadToEnd();
                reader.Close();

                string[] busStops = fileContents.Split(new string[] { "[station]" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string busStop in busStops)
                {
                    if (busStopsData.FindIndex(s => s == busStop) == -1)
                    {
                        busStopsData.Add(busStop);
                    }
                }
            }
        }
        catch { }

        return false;
    }

    public static bool isBusStopsLoaded()
    {
        return busStopsData != null && busStopsData.Count() > 0;
    }

    public static string findBusStop(int busStopID)
    {
        if (busStopsData != null && busStopsData.Count() > 0)
        {
            foreach (string busStop in busStopsData)
            {
                try
                {
                    string[] busStopInfo = busStop.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    if (busStopInfo[0].Trim() == busStopID.ToString())
                    {
                        return busStopInfo[2];
                    }
                }
                catch { }
            }
        }

        return null;
    }
}