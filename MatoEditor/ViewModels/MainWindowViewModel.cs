using MatoEditor.Services;

namespace MatoEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public DocumentTreeViewModel DocumentTreeViewModel { get; }

    public MainWindowViewModel(IFileSystemService fileSystemService)
    {
        DocumentTreeViewModel = new DocumentTreeViewModel(fileSystemService);
    }
}