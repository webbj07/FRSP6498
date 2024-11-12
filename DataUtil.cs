using System.Text;

namespace FRSP6498;
public static class DataUtil
{
    public static readonly string DATA_PATH = FileSystem.AppDataDirectory + "\\data\\";
    public static void WriteData(string configName, Dictionary<string, List<string>> data){
        ValidateDataPath();
        StringBuilder builder = new();

        if (!File.Exists(DATA_PATH + configName) || File.ReadAllText(DATA_PATH + configName).Length <= 0)
        {
            //writes a new csv file w header
            WriteCsvHeader(DATA_PATH + configName, data);
        }

        //iterate from 0 to the end of all of the arrays (assume that all arrays are the same size)
        for (int i = 0; i < data.First().Value.Count ; i++)
        {
            foreach (var keyValuePair in data)
            {
                builder.Append(keyValuePair.Value[i]+',');

            }
            builder.Append('\n');
        }
        File.WriteAllText(DATA_PATH + configName, builder.ToString());

    }
    public static void WriteCsvHeader(string fileName, Dictionary<string, List<string>> data){
        ValidateDataPath();
        StringBuilder header = new();
        foreach (var keyValuePair in data)
        {
            header.Append(keyValuePair.Key + ',');
        }
        header.Append('\n');

        File.WriteAllText(DATA_PATH + fileName, header.ToString());
    }
    public static Dictionary<string, List<string>> ConvertSettingsToDataDict(List<UISettings> settings){
        Dictionary<string,List<string>> data = [];
        foreach (var item in settings)
        {
            data.Add(item.Name, []);
        }
        return data;
    }
    public static void ValidateDataPath() {
        if (!Directory.Exists(DATA_PATH))
        {
            Directory.CreateDirectory(DATA_PATH);
        }
    }
}
