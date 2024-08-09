using System;
using Markdig;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class EditorViewModel : ViewModelBase
{
    public EditorViewModel()
    {
        ContentString = "";
        ContentHtml = "";
        this.WhenAnyValue(x => x.ContentString).Subscribe(_ => ConvertMarkdown());
    }

    private string _contentString;
    private string _contentHtml;

    public string ContentString
    {
        get => _contentString;
        set => this.RaiseAndSetIfChanged(ref _contentString, value);
    }

    public string ContentHtml
    {
        get => _contentHtml;
        set => this.RaiseAndSetIfChanged(ref _contentHtml, value);
    }

    private void ConvertMarkdown()
    {
        ContentHtml = ContentString == "" ? "<br/>" : Markdown.ToHtml(ContentString);
    }
}