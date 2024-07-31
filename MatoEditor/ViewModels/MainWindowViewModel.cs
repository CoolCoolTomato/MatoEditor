using MatoEditor.Services;

namespace MatoEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public CategoryViewModel CategoryViewModel { get; }

    public MainWindowViewModel(IFileSystemService fileSystemService)
    {
        CategoryViewModel = new CategoryViewModel(fileSystemService);
    }
}