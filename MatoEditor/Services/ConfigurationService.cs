using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
using ReactiveUI;

namespace MatoEditor.Services;
    
public class ConfigurationService : ReactiveObject
{
    private const string ConfigFileName = "config.json";
    private readonly string _configFilePath;
    private readonly StorageService _storageService;

    public ConfigurationService(StorageService storageService)
    {
        _storageService = storageService;
        _configFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".MatoEditor",
            ConfigFileName
        );
    }

    public void LoadConfiguration()
    {
        if (!File.Exists(_configFilePath))
        {
            CreateDefaultConfig();
        }

        var json = File.ReadAllText(_configFilePath);
        var config = JsonSerializer.Deserialize<EditorConfig>(json);

        if (config != null && !string.IsNullOrEmpty(config.LastOpenedDirectory))
        {
            _storageService.RootDirectoryPath = config.LastOpenedDirectory;
            Application.Current.RequestedThemeVariant =
                config.Theme == "Light" ? ThemeVariant.Light : ThemeVariant.Dark;
        }
    }

    public void SaveConfiguration()
    {
        var config = new EditorConfig
        {
            LastOpenedDirectory = _storageService.RootDirectoryPath,
            Theme = Application.Current.RequestedThemeVariant == ThemeVariant.Light ? "Light" : "Dark"
        };

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath));
        File.WriteAllText(_configFilePath, json);
    }

    private void CreateDefaultConfig()
    {
        var defaultConfig = new EditorConfig
        {
            LastOpenedDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Theme = Application.Current.RequestedThemeVariant == ThemeVariant.Light ? "Light" : "Dark"
        };

        var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath));
        File.WriteAllText(_configFilePath, json);
    }
}

public class EditorConfig
{
    public string LastOpenedDirectory { get; set; }
    public string Theme { get; set; }
}
