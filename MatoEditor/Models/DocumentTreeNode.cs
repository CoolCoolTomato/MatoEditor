using System.Collections.ObjectModel;
using System.Linq;

namespace MatoEditor.Models;

public class DocumentTreeNode
{
    public DocumentTreeNode()
    {
        Name = "";
        Path = "";
        IsDirectory = false;
        SubNodes = new ObservableCollection<DocumentTreeNode>();
    }
    public string Name { get; set; }
    public string Path { get; set; }
    public bool IsDirectory { get; set; }
    public ObservableCollection<DocumentTreeNode> SubNodes { get; set; }
}