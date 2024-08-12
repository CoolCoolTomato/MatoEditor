using ReactiveUI;

namespace MatoEditor.Services;

public class StorageService : ReactiveObject
{
    private string _rootDirectoryPath;
    public string RootDirectoryPath
    {
        get => _rootDirectoryPath;
        set => this.RaiseAndSetIfChanged(ref _rootDirectoryPath, value);
    }

    private string _currentFilePath;
    public string CurrentFilePath
    {
        get => _currentFilePath;
        set => this.RaiseAndSetIfChanged(ref _currentFilePath, value);
    }
}
