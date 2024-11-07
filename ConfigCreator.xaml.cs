using System.Diagnostics;

namespace FRSP6498;

public partial class ConfigCreator : ContentPage
{
    private readonly List<UISettings> settings = [];
    private UISettings currentSettings = new() {Name = ""};
    /// <summary>
    /// Used for finding a specific Control in the settings list after it has been added
    /// </summary>
    private int settingCount = 0;
    private int? editIndex = null;
    private string? fileName = null;
	internal ConfigCreator(List<UISettings>? preWrittenConfig, string? fileName)
	{
        InitializeComponent();
        if (preWrittenConfig != null)
        {
            settings = preWrittenConfig;
            Debug.WriteLine("Added Pre-Configured Controls:");
            foreach (var item in preWrittenConfig)
            {
                Debug.WriteLine(item.Name);
                //add the settings in the config in the order they were read
                currentSettings = item;
                DisplaySetting(null);
            }
        }
        this.fileName = fileName;
	}
    public void Control_Add_Clicked(object sender, EventArgs e)
    {
        var b = sender as Microsoft.Maui.Controls.Button;
        switch (b!.Text)
        {
            case "CheckBox":
                currentSettings.DataType = "bool";
                AddCommonSettings();
                break;
            case "Rich Text Box":
                currentSettings.DataType = "rtf";
                AddCommonSettings();
                break;
            case "Simple Text Input":
                currentSettings.DataType = "string";
                AddCommonSettings();
                break;
            case "Number Input":
                currentSettings.DataType = "double";
                AddCommonSettings();
                break;
            default:
                break;
        }
    }
    public void AddCommonSettings()
    {
        if (UiSettings.Children.Count > 0)
        {
            UiSettings.Children.Clear();
            var type = currentSettings.DataType;
            currentSettings = new()
            {
                Name = "",
                DataType = type
            };
        }
        var ControlTypeSelected = new Label() { Text = currentSettings.DataType, FontSize = 24, HorizontalOptions = LayoutOptions.Center };
        var nameInfor = new Label() { Text = "Note--The name of the control is also the text that is displayed on the control when loaded into the ui" };
        var nameEntry = new Entry() { Placeholder = "Control Name" };
        nameEntry.TextChanged += OtherUiInformationChanged;
        var elementPosition = new RadioButton() { Content = "start" };
        elementPosition.CheckedChanged += ElementPositionTextChanged;
        var radioButton = new RadioButton() { Content = "center" };
        radioButton.CheckedChanged += ElementPositionTextChanged;
        var radioButton2 = new RadioButton() { Content = "end"};
        radioButton2.CheckedChanged += ElementPositionTextChanged;

        UiSettings.Add(ControlTypeSelected);
        UiSettings.Add(nameInfor);
        UiSettings.Add(nameEntry);
        UiSettings.Add(new Label() { Text = "Position of the element in the ui:" });
        UiSettings.Add(radioButton);
        UiSettings.Add(radioButton2);
        UiSettings.Add(elementPosition);

    }
    private void AddPreConfiguredSettings(UISettings preConfigSettings)
    {
        currentSettings = preConfigSettings;
        if (UiSettings.Children.Count > 0)
        {
            UiSettings.Children.Clear();
            var type = currentSettings.DataType;
            currentSettings = new(){Name = ""};
            currentSettings.DataType = type;
        }
        var ControlTypeSelected = new Label() { Text = currentSettings.DataType, FontSize = 24, HorizontalOptions = LayoutOptions.Center };
        var nameInfor = new Label() { Text = "Note--The name of the control is also the text that is displayed on the control when loaded into the ui" };
        var nameEntry = new Entry() { Placeholder = "Control Name", Text= currentSettings.Name };
        nameEntry.TextChanged += OtherUiInformationChanged;

        var elementLeftAligned = new RadioButton() { Content = "start" };
        elementLeftAligned.CheckedChanged += ElementPositionTextChanged;

        var elementCentered = new RadioButton() { Content = "center" };
        elementCentered.CheckedChanged += ElementPositionTextChanged;

        var elementRightAligned = new RadioButton() { Content = "end"};
        elementRightAligned.CheckedChanged += ElementPositionTextChanged;

        switch (currentSettings.Position)
        {
            case "center": elementCentered.IsChecked = true; break;
            case "start": elementLeftAligned.IsChecked = true; break;
            case "end": elementRightAligned.IsChecked = true; break;
        }

        UiSettings.Add(ControlTypeSelected);
        UiSettings.Add(nameInfor);
        UiSettings.Add(nameEntry);
        UiSettings.Add(new Label() { Text = "Position of the element in the ui:" });
        UiSettings.Add(elementCentered);
        UiSettings.Add(elementLeftAligned);
        UiSettings.Add(elementRightAligned);
    }
    public void ElementPositionTextChanged(object? sender, EventArgs e)
    {
        var button = sender as RadioButton;
        currentSettings.Position = button!.Content.ToString();
    }
    public void OtherUiInformationChanged(object? sender, EventArgs e)
    {
        var entry = sender as Entry;
        if (entry!.Placeholder.Equals("Control Name"))
        {
            currentSettings.Name = entry.Text;
        }
    }
    /// <summary>
    /// Resets the current UiSettings object and clears the ui
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Submit_Clicked(object? sender, EventArgs e)
    {
        if (editIndex != null)
        {
            settings.RemoveAt((int)editIndex);
            settingCount -= 1;
            settings.Insert((int)editIndex, currentSettings);
            DisplaySetting(editIndex);

            Debug.WriteLine("Inserting");
            editIndex = null;
        }
        else
        {
            DisplaySetting(null);
            settings.Add(currentSettings);
            Debug.WriteLine("Adding");

        }

        settingCount += 1;
        currentSettings = new(){
            Name = ""
        };
        UiSettings.Children.Clear();
    }
    private void DisplaySetting(int? index)
    {

        var controlType = "";

        switch (currentSettings.DataType)
        {
            case "double": controlType = "Number Input"; break;
            case "bool": controlType = "checkBox"; break;
            case "string": controlType = "Simple Text Input"; break;
            case "rtf": controlType = "Rich Text Box"; break;
        }

        var controlGrid = new Grid();
        controlGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        controlGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(60)));

        controlGrid.Add(new Label() { Text = $"{controlType} - {currentSettings.Name}", VerticalOptions = LayoutOptions.Center, ClassId = settingCount.ToString() }, 0);

        var editButton = new Button() { Text = "Edit", ClassId = settingCount.ToString(), WidthRequest = 60 };
        editButton.Clicked += Edit_Clicked;
        controlGrid.Add(editButton, 1);

        if(index != null)
        {
            CurrentControlView.Children.RemoveAt((int)index);
            CurrentControlView.Children.Insert((int)index, controlGrid);
        }
        else
        {
            CurrentControlView.Add(controlGrid);
        }

    }
    private async void Save_Clicked(object? sender, EventArgs e)
    {
        //only ask the user for a fileName if there is not one already defined
        //fileName should only be defined at this point if the user is editing a config
        fileName ??= await DisplayPromptAsync("File Name", "Enter a file name (leave empty to cancel)");

        if (fileName != null && fileName != string.Empty) {
            Debug.WriteLine($"Attempting to write config with name {fileName}");
            ConfigUtil.WriteConfig(settings, fileName);
        }
        else
        {
            Debug.WriteLine("Config Write Canceled");
        }
        await Shell.Current.GoToAsync("..");
    }
    private void Edit_Clicked(object? sender, EventArgs e)
    {
        var button = sender as Button;
        if(int.TryParse(button!.ClassId, out var r))
        {
            AddPreConfiguredSettings(settings[r]);
            editIndex = r;
            Debug.WriteLine("Editing");
            return;
        }
        else
        {
            Debug.WriteLine($"Setting to Edit not found ({button.ClassId}");
        }
    }
}
