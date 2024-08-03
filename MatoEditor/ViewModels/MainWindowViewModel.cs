using MatoEditor.Services;
using MatoEditor.Views;

namespace MatoEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public DocumentTreeViewModel DocumentTreeViewModel { get; }
    public EditorViewModel EditorViewModel { get; }

    public MainWindowViewModel(IFileSystemService fileSystemService)
    {
        DocumentTreeViewModel = new DocumentTreeViewModel(fileSystemService);
        EditorViewModel = new EditorViewModel();
    }
}