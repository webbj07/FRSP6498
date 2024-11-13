
using System.Diagnostics;
using CommunityToolkit.Maui.Storage;
namespace FRSP6498;

public partial class InputPage : ContentPage
{
    private readonly List<UISettings> loadedSettings = [];
    private readonly Dictionary<string, string> submissions = [];
    private readonly long currentID = 0;
    /// <summary>
    /// Where in the submissions dictionary to add data
    /// </summary>
    private int submissionIndex = 0;
    public InputPage(List<UISettings> loadedSettings, string rootConfigFileName){
        this.loadedSettings = loadedSettings;
        currentID = ConfigUtil.ConvertConfigToID(rootConfigFileName);
        ConfigUtil.SaveConfigToMemory(rootConfigFileName);
        //add input names to the dictionary to save data
        //(values are of type List<string> to allow for multiple inputs without recreating the dictionary)
        submissions.Add("TeamNum", string.Empty);
        submissions.Add("MatchNum", string.Empty);
        foreach (var control in loadedSettings)
        {
            if (control.DataType != null)
            {
                submissions.Add(control.Name, string.Empty);
            }
        }
        InitializeComponent();
        DataUtil.ValidateDataFile(currentID.ToString(), submissions);
        CreateControls();
    }
    public void CreateControls(){
        CustomGrid.RowDefinitions.Clear();
        for (var i = 0; i <(int)Math.Ceiling((double)loadedSettings.Count / 3); i++)
        {
            CustomGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        }
        int[] rowPositions = [0, 0, 0];
        var currentPosition = 0;
        foreach (var control in loadedSettings)
        {
            switch (control.DataType)
            {
                case"double":
                    var NumberBox = new Entry() {Placeholder = control.Name, ClassId = control.Name, HeightRequest = 50, HorizontalOptions=LayoutOptions.End};
                    NumberBox.TextChanged += HandleNumberInputChanged;
                    currentPosition = ConfigUtil.PositionToGridCol(control.Position!);
                    CustomGrid.Add(InputUtil.WrapWithLabel(NumberBox,StackOrientation.Horizontal, control.Name),currentPosition, rowPositions[currentPosition]);
                    rowPositions[currentPosition] += 1;
                    break;
                //TODO: Make a control to handle number input
                case "string":
                    var entry = new Entry() {Placeholder = control.Name, ClassId = control.Name, HeightRequest = 50, HorizontalOptions=LayoutOptions.End};
                    entry.TextChanged += HandleStringInputChanged;
                    currentPosition = ConfigUtil.PositionToGridCol(control.Position!);
                    CustomGrid.Add(InputUtil.WrapWithLabel(entry, StackOrientation.Horizontal, control.Name),currentPosition, rowPositions[currentPosition]);
                    rowPositions[currentPosition] += 1;
                break;
                case"rtf":
                    var editor = new Editor() {ClassId = control.Name, AutoSize = EditorAutoSizeOption.TextChanges, MaximumHeightRequest = 200};
                    editor.TextChanged += HandleStringInputChanged;
                    currentPosition = ConfigUtil.PositionToGridCol(control.Position!);
                    CustomGrid.Add(InputUtil.WrapWithLabel(editor, StackOrientation.Vertical, control.Name),currentPosition, rowPositions[currentPosition]);
                    rowPositions[currentPosition] += 1;
                    break;
                case"bool":
                    var checkBox = new CheckBox() {ClassId = control.Name, HorizontalOptions = LayoutOptions.End};
                    checkBox.CheckedChanged += HandleBoolInputChanged;
                    currentPosition = ConfigUtil.PositionToGridCol(control.Position!);
                    CustomGrid.Add(InputUtil.WrapWithLabel(checkBox, StackOrientation.Horizontal, control.Name),currentPosition, rowPositions[currentPosition]);
                    rowPositions[currentPosition] += 1;
                    break;
                default: break;
            }
        }
    }
    public void HandleStringInputChanged(object? sender, EventArgs e){
        if (sender!.GetType() == typeof(Entry))
        {
            var obj = sender as Entry;
            //uses classId to store the name of the control
            submissions[obj!.ClassId] =  obj.Text;
        }else if (sender.GetType() == typeof(Editor))
        {
            var obj = sender as Editor;
            submissions[obj!.ClassId] = obj.Text;
        }
    }
    public void HandleBoolInputChanged(object? sender, CheckedChangedEventArgs e){
        if(sender!.GetType() == typeof(CheckBox)){
            var obj = sender as CheckBox;
            submissions[obj!.ClassId] = obj.IsChecked.ToString();
        }
    }
    public void HandleNumberInputChanged(object? sender, EventArgs e){
        if (sender!.GetType() == typeof(Entry))
        {
           var obj = sender as Entry;
           var text = obj!.Text;
           try
           {
                var value = double.Parse(text);
                //if it hasn't thrown exception by now the input is good
                submissions[obj!.ClassId] = value.ToString();
           }
           catch (Exception)
           {
            //latest input is not good so remove it from the control
                
                obj.Text = obj.Text.Length > 0 ? obj.Text.Remove(obj.Text.Length-1) : obj.Text;
            }
        }
    }
    public void HandleSubmission(object? sender, EventArgs e){

        submissions["TeamNum"] = TeamNum.Text;
        submissions["MatchNum"] = MatchNum.Text;
        TeamNum.Text = string.Empty;
        MatchNum.Text = string.Empty;
        DisplayAlert("Alert", "Saved Data", "Ok");

        DataUtil.ValidateDataFile(currentID.ToString(), submissions);
        DataUtil.WriteData(currentID.ToString(), submissions);

        //regenerate grid to clear all previous inputs
        CustomGrid.Clear();
        CreateControls();
        //increment dictionary index to add data to
        submissionIndex += 1;
    }
    public async void HandleExport(object? sender, EventArgs e)
    {
        var stream = File.OpenRead(DataUtil.DATA_PATH + currentID +".csv");
        
        //this will not work on old versions of mac or android

#pragma warning disable CA1416 // Validate platform compatibility
        var result = await FileSaver.SaveAsync(currentID.ToString() + ".csv", stream);
#pragma warning restore CA1416 // Validate platform compatibility
        if (!result.IsSuccessful)
        {
            await DisplayAlert("Warning", "Export Failed - Please try again", "Ok");
        }
    }
}
