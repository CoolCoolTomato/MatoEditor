using Avalonia.Controls;
using MatoEditor.Services;
using MatoEditor.Views;

namespace MatoEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Window _window;
    public NavigationViewModel NavigationViewModel { get; }
    public DocumentTreeViewModel DocumentTreeViewModel { get; }
    public EditorViewModel EditorViewModel { get; }

    public MainWindowViewModel(Window window, IFileSystemService fileSystemService, StorageService storageService)
    {
        _window = window;
        NavigationViewModel = new NavigationViewModel(window, storageService);
        DocumentTreeViewModel = new DocumentTreeViewModel(fileSystemService, storageService);
        EditorViewModel = new EditorViewModel();
        
    }
}