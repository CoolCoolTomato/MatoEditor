using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using MatoEditor.Dialogs;
using MatoEditor.Models;
using MatoEditor.Services;
using ReactiveUI;
using Ursa.Controls;

namespace MatoEditor.ViewModels;

public class DocumentTreeViewModel : ViewModelBase
{
    public DocumentTreeViewModel(IFileSystemService fileSystemService, StorageService storageService)
    {
        _fileSystemService = fileSystemService;
        _storageService = storageService;
        
        _rootNode = new Node();
        _storageService.WhenAnyValue(x => x.RootDirectoryPath)
            .Subscribe(RootDirectoryPath =>
            {
                _rootNode.Name = Path.GetFileName(RootDirectoryPath);
                _rootNode.Path = RootDirectoryPath;
                _rootNode.SubNodes.Clear();
                _rootNode.IsDirectory = true;
                InitDocumentTree();
            });
        SelectedNode = new Node();
        
        OpenCreateDirectoryDialogCommand = ReactiveCommand.Create<string>(OpenCreateDirectoryDialog);
        OpenRenameDirectoryDialogCommand = ReactiveCommand.Create<Node>(OpenRenameDirectoryDialog);
        OpenDeleteDirectoryDialogCommand = ReactiveCommand.Create<string>(OpenDeleteDirectoryDialog);
        OpenCreateFileDialogCommand = ReactiveCommand.Create<string>(OpenCreateFileDialog);
        OpenRenameFileDialogCommand = ReactiveCommand.Create<Node>(OpenRenameFileDialog);
        OpenDeleteFileDialogCommand = ReactiveCommand.Create<string>(OpenDeleteFileDialog);

        this.WhenAnyValue(x => x.SelectedNode.Path)
            .Subscribe(_ => ChangeSelectedFile());
    }
    private readonly IFileSystemService _fileSystemService;
    private StorageService _storageService;
    
    private Node _rootNode { get; set; }
    
    private ObservableCollection<Node> _documentTree;
    public ObservableCollection<Node> DocumentTree
    {
        get => _documentTree;
        set => this.RaiseAndSetIfChanged(ref _documentTree, value);
    }
    
    private Node _selectedNode;
    public Node SelectedNode
    {
        get => _selectedNode;
        set => this.RaiseAndSetIfChanged(ref _selectedNode, value);
    }
    
    public ICommand OpenCreateDirectoryDialogCommand { get; }
    public ICommand OpenRenameDirectoryDialogCommand { get; }
    public ICommand OpenDeleteDirectoryDialogCommand { get; }
    public ICommand OpenCreateFileDialogCommand { get; }
    public ICommand OpenRenameFileDialogCommand { get; }
    public ICommand OpenDeleteFileDialogCommand { get; }

    private void InitDocumentTree()
    {
        BuildDocumentTree(_rootNode);
        DocumentTree = new ObservableCollection<Node>() { _rootNode };
    }
    private async void BuildDocumentTree(Node node)
    {
        IEnumerable<DirectoryInfo> subDirectoryInfos = await _fileSystemService.GetSubDirectories(node.Path);
        foreach (var subDirectoryInfo in subDirectoryInfos)
        {
            var subDirectory = new Node()
            {
                Name = subDirectoryInfo.Name,
                Path = subDirectoryInfo.FullName,
                IsDirectory = true,
            };
            node.SubNodes.Add(subDirectory);
            BuildDocumentTree(subDirectory);
        }
        
        IEnumerable<FileInfo> fileInfos = await _fileSystemService.GetFiles(node.Path);
        foreach (var fileInfo in fileInfos)
        {
            var file = new Node()
            {
                Name = fileInfo.Name,
                Path = fileInfo.FullName,
                IsDirectory = false,
            };
            node.SubNodes.Add(file);
        }
    }

    private void ChangeSelectedFile()
    {
        if (!SelectedNode.IsDirectory)
        {
            _storageService.CurrentFilePath = SelectedNode.Path;
        }
    }
    
