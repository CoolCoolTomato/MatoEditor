﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using MatoEditor.Services;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class NavigationViewModel : ViewModelBase
{
    public NavigationViewModel(Window window, StorageService storageService, ConfigurationService configurationService, EditorViewModel editorViewModel)
    {
        _window = window;
        _storageService = storageService;
        _configurationService = configurationService;
        _editorViewModel = editorViewModel;

        IsLight = Application.Current.RequestedThemeVariant == ThemeVariant.Light;
        
        SelectDirectoryCommand = ReactiveCommand.CreateFromTask(SelectDirectory);
        SaveFileCommand = ReactiveCommand.CreateFromTask(SaveFile);
        ChangeThemeCommand = ReactiveCommand.Create(ChangeTheme);
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
    private EditorViewModel _editorViewModel;

    private string _filePath;
    public string FilePath
    {
        get => _filePath;
        set => this.RaiseAndSetIfChanged(ref _filePath, value);
    }

    private bool _isLight;
    public bool IsLight
    {
        get => _isLight;
        set => this.RaiseAndSetIfChanged(ref _isLight, value);
    }
    public ICommand SelectDirectoryCommand { get; }
    public ICommand SaveFileCommand { get; }
    public ICommand ChangeThemeCommand { get; }
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
    private async Task SaveFile()
    {
        await _editorViewModel.SaveFile();
    }
    private void ChangeTheme()
    {
        if (IsLight)
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
            _editorViewModel.Editor.TextArea.TextView.LinkTextForegroundBrush = _editorViewModel.Editor.Foreground;
            IsLight = false;
        }
        else
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
            _editorViewModel.Editor.TextArea.TextView.LinkTextForegroundBrush = _editorViewModel.Editor.Foreground;
            IsLight = true;
        }
        _configurationService.SaveConfiguration();
    }
    private void SetEditorMode(string mode)
    {
        _editorViewModel.SetEditorMode(mode);
    }
}