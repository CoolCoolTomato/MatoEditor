using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MatoEditor.Services;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class NavigationViewModel : ViewModelBase
{
    public NavigationViewModel(Window window, StorageService storageService)
    {
        _window = window;
        _storageService = storageService;
        SelectDirectoryCommand = ReactiveCommand.CreateFromTask(SelectDirectory);
    }
    private readonly Window _window;
    private StorageService _storageService;

    public ICommand SelectDirectoryCommand { get; }
    private async Task SelectDirectory()
    {
        var directory = await _window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Directory"
        });

        if (directory.Count > 0)
        {
            _storageService.RootDirectoryPath = directory[0].Path.LocalPath;
        }
    }
}