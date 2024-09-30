using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaEdit;
using Markdown.Avalonia.Full;
using MatoEditor.Dialogs;
using MatoEditor.Models;
using MatoEditor.Services;
using ReactiveUI;
using Ursa.Controls;

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
        Editor.KeyDown += Editor_KeyDown;

        Viewer = _window.FindControl<UserControl>("EditorUserControl").FindControl<MarkdownScrollViewer>("MarkdownScrollViewer");
        InsertSymbolCommand = ReactiveCommand.Create<string>(InsertSymbol);
        
        ContentString = "";
        FileTabs = new ObservableCollection<FileTab>();
        DeleteFileTabCommand = ReactiveCommand.Create<string>(DeleteFileTab);
        
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
                if (SelectedFileTab != null)
                {
                    this.SelectedFileTab.NewContentString = ContentString;
                }
            });
        this.WhenAnyValue(x => x.SelectedFileTab.Path)
            .Subscribe(path =>
            {
                if (path != null && path != "")
                {
                    _storageService.CurrentFilePath = path;
                }
            });
        _storageService.WhenAnyValue(x => x.CurrentFilePath)
            .Subscribe(async CurrentFilePath =>
            {
                if (CurrentFilePath != null && CurrentFilePath != "")
                {
                    var fileTab = FindFileTab(CurrentFilePath);
                    if (fileTab == null)
                    {
                        var content = await GetContentStringFormFile(CurrentFilePath);
                        var newFileTab = new FileTab()
                        {
                            Name = Path.GetFileName(CurrentFilePath),
                            Path = CurrentFilePath,
                            OldContentString = content,
                            NewContentString = content
                        };
                        FileTabs.Add(newFileTab);
                    }
                    SelectedFileTab = FindFileTab(CurrentFilePath);
                    ContentString = SelectedFileTab.NewContentString;
                }
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
    private ObservableCollection<FileTab> _fileTabs;
    public ObservableCollection<FileTab> FileTabs
    {
        get => _fileTabs;
        set => this.RaiseAndSetIfChanged(ref _fileTabs, value);
    }
    public ICommand DeleteFileTabCommand { get; }

    private FileTab _selectedFileTab;
    public FileTab SelectedFileTab
    {
        get => _selectedFileTab;
        set => this.RaiseAndSetIfChanged(ref _selectedFileTab, value);
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
    private async void InsertSymbol(string symbol)
    {
        var caretOffset = Editor.CaretOffset;
        if (symbol == "table")
        {
            var options = new DialogOptions()
            {
                Title = "Create Table",
                Mode = DialogMode.None,
                Button = DialogButton.OKCancel,
                ShowInTaskBar = false,
                IsCloseButtonVisible = true,
                StartupLocation = WindowStartupLocation.CenterOwner,
            };
            var twoTextBoxDialogViewModel = new TwoNumberBoxDialogViewModel()
            {
                Label1 = "Row",
                Label2 = "Column",
            };
            var result  = await Dialog.ShowModal<TwoNumberBoxDialog, TwoNumberBoxDialogViewModel>(twoTextBoxDialogViewModel, options: options);
            if (result == DialogResult.OK)
            {
                var row = twoTextBoxDialogViewModel.Number1;
                var column = twoTextBoxDialogViewModel.Number2;
                var sb = new System.Text.StringBuilder();

                for (int i = 0; i < column; i++)
                {
                    sb.Append("| ");
                }
                sb.AppendLine("|");

                for (int i = 0; i < column; i++)
                {
                    sb.Append("| --- ");
                }
                sb.AppendLine("|");

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        sb.Append("| ");
                    }
                    sb.AppendLine("|");
                }

                symbol = sb.ToString();
            }
            else
            {
                return;
            }
        }
        Editor.Document.Insert(caretOffset, symbol);
        Editor.CaretOffset = caretOffset + symbol.Length;
    }
    private async Task<string> GetContentStringFormFile(string filePath)
    {
        return await _fileSystemService.ReadFileAsync(filePath);
    }
    public async Task SaveFile()
    {
        if (SelectedFileTab != null)
        {
            _ = await _fileSystemService.WriteFileAsync(SelectedFileTab.Path, SelectedFileTab.NewContentString);
            SelectedFileTab.OldContentString = SelectedFileTab.NewContentString;
        }
    }
    private void Editor_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.S && e.KeyModifiers == KeyModifiers.Control)
        {
            SaveFile();
            e.Handled = true;
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
    private FileTab? FindFileTab(string path)
    {
        return FileTabs.FirstOrDefault(fileTab => fileTab.Path == path);
    }
    private async void DeleteFileTab(string path)
    {
        var fileTab = FindFileTab(path);
        if (fileTab != null)
        {
            if (fileTab.OldContentString != fileTab.NewContentString)
            {
                var options = new DialogOptions()
                {
                    Title = "Do you want to save the file changes?",
                    Mode = DialogMode.None,
                    Button = DialogButton.OKCancel,
                    ShowInTaskBar = false,
                    IsCloseButtonVisible = true,
                    StartupLocation = WindowStartupLocation.CenterOwner,
                };
                var baseDialogViewModel = new BaseDialogViewModel();
                var result  = await Dialog.ShowModal<BaseDialog, BaseDialogViewModel>(baseDialogViewModel, options: options);
                if (result == DialogResult.OK)
                {
                    var ok = await _fileSystemService.WriteFileAsync(fileTab.Path, fileTab.NewContentString);
                    if (ok)
                    {
                        FileTabs.Remove(fileTab);
                    }
                }
            }
            FileTabs.Remove(fileTab);
        }

        if (SelectedFileTab == null)
        {
            ContentString = "";
        }
    }
}