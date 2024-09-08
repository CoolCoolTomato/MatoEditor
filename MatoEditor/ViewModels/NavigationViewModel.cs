using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MatoEditor.Services;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class NavigationViewModel : ViewModelBase
{
    public NavigationViewModel(Window window, StorageService storageService, ConfigurationService configurationService)
    {
        _window = window;
        _storageService = storageService;
        _configurationService = configurationService;
        SelectDirectoryCommand = ReactiveCommand.CreateFromTask(SelectDirectory);
    }
    private readonly Window _window;
    private StorageService _storageService;
    private ConfigurationService _configurationService;

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
            _configurationService.SaveConfiguration();
        }
    }
}