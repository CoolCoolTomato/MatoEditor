using Markdig;

namespace MatoEditor.ViewModels;

public class EditorViewModel : ViewModelBase
{
    public EditorViewModel()
    {
        md = "# Hello World\nThis is **bold** and this is *italic*.";
        html = Markdown.ToHtml(md);
    }
    public string md { get; set; }
    public string html { get; set; }
}