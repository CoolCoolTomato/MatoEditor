using ReactiveUI;

namespace MatoEditor.Models;

public class FileTab : ReactiveObject
{
    public FileTab()
    {
        Name = "";
        Path = "";
        OldContentString = "";
        NewContentString = "";
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

    private string _oldContentString;
    public string OldContentString
    {
        get => _oldContentString;
        set => this.RaiseAndSetIfChanged(ref _oldContentString, value);
    }
    
    private string _newContentString;
    public string NewContentString
    {
        get => _newContentString;
        set => this.RaiseAndSetIfChanged(ref _newContentString, value);
    }
}