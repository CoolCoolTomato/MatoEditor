using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using AvaloniaEdit;
using Markdown.Avalonia.Full;
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
        
        Editor = _window.FindControl<UserControl>("EditorUserControl").FindControl<TextEditor>("TextEditor");
        Editor.TextArea.TextView.LinkTextForegroundBrush = Editor.Foreground;
        Viewer = _window.FindControl<UserControl>("EditorUserControl").FindControl<MarkdownScrollViewer>("MarkdownScrollViewer");
        InsertSymbolCommand = ReactiveCommand.Create<string>(InsertSymbol);
        
        ContentString = "";
        
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
        this.WhenAnyValue(x => x.ContentString)
            .Subscribe(ContentString =>
            {
                try
                {
                    Viewer.Markdown = ContentString;
                }
                catch (Exception e)
                {
                    Viewer.Markdown = "";
                    Viewer.Markdown = ContentString;
                }
            });
        _storageService.WhenAnyValue(x => x.CurrentFilePath)
            .Subscribe(CurrentFilePath =>
            {
                UpdateContentString(CurrentFilePath);
            });
    }
    private readonly Window _window;
    private readonly IFileSystemService _fileSystemService;
    private StorageService _storageService;
    
    public TextEditor Editor { get; set; }
    public MarkdownScrollViewer Viewer { get; set; }
    
    private string _contentString;
    public string ContentString
    {
        get => _contentString;
        set => this.RaiseAndSetIfChanged(ref _contentString, value);
    }
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
        var caretOffset = Editor.CaretOffset;
        Editor.Document.Insert(caretOffset, symbol);
        Editor.CaretOffset = caretOffset + symbol.Length;
    }
    private async void UpdateContentString(string filePath)
    {
        ContentString = await _fileSystemService.ReadFileAsync(filePath);
    }
    public async Task SaveFile()
    {
        if (_storageService.CurrentFilePath != "")
        {
            _ = await _fileSystemService.WriteFileAsync(_storageService.CurrentFilePath, ContentString);
        }
    }
    public void SetEditorMode(string mode)
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