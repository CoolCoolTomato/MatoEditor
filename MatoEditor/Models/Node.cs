using System.Collections.ObjectModel;
using ReactiveUI;

namespace MatoEditor.Models;

public class Node : ReactiveObject
{
    public Node()
    {
        Name = "";
        Path = "";
        IsDirectory = false;
        SubNodes = new ObservableCollection<Node>();
    }
    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    private string _path;
    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }
    private bool _isDirectory;
    public bool IsDirectory
    {
        get => _isDirectory;
        set => this.RaiseAndSetIfChanged(ref _isDirectory, value);
    }
    private ObservableCollection<Node> _subNodes;
    public ObservableCollection<Node> SubNodes
    {
        get => _subNodes;
        set => this.RaiseAndSetIfChanged(ref _subNodes, value);
    }
}