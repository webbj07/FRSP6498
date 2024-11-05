
using System.Diagnostics;
using Microsoft.Maui.Controls.Shapes;

namespace FRSP6498;

public partial class MainPage : ContentPage
{
    private readonly List<string> configs = [];

    public MainPage()
    {
        InitializeComponent();
        RefreshConfigs();
        NavigatedTo += MainPage_NavigatedTo;
    }

    private void MainPage_NavigatedTo(object? sender, NavigatedToEventArgs e) => RefreshConfigs();

    private void Refresh_Clicked(object sender, EventArgs e)
    {
        RefreshConfigs();
    }
    private void RefreshConfigs()
    {
        configs.Clear();
        configView.Clear();
        var fullFiles = Directory.GetFiles(ConfigUtil.CONFIG_DIRECTORY);
        foreach (var item in fullFiles)
        {
            configs.Add(System.IO.Path.GetFileName(item));
        }
        foreach (var config in configs)
        {
            var border = new Border() { Stroke = new SolidColorBrush(Colors.Gray), StrokeThickness = 1, StrokeShape = new RoundRectangle() { CornerRadius = 10 } };
            var g = new Grid() {Padding = new Thickness(4, 4), ClassId = config };
            g.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(10, GridUnitType.Star)));
            g.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            g.ColumnDefinitions.Add(new ColumnDefinition(new GridLength (1, GridUnitType.Star)));
            g.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            
            var configName = new Label() { Text = config, VerticalOptions = LayoutOptions.Center, FontSize = 18, Margin = new Thickness(10,0,0,0) };
            var Open = new Button() { Text = "Open", ClassId = config /*bad way to keep track of what file this button is talking about*/, WidthRequest = 80 };
            var Edit = new Button() { Text = "Edit", ClassId = config, WidthRequest = 80};
            var Delete = new Button() { Text = "Delete", ClassId = config, WidthRequest=80};
            Open.Clicked += Config_Clicked;
            Edit.Clicked += EditConfig_Clicked;
            Delete.Clicked += DeleteConfig_Clicked;
            g.Add(configName, 0);
            g.Add(Open, 1/*Column to add the control to*/);
            g.Add(Edit, 2);
            g.Add(Delete, 3);
            border.Content = g;
            configView.Add(border);
        }
        if (configs.Count >0)
        {
            Debug.WriteLine("Existing Configs Found");
        }
        else
        {
            Debug.WriteLine($"No Existing Configs Found at path {ConfigUtil.CONFIG_DIRECTORY}");
        }
    }
    private void Config_Clicked(object? sender, EventArgs e)
    {
        //TODO: figure out how to navigate to a new page
        var b = sender as Button;
        var path = FileSystem.Current.AppDataDirectory + "\\" + b!.Text;
        Debug.WriteLine(path);
    }

    private async void AddNew_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new ConfigCreator(null, null));
    }
    private async void EditConfig_Clicked(object? sender, EventArgs e)
    {
        var button = sender as Button;
        await Navigation.PushAsync(new ConfigCreator(ConfigUtil.ReadConfig(button!.ClassId), button!.ClassId[..button!.ClassId.IndexOf('.')]));
    }
    private void DeleteConfig_Clicked(object? sender, EventArgs e)
    {
        var b = sender as Button;
        if (File.Exists(ConfigUtil.CONFIG_DIRECTORY + b!.ClassId))
        {
            File.Delete(ConfigUtil.CONFIG_DIRECTORY + b.ClassId);
            RefreshConfigs();
        }
    }
}

