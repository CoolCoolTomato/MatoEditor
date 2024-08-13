using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
}
