using System;
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
        SetEditorModeCommand = ReactiveCommand.Create<string>(SetEditorMode);

        _storageService.WhenAnyValue(x => x.CurrentFilePath)
            .Subscribe(CurrentFilePath =>
            {
                FilePath = CurrentFilePath;
            });
    }
    private readonly Window _window;
    private StorageService _storageService;
    private ConfigurationService _configurationService;

    private string _filePath;
    public string FilePath
    {
        get => _filePath;
        set => this.RaiseAndSetIfChanged(ref _filePath, value);
    }
    private string _editorMode;
    public string EditorMode
    {
        get => _editorMode;
        set => this.RaiseAndSetIfChanged(ref _editorMode, value);
    }
    
    public ICommand SelectDirectoryCommand { get; }
    public ICommand SetEditorModeCommand { get; }
    
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
    private void SetEditorMode(string mode)
    {
        EditorMode = mode;
    }
}