using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using MatoEditor.Models;
using MatoEditor.Services;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class DocumentTreeViewModel : ViewModelBase
{
    public DocumentTreeViewModel(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
        BuildDocumentTree();
    }
    private readonly IFileSystemService _fileSystemService;

    public ObservableCollection<DocumentTreeNode> DocumentTree { get; set; }

    private async void BuildDocumentTree()
    {
        DocumentTreeNode rootNode = new DocumentTreeNode()
        {
            Name = "articles",
            Path = "E://ohh//pyohh//work//oo//articles",
            IsDirectory = true,
        };
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