    private Node FindNodeByPath(Node currentNode, string path)
    {
        if (currentNode.Path == path)
        {
            return currentNode;
        }

        foreach (var subNode in currentNode.SubNodes)
        {
            var result = FindNodeByPath(subNode, path);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
    private void InsertNode(Node currentNode, Node node)
    {
        int insertIndex = -1;
        for (int i = 0; i < currentNode.SubNodes.Count; i++)
        {
            if (currentNode.SubNodes[i].IsDirectory == node.IsDirectory)
            {
                if (string.Compare(currentNode.SubNodes[i].Name, node.Name, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    insertIndex = i;
                    break;
                }
            }

            if (!currentNode.SubNodes[i].IsDirectory && node.IsDirectory)
            {
                insertIndex = i;
                break;
            }
        }
        if (insertIndex == -1)
        {
            currentNode.SubNodes.Add(node);
        }
        else
        {
            currentNode.SubNodes.Insert(insertIndex, node);
        }
    }
    private bool DeleteNodeByPath(Node currentNode, string path)
    {
        for (int i = 0; i < currentNode.SubNodes.Count; i++)
        {
            var subNode = currentNode.SubNodes[i];
            if (subNode.Path == path)
            {
                currentNode.SubNodes.RemoveAt(i);
                return true;
            }
            
            var result = DeleteNodeByPath(subNode, path);
            if (result)
            {
                return true;
            }
        }

        return false;
    }
    
    private async void OpenCreateDirectoryDialog(string path)
    {
        var options = new DialogOptions()
        {
            Title = "Input directory name",
            Mode = DialogMode.None,
            Button = DialogButton.OKCancel,
            ShowInTaskBar = false,
            IsCloseButtonVisible = true,
            StartupLocation = WindowStartupLocation.CenterOwner,
        };
        var textBoxDialogViewModel = new TextBoxDialogViewModel();
        var result  = await Dialog.ShowModal<TextBoxDialog, TextBoxDialogViewModel>(textBoxDialogViewModel, options: options);
        if (result == DialogResult.OK)
        {
            var currentNode = FindNodeByPath(_rootNode, path);
            if (currentNode != null)
            {
                var directory = new Node()
                {
                    Name = textBoxDialogViewModel.Content,
                    Path = path + "/" + textBoxDialogViewModel.Content,
                    IsDirectory = true,
                };
                InsertNode(currentNode, directory);
                await _fileSystemService.CreateDirectoryAsync(directory.Path);
            }
        }
    }
    private async void OpenRenameDirectoryDialog(Node node)
    {
        var options = new DialogOptions()
        {
            Title = "Input new directory name",
            Mode = DialogMode.None,
            Button = DialogButton.OKCancel,
            ShowInTaskBar = false,
            IsCloseButtonVisible = true,
            StartupLocation = WindowStartupLocation.CenterOwner,
        };
        var textBoxDialogViewModel = new TextBoxDialogViewModel()
        {
            Content = node.Name,
        };
        var result  = await Dialog.ShowModal<TextBoxDialog, TextBoxDialogViewModel>(textBoxDialogViewModel, options: options);
        if (result == DialogResult.OK)
        {
            var currentNode = FindNodeByPath(_rootNode, node.Path);
            if (currentNode != null)
            {
                currentNode.Name = textBoxDialogViewModel.Content;
                currentNode.Path = Path.GetDirectoryName(node.Path) + "/" + textBoxDialogViewModel.Content;
                await _fileSystemService.RenameDirectoryAsync(node.Path, currentNode.Path);
            }
        }
    }
    private async void OpenDeleteDirectoryDialog(string path)
    {
        var options = new DialogOptions()
        {
            Title = "Are you sure you want to delete the selected directory?",
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
            var currentNode = FindNodeByPath(_rootNode, path);
            if (currentNode != null)
            {
                if (DeleteNodeByPath(_rootNode, path))
                {
                    await _fileSystemService.DeleteDirectoryAsync(path);
                }
            }
        }
    }
    private async void OpenCreateFileDialog(string path)
    {
        var options = new DialogOptions()
        {
            Title = "Input file name",
            Mode = DialogMode.None,
            Button = DialogButton.OKCancel,
            ShowInTaskBar = false,
            IsCloseButtonVisible = true,
            StartupLocation = WindowStartupLocation.CenterOwner,
        };
        var textBoxDialogViewModel = new TextBoxDialogViewModel();
        var result  = await Dialog.ShowModal<TextBoxDialog, TextBoxDialogViewModel>(textBoxDialogViewModel, options: options);
        if (result == DialogResult.OK)
        {
            var currentNode = FindNodeByPath(_rootNode, path);
            if (currentNode != null)
            {
                var file = new Node()
                {
                    Name = textBoxDialogViewModel.Content,
                    Path = path + "/" + textBoxDialogViewModel.Content,
                    IsDirectory = false,
                };
                InsertNode(currentNode, file);
                await _fileSystemService.CreateFileAsync(file.Path);   
            }
        }
    }
    private async void OpenRenameFileDialog(Node node)
    {
        var options = new DialogOptions()
        {
            Title = "Input new file name",
            Mode = DialogMode.None,
            Button = DialogButton.OKCancel,
            ShowInTaskBar = false,
            IsCloseButtonVisible = true,
            StartupLocation = WindowStartupLocation.CenterOwner,
        };
        var textBoxDialogViewModel = new TextBoxDialogViewModel()
        {
            Content = node.Name,
        };
        var result  = await Dialog.ShowModal<TextBoxDialog, TextBoxDialogViewModel>(textBoxDialogViewModel, options: options);
        if (result == DialogResult.OK)
        {
            var currentNode = FindNodeByPath(_rootNode, node.Path);
            if (currentNode != null)
            {
                currentNode.Name = textBoxDialogViewModel.Content;
                currentNode.Path = Path.GetDirectoryName(node.Path) + "/" + textBoxDialogViewModel.Content;
                await _fileSystemService.RenameDirectoryAsync(node.Path, currentNode.Path);
            }
        }
    }
    private async void OpenDeleteFileDialog(string path)
    {
        var options = new DialogOptions()
        {
            Title = "Are you sure you want to delete the selected file?",
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
            var currentNode = FindNodeByPath(_rootNode, path);
            if (currentNode != null)
            {
                if (DeleteNodeByPath(_rootNode, path))
                {
                    await _fileSystemService.DeleteFileAsync(path);
                }
            }
        }
    }
}