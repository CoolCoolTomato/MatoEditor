using System;
using Markdig;
using MatoEditor.Services;
using ReactiveUI;

namespace MatoEditor.ViewModels;

public class EditorViewModel : ViewModelBase
{
    public EditorViewModel(IFileSystemService fileSystemService, StorageService storageService)
    {
        _fileSystemService = fileSystemService;
        _storageService = storageService;
        ContentString = "";
        ContentHtml = "";
        _storageService.WhenAnyValue(x => x.CurrentFilePath)
            .Subscribe(CurrentFilePath =>
            {
                UpdateContentString(CurrentFilePath);
            });
        this.WhenAnyValue(x => x.ContentString).Subscribe(_ =>
        {
            ConvertMarkdown();
            SaveFile();
        });
    }
    
    private readonly IFileSystemService _fileSystemService;
    private StorageService _storageService;
    
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

    private async void UpdateContentString(string filePath)
    {
        ContentString = await _fileSystemService.ReadFileAsync(filePath);
    }

    private async void SaveFile()
    {
        _ = await _fileSystemService.WriteFileAsync(_storageService.CurrentFilePath, ContentString);
    }
}