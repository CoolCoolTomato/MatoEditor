using MatoEditor.ViewModels;

namespace MatoEditor.Dialogs;

public class TextBoxDialogViewModel : ViewModelBase
{
    public string Content { get; set; }
    public TextBoxDialogViewModel()
    {
        Content = "";
    }
}