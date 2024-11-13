using System.Diagnostics;
using System.Text;

namespace FRSP6498;
public static class DataUtil
{
    public static readonly string DATA_PATH = FileSystem.AppDataDirectory + "\\data\\";
    public static void WriteData(string configName, Dictionary<string, string> data){
        ValidateDataPath();
        ValidateDataFile(configName, data);
        StringBuilder builder = new();
        //iterate from 0 to the end of all of the arrays (assume that all arrays are the same size)
        Debug.WriteLine("Writing to dataFile");
        foreach (var item in data.Values)
        {
            builder.Append(item + ',');
        }
        builder.Remove(builder.Length - 1, 1);
        builder.Append('\n');
        File.AppendAllText(DATA_PATH + configName + ".csv", builder.ToString());
    }
    public static void WriteCsvHeader(string fileName, Dictionary<string, string> data){
        ValidateDataPath();
        StringBuilder header = new();
        Debug.WriteLine("Writing csv header");
        header.Append("TeamNum,MatchNum,");
        foreach (var keyValuePair in data)
        {
            header.Append(keyValuePair.Key + ',');
        }
        header.Remove(header.Length - 1, 1);
        header.Append('\n');

        File.WriteAllText(DATA_PATH + fileName + ".csv", header.ToString());
    }
    public static string GetHeaderFromDataDict(Dictionary<string, string> data)
    {
        var header = new StringBuilder();
        foreach (var keyValuePair in data)
        {
            header.Append(keyValuePair.Key);
        }
        return header.ToString();
    }
    public static Dictionary<string, string> ConvertSettingsToDataDict(List<UISettings> settings){
        Dictionary<string, string> data = [];
        foreach (var item in settings)
        {
            data.Add(item.Name, string.Empty);
        }
        return data;
    }
    public static void ValidateDataPath() {
        if (!Directory.Exists(DATA_PATH))
        {
            Directory.CreateDirectory(DATA_PATH);
        }
    }
    public static void ValidateDataFile(string fileName, Dictionary<string, string> data)
    {
        var fullPath = $"{DATA_PATH}/{fileName}.csv";
        if (!File.Exists(fullPath))
        {
            WriteCsvHeader(fileName, data);
        }
        var entries = File.ReadAllLines(fullPath);
        if (entries.Length > 0 && !entries[0].Equals(GetHeaderFromDataDict(data)))
        {
            entries[0] = GetHeaderFromDataDict(data);
            File.WriteAllLines(DATA_PATH + fileName, entries);
        }
        else
        {
            WriteCsvHeader(fileName, data);
        }
    }

}
