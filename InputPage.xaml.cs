namespace FRSP6498;

public partial class InputPage : ContentPage
{
    List<UISettings> loadedSettings = [];
    public InputPage(List<UISettings> loadedSettings){
        this.loadedSettings = loadedSettings;
        InitializeComponent();
    }
}
