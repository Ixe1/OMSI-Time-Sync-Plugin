using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class BusStopsCfgReader
{
    private static List<string> busStopsData;

    public static bool readBusStopsCfg(string fileLocation)
    {
        try
        {
            busStopsData = new List<string>();

            TextReader reader = new StreamReader(fileLocation);

            string fileContents = reader.ReadToEnd();
            reader.Close();

            string[] busStops = fileContents.Split(new string[] { "[busstop]" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string busStop in busStops)
            {
                if (busStopsData.FindIndex(s => s == busStop) == -1)
                {
                    busStopsData.Add(busStop);
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

                    if (busStopInfo[2].Trim() == busStopID.ToString())
                    {
                        return busStopInfo[0];
                    }
                }
                catch { }
            }
        }

        return null;
    }
}
