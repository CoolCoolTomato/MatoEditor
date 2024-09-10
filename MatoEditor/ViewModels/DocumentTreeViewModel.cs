using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using MatoEditor.Models;
using MatoEditor.Services;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class DocumentTreeViewModel : ViewModelBase
{
    public DocumentTreeViewModel(IFileSystemService fileSystemService, StorageService storageService)
    {
        _fileSystemService = fileSystemService;
        _storageService = storageService;
        _rootNode = new DocumentTreeNode();
        _storageService.WhenAnyValue(x => x.RootDirectoryPath)
            .Subscribe(RootDirectoryPath =>
            {
                _rootNode.Name = Path.GetFileName(RootDirectoryPath);
                _rootNode.Path = RootDirectoryPath;
                _rootNode.SubNodes.Clear();
                _rootNode.IsDirectory = true;
                BuildDocumentTree();
            });
        SelectedNode = new DocumentTreeNode();
        
        OpenCreateDirectoryDialogCommand = ReactiveCommand.Create<string>(OpenCreateDirectoryDialog);
        OpenRenameDirectoryDialogCommand = ReactiveCommand.Create<string>(OpenRenameDirectoryDialog);
        OpenDeleteDirectoryDialogCommand = ReactiveCommand.Create<string>(OpenDeleteDirectoryDialog);
        OpenCreateFileDialogCommand = ReactiveCommand.Create<string>(OpenCreateFileDialog);
        OpenRenameFileDialogCommand = ReactiveCommand.Create<string>(OpenRenameFileDialog);
        OpenDeleteFileDialogCommand = ReactiveCommand.Create<string>(OpenDeleteFileDialog);
        this.WhenAnyValue(x => x.SelectedNode.Path)
            .Subscribe(_ => SelectFile());
    }
    
    private readonly IFileSystemService _fileSystemService;
    private StorageService _storageService;
    
    private DocumentTreeNode _rootNode { get; set; }
    
    private ObservableCollection<DocumentTreeNode> _documentTree;
    public ObservableCollection<DocumentTreeNode> DocumentTree
    {
        get => _documentTree;
        set => this.RaiseAndSetIfChanged(ref _documentTree, value);
    }
    
    private void BuildDocumentTree()
    {
        BuildDocumentNode(_rootNode);
        
        DocumentTree = new ObservableCollection<DocumentTreeNode>() { _rootNode };
    }
    private async void BuildDocumentNode(DocumentTreeNode node)
    {
        IEnumerable<DirectoryInfo> subDirectoryInfos = await _fileSystemService.GetSubDirectories(node.Path);
        foreach (var subDirectoryInfo in subDirectoryInfos)
        {
            var subDirectory = new DocumentTreeNode()
            {
                Name = subDirectoryInfo.Name,
                Path = subDirectoryInfo.FullName,
                IsDirectory = true,
            };
            node.SubNodes.Add(subDirectory);
            BuildDocumentNode(subDirectory);
        }
        
        IEnumerable<FileInfo> fileInfos = await _fileSystemService.GetFiles(node.Path);
        foreach (var fileInfo in fileInfos)
        {
            var file = new DocumentTreeNode()
            {
                Name = fileInfo.Name,
                Path = fileInfo.FullName,
                IsDirectory = false,
            };
            node.SubNodes.Add(file);
        }
    }

    private DocumentTreeNode _selectedNode;
    public DocumentTreeNode SelectedNode
    {
        get => _selectedNode;
        set => this.RaiseAndSetIfChanged(ref _selectedNode, value);
    }

    private void SelectFile()
    {
        if (!SelectedNode.IsDirectory)
        {
            _storageService.CurrentFilePath = SelectedNode.Path;
        }
    }
    
    public ICommand OpenCreateDirectoryDialogCommand { get; }
    private async void OpenCreateDirectoryDialog(string path)
    {
        var dialog = new ContentDialog()
        {
            Title = "Create Directory",
            Content = new TextBox()
            {
                Watermark = "Input directory name"
            },
            PrimaryButtonText = "Create",
            CloseButtonText = "Close",
        };
        
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var directoryName = ((TextBox)dialog.Content).Text;
            await _fileSystemService.CreateDirectoryAsync(path + "/" + directoryName);
            _rootNode.SubNodes.Clear();
            BuildDocumentTree();
        }
    }
    
    public ICommand OpenRenameDirectoryDialogCommand { get; }
    private async void OpenRenameDirectoryDialog(string path)
    {
        var dialog = new ContentDialog()
        {
            Title = "Rename Directory",
            Content = new TextBox()
            {
                Watermark = "Input new directory name"
            },
            PrimaryButtonText = "Rename",
            CloseButtonText = "Close",
        };
        
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var newDirectoryName = ((TextBox)dialog.Content).Text;
            await _fileSystemService.RenameDirectoryAsync(path, Path.GetDirectoryName(path) + "/" + newDirectoryName);
            _rootNode.SubNodes.Clear();
            BuildDocumentTree();
        }
    }
    
    public ICommand OpenDeleteDirectoryDialogCommand { get; }
    private async void OpenDeleteDirectoryDialog(string path)
    {
        var dialog = new ContentDialog()
        {
            Title = "Delete Directory",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Close",
        };
        
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await _fileSystemService.DeleteDirectoryAsync(path);
            _rootNode.SubNodes.Clear();
            BuildDocumentTree();
        }
    }
    
    public ICommand OpenCreateFileDialogCommand { get; }
    private async void OpenCreateFileDialog(string path)
    {
        var dialog = new ContentDialog()
        {
            Title = "Create File",
            Content = new TextBox()
            {
                Watermark = "Input file name"
            },
            PrimaryButtonText = "Create",
            CloseButtonText = "Close",
        };
        
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var fileName = ((TextBox)dialog.Content).Text;
            await _fileSystemService.CreateFileAsync(path + "/" + fileName);
            _rootNode.SubNodes.Clear();
            BuildDocumentTree();
        }
    }
    
    public ICommand OpenRenameFileDialogCommand { get; }
    private async void OpenRenameFileDialog(string path)
    {
        var dialog = new ContentDialog()
        {
            Title = "Rename File",
            Content = new TextBox()
            {
                Watermark = "Input new file name"
            },
            PrimaryButtonText = "Rename",
            CloseButtonText = "Close",
        };
        
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var newDirectoryName = ((TextBox)dialog.Content).Text;
            await _fileSystemService.RenameFileAsync(path, Path.GetDirectoryName(path) + "/" + newDirectoryName);
            _rootNode.SubNodes.Clear();
            BuildDocumentTree();
        }
    }
    
    public ICommand OpenDeleteFileDialogCommand { get; }
    private async void OpenDeleteFileDialog(string path)
    {
        var dialog = new ContentDialog()
        {
            Title = "Delete File",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Close",
        };
        
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await _fileSystemService.DeleteFileAsync(path);
            _rootNode.SubNodes.Clear();
            BuildDocumentTree();
        }
    }
}
