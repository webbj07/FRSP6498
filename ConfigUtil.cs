using System.Diagnostics;
using System.Text.Json;

namespace FRSP6498;
internal static class ConfigUtil
{
    public static readonly string CONFIG_DIRECTORY = FileSystem.Current.AppDataDirectory + "/configs/";
    public static readonly string LAST_CONFIG_PATH = FileSystem.Current.AppDataDirectory + "/lastconfig.txt";
    public static string WriteConfig(List<UISettings> settings, string fileName)
    {
        CheckConfigDirectory();
        Debug.WriteLine($"{settings.Count} items found -- Writing");
        var path = CONFIG_DIRECTORY + $"{fileName}.json";
        var fulljson = "";
        foreach (var item in settings)
        {
            var json = JsonSerializer.Serialize(item);
            Debug.WriteLine($"{json} written to {path}");
            fulljson+=json;
        }
        File.WriteAllText(path, fulljson);
        return fulljson;
    }
    internal static List<UISettings>? ReadConfig(string fileName)
    {
        CheckConfigDirectory();
        string json;
        try
        {
            Debug.WriteLine($"Attempting read from {CONFIG_DIRECTORY + fileName}");
            json = File.ReadAllText(CONFIG_DIRECTORY + fileName);
        }catch (Exception)
        {
            return [];
        }
        var controls = json.Split("}", StringSplitOptions.RemoveEmptyEntries);
        List<UISettings> config = [];
        Debug.WriteLine($"Controls found in config {fileName}");
        foreach (var item in controls)
        {
            var control = JsonSerializer.Deserialize<UISettings>(item + "}");
            if (control != null)
            {
                config.Add(control);
                Debug.WriteLine(control.Name);
            }
        }
        return config;
    }
    /// <summary>
    /// Sums the value of all the characters in a string
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static long ConvertConfigToID(string fileName){
        CheckConfigDirectory();
        if (!File.Exists(CONFIG_DIRECTORY+fileName))
        {
            return 0;
        }
        var json = File.ReadAllText(CONFIG_DIRECTORY + fileName);
        return StringToID(json);
    }
    public static long StringToID(string str){
        long sum = 0;
        for (var i = 0; i < str.Length; i++)
        {
            sum+=str[i];
        }
        return sum;
    }
    internal static void CheckConfigDirectory()
    {
        if(!Directory.Exists(CONFIG_DIRECTORY))
        {
            Directory.CreateDirectory(CONFIG_DIRECTORY);
        }
    }
    public static int PositionToGridCol(string position)
    {
        return position switch
        {
            "start" => 0,
            "center" => 1,
            "end" => 2,
            _ => 0,
        };
    }
    public static void SaveConfigToMemory(string configName)
    {
        Debug.WriteLine($"Saving config file {configName} to program memory");
        File.WriteAllText(LAST_CONFIG_PATH, configName);
    }
    public static string? GetLastConfigFromMemory()
    {
        if (!File.Exists(LAST_CONFIG_PATH))
        {
            File.Create(LAST_CONFIG_PATH);
            return null;
        }
        else
        {
            return File.ReadAllText(LAST_CONFIG_PATH);
        }
    }
}
