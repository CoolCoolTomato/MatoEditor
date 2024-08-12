using MatoEditor.Services;
using MatoEditor.Views;

namespace MatoEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public NavigationViewModel NavigationViewModel { get; }
    public DocumentTreeViewModel DocumentTreeViewModel { get; }
    public EditorViewModel EditorViewModel { get; }

    public MainWindowViewModel(IFileSystemService fileSystemService, StorageService storageService)
    {
        NavigationViewModel = new NavigationViewModel();
        DocumentTreeViewModel = new DocumentTreeViewModel(fileSystemService, storageService);
        EditorViewModel = new EditorViewModel();
        
    }
}