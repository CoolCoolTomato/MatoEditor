using MatoEditor.ViewModels;

namespace MatoEditor.Dialogs;

public class TwoNumberBoxDialogViewModel : ViewModelBase
{
    public string Label1 { get; set; }
    public string Label2 { get; set; }
    public int Number1 { get; set; }
    public int Number2 { get; set; }
    
    public TwoNumberBoxDialogViewModel()
    {
        Label1 = "";
        Label2 = "";
        Number1 = 1;
        Number2 = 1;
    }
}