using AVFoundation;

namespace FRSP6498;

public partial class InputPage : ContentPage
{
    readonly List<UISettings> loadedSettings = [];
    readonly Dictionary<string, List<string>> submissions = [];
    /// <summary>
    /// Where in the submissions dictionary to add data
    /// </summary>
    int submissionIndex = 0;
    public InputPage(List<UISettings> loadedSettings){
        this.loadedSettings = loadedSettings;
        //add input names to the dictionary to save data
        //(values are of type List<string> to allow for multiple inputs without recreating the dictionary)
        foreach (var control in loadedSettings)
        {
            if (control.DataType != null)
            {
                submissions.Add(control.Name, []);
            }
        }
        InitializeComponent();
    }
    public void CreateControls(){
        foreach (var control in loadedSettings)
        {
            switch (control.DataType)
            {
                case"double":
                    var NumberBox = new Entry() {Placeholder = control.Name, ClassId = control.Name};
                    NumberBox.TextChanged += HandleNumberInputChanged;
                    CustomGrid.Add(InputUtil.WrapWithLabel(NumberBox,StackOrientation.Horizontal, control.Name));
                    break;
                //TODO: Make a control to handle number input
                case "string":
                    var Entry = new Entry() {Placeholder = control.Name, ClassId = control.Name};
                    Entry.TextChanged += HandleStringInputChanged;
                    CustomGrid.Add(InputUtil.WrapWithLabel(Entry, StackOrientation.Horizontal, control.Name));
                break;
                case"rtf":
                    var editor = new Editor() {ClassId = control.Name};
                    editor.TextChanged += HandleStringInputChanged;
                    CustomGrid.Add(InputUtil.WrapWithLabel(editor, StackOrientation.Vertical, control.Name));
                    break;
                case"bool":
                    var checkBox = new CheckBox() {ClassId = control.Name};
                    checkBox.CheckedChanged += HandleBoolInputChanged;
                    CustomGrid.Add(InputUtil.WrapWithLabel(checkBox, StackOrientation.Horizontal, control.Name));
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
            submissions[obj!.ClassId].Insert(submissionIndex, obj.Text);
        }else if (sender.GetType() == typeof(Editor))
        {
            var obj = sender as Editor;
            submissions[obj!.ClassId].Insert(submissionIndex, obj.Text);
        }
    }
    public void HandleBoolInputChanged(object? sender, CheckedChangedEventArgs e){
        if(sender!.GetType() == typeof(CheckBox)){
            var obj = sender as CheckBox;
            submissions[obj!.ClassId].Insert(submissionIndex, $"{obj.IsChecked}");
        }
    }
    public void HandleNumberInputChanged(object? sender, EventArgs e){
        if (sender!.GetType() == typeof(Entry))
        {
           var obj = sender as Entry;
           string text = obj!.Text;
           try
           {
                double value = double.Parse(text);
                //if it hasn't thrown exception by now the input is good
                submissions[obj!.ClassId].Insert(submissionIndex, $"{value}");
           }
           catch (Exception)
           {
            //latest input is not good so remove it from the control
                obj.Text = text.Substring(text.Length-2, text.Length -1);
                sender = obj;
           }
        }
    }
    public void HandleSubmission(object? sender, EventArgs e){
        //regenerate grid to clear all previous inputs
        CustomGrid.Clear();
        CreateControls();
        //increment dictionary index to add data to
        submissionIndex += 1;
    }
    public void HandleSave(object? sender, EventArgs e){

    }
}
