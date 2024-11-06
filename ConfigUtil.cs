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
    internal static List<IView> GenerateControlsFromConfig(List<UISettings> controls){
        List<IView> controlsList = [];
        foreach (var control in controls){
            switch (control.DataType)
            {
                case "bool":
                CheckBox c = new CheckBox();

                controlsList.Add(new CheckBox());
                break;
                case "double":
                controlsList.Add(new Entry(){Placeholder = control.Name});
                break;
                case "rtf":
                controlsList.Add(new Editor(){Placeholder = control.Name});
                break;
                case "string":
                controlsList.Add(new Entry(){Placeholder = control.Name});
                break;
                default:
                break;
            }
        }
        return controlsList;
    }
    internal static void CheckConfigDirectory()
    {
        if(!Directory.Exists(CONFIG_DIRECTORY))
        {
            Directory.CreateDirectory(CONFIG_DIRECTORY);
        }
    }
}
