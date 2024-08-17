using System;
using System.Windows.Input;
using Avalonia.Controls;
using AvaloniaEdit;
using Markdig;
using MatoEditor.Services;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class EditorViewModel : ViewModelBase
{
    public EditorViewModel(Window window, IFileSystemService fileSystemService, StorageService storageService)
    {
        _window = window;
        _fileSystemService = fileSystemService;
        _storageService = storageService;

        _textEditor = _window.FindControl<UserControl>("EditorUserControl").FindControl<TextEditor>("TextEditor");
        InsertSymbolCommand = ReactiveCommand.Create<string>(InsertSymbol);
        
        ContentString = "";
        ContentHtml = "";

        EditorVisible = true;
        ViewerVisible = true;
        SetEditorModeCommand = ReactiveCommand.Create<string>(SetEditorMode);
        
        _storageService.WhenAnyValue(x => x.CurrentFilePath)
            .Subscribe(CurrentFilePath =>
            {
                UpdateContentString(CurrentFilePath);
            });
        this.WhenAnyValue(x => x.ContentString).Subscribe(_ =>
        {
            ConvertMarkdown();
            SaveFile();
        });
    }

    private readonly Window _window;
    private readonly IFileSystemService _fileSystemService;
    private StorageService _storageService;

    private TextEditor _textEditor { get; set; }
    public ICommand InsertSymbolCommand { get; }
    private void InsertSymbol(string symbol)
    {
        var caretOffset = _textEditor.CaretOffset;
        _textEditor.Document.Insert(caretOffset, symbol);
        _textEditor.CaretOffset = caretOffset + symbol.Length;
    }
    
    private string _contentString;
    private string _contentHtml;

    public string ContentString
    {
        get => _contentString;
        set => this.RaiseAndSetIfChanged(ref _contentString, value);
    }

    public string ContentHtml
    {
        get => _contentHtml;
        set => this.RaiseAndSetIfChanged(ref _contentHtml, value);
    }
    private void ConvertMarkdown()
    {
        ContentHtml = ContentString == "" ? "<br/>" : Markdown.ToHtml(ContentString);
    }

    private async void UpdateContentString(string filePath)
    {
        ContentString = await _fileSystemService.ReadFileAsync(filePath);
    }

    private async void SaveFile()
    {
        _ = await _fileSystemService.WriteFileAsync(_storageService.CurrentFilePath, ContentString);
    }

    private bool _editorVisible;
    public bool EditorVisible
    {
        get => _editorVisible;
        set => this.RaiseAndSetIfChanged(ref _editorVisible, value);
    }
    private bool _viewerVisible;
    public bool ViewerVisible
    {
        get => _viewerVisible;
        set => this.RaiseAndSetIfChanged(ref _viewerVisible, value);
    }
    
    public ICommand SetEditorModeCommand { get; }
    private void SetEditorMode(string mode)
    {
        if (mode == "edit")
        {
            EditorVisible = true;
            ViewerVisible = false;
        }
        else if (mode == "view")
        {
            EditorVisible = false;
            ViewerVisible = true;
        }
        else
        {
            EditorVisible = true;
            ViewerVisible = true;
        }
    }
}