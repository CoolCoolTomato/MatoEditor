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
        RootNode = new DocumentTreeNode
        {
            Name = "Artices"
        };
        _storageService.WhenAnyValue(x => x.RootDirectoryPath).Subscribe(RootDirectoryPath => RootNode.Path = RootDirectoryPath);
        BuildDocumentTree();
    }
    private readonly IFileSystemService _fileSystemService;
    private StorageService _storageService;

    private DocumentTreeNode _rootNode;
    public DocumentTreeNode RootNode
    {
        get => _rootNode;
        set => this.RaiseAndSetIfChanged(ref _rootNode, value);
    }
    
    public ObservableCollection<DocumentTreeNode> DocumentTree { get; set; }

    private async void BuildDocumentTree()
    {
        DocumentTreeNode rootNode = RootNode;
        BuildDocumentNode(rootNode);
        DocumentTree = new ObservableCollection<DocumentTreeNode>() { rootNode };
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
}
