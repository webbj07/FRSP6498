using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace FRSP6498;
internal static class ConfigUtil
{
    public static readonly string CONFIG_DIRECTORY = FileSystem.Current.AppDataDirectory + "/configs/";
    public static void WriteConfig(List<UISettings> settings, string fileName)
    {
        CheckConfigDirectory();
        Debug.WriteLine($"{settings.Count} items found -- Writing");
        var path = CONFIG_DIRECTORY + $"{fileName}.json";
        foreach (var item in settings)
        {
            var json = JsonSerializer.Serialize(item);
            Debug.WriteLine($"{json} written to {path}");
            File.WriteAllText(path, json);
        }
    }
    internal static List<UISettings>? ReadConfig(string fileName)
    {
        CheckConfigDirectory();
        var json = File.ReadAllText(CONFIG_DIRECTORY+fileName);
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
    public static long ConvertConfigToID(string fileName){
        CheckConfigDirectory();
        if (!File.Exists(CONFIG_DIRECTORY+fileName))
        {
            return 0;
        }
        var json = File.ReadAllText(CONFIG_DIRECTORY + fileName);
        long charSum=0;
        for (int i = 0; i < json.Length; i++)
        {
            charSum+=json[i];
        }
        return charSum;
    }
    internal static void CheckConfigDirectory()
    {
        if(!Directory.Exists(CONFIG_DIRECTORY))
        {
            Directory.CreateDirectory(CONFIG_DIRECTORY);
        }
    }
}
