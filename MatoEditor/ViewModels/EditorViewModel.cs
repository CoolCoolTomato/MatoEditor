using System;
using System.Windows.Input;
using Avalonia.Controls;
using AvaloniaEdit;
using MatoEditor.Services;
using MatoEditor.utils.Markdown;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class EditorViewModel : ViewModelBase
{
    public EditorViewModel(Window window, IFileSystemService fileSystemService, StorageService storageService, NavigationViewModel navigationViewModel)
    {
        _window = window;
        _fileSystemService = fileSystemService;
        _storageService = storageService;
        _navigationViewModel = navigationViewModel;
        
        _textEditor = _window.FindControl<UserControl>("EditorUserControl").FindControl<TextEditor>("TextEditor");
        InsertSymbolCommand = ReactiveCommand.Create<string>(InsertSymbol);
        
        ContentString = "";
        ContentHtml = "";
        
        EditorVisible = true;
        ViewerVisible = true;
        EditorGridField = new GridField
        {
            Column = 0,
            ColumnSpan = 1
        };
        ViewerGridField = new GridField
        {
            Column = 1,
            ColumnSpan = 1
        };
        SetEditorModeCommand = ReactiveCommand.Create<string>(SetEditorMode);
        _navigationViewModel.WhenAnyValue(x => x.EditorMode)
            .Subscribe(EditorMode =>
            {
                SetEditorMode(EditorMode);
            });
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
    private NavigationViewModel _navigationViewModel;
    
    private string _contentString;
    public string ContentString
    {
        get => _contentString;
        set => this.RaiseAndSetIfChanged(ref _contentString, value);
    }
    private string _contentHtml;
    public string ContentHtml
    {
        get => _contentHtml;
        set => this.RaiseAndSetIfChanged(ref _contentHtml, value);
    }
    
    private TextEditor _textEditor { get; set; }
    public ICommand InsertSymbolCommand { get; }
    
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
    
    private GridField _editorGridField;
    public GridField EditorGridField
    {
        get => _editorGridField;
        set => this.RaiseAndSetIfChanged(ref _editorGridField, value);
    }
    private GridField _viewerGridField;
    public GridField ViewerGridField
    {
        get => _viewerGridField;
        set => this.RaiseAndSetIfChanged(ref _viewerGridField, value);
    }
    
    public ICommand SetEditorModeCommand { get; }
    public class GridField : ReactiveObject
    {
        public GridField()
        {
            Row = 0;
            Column = 0;
            RowSpan = 0;
            ColumnSpan = 0;
        }

        private int _row;
        public int Row
        {
            get => _row;
            set => this.RaiseAndSetIfChanged(ref _row, value);
        }
        private int _column;
        public int Column
        {
            get => _column;
            set => this.RaiseAndSetIfChanged(ref _column, value);
        }
        private int _rowSpan;
        public int RowSpan
        {
            get => _rowSpan;
            set => this.RaiseAndSetIfChanged(ref _rowSpan, value);
        }
        private int _columnSpan;
        public int ColumnSpan
        {
            get => _columnSpan;
            set => this.RaiseAndSetIfChanged(ref _columnSpan, value);
        }
    }
    
    private void InsertSymbol(string symbol)
    {
        var caretOffset = _textEditor.CaretOffset;
        _textEditor.Document.Insert(caretOffset, symbol);
        _textEditor.CaretOffset = caretOffset + symbol.Length;
    }
    private void ConvertMarkdown()
    {
        ContentHtml = ContentString == "" ? "<br/>" : MarkdownConverter.ConvertMarkdownToHtml(ContentString);
    }
    private async void UpdateContentString(string filePath)
    {
        ContentString = await _fileSystemService.ReadFileAsync(filePath);
    }
    private async void SaveFile()
    {
        if (_storageService.CurrentFilePath != "")
        {
            _ = await _fileSystemService.WriteFileAsync(_storageService.CurrentFilePath, ContentString);
        }
    }
    private void SetEditorMode(string mode)
    {
        if (mode == "edit")
        {
            EditorVisible = true;
            ViewerVisible = false;
            EditorGridField.Column = 0;
            EditorGridField.ColumnSpan = 2;
            ViewerGridField.Column = 1;
            ViewerGridField.ColumnSpan = 0;
        }
        else if (mode == "view")
        {
            EditorVisible = false;
            ViewerVisible = true;
            EditorGridField.Column = 1;
            EditorGridField.ColumnSpan = 0;
            ViewerGridField.Column = 0;
            ViewerGridField.ColumnSpan = 2;
        }
        else
        {
            EditorVisible = true;
            ViewerVisible = true;
            EditorGridField.Column = 0;
            EditorGridField.ColumnSpan = 1;
            ViewerGridField.Column = 1;
            ViewerGridField.ColumnSpan = 1;
        }
    }
}