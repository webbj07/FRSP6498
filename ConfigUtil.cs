using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace FRSP6498;
internal static class ConfigUtil
{
    public static readonly string CONFIG_DIRECTORY = FileSystem.Current.AppDataDirectory + "\\configs\\";
    public static void WriteConfig(List<UISettings> settings, string fileName)
    {
        var path = CONFIG_DIRECTORY + $"{fileName}.json";
        CheckConfigDirectory();
        foreach (var item in settings)
        {
            var json = JsonSerializer.Serialize(item);
            Debug.WriteLine($"{json} written to {CONFIG_DIRECTORY}");
            File.WriteAllText(path, json);
        }
    }
    internal static List<UISettings>? ReadConfig(string fileName)
    {
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
    public static void CheckConfigDirectory()
    {
        if(!Directory.Exists(CONFIG_DIRECTORY))
        {
            Directory.CreateDirectory(CONFIG_DIRECTORY);
        }
    }
}
