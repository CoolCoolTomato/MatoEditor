using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using MatoEditor.Services;
using MatoEditor.ViewModels;
using MatoEditor.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MatoEditor;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public IServiceProvider? ServiceProvider { get; private set; }
    
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);
            
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            
            var configService = ServiceProvider.GetRequiredService<ConfigurationService>();
            configService.LoadConfiguration();
            
            var mainWindow = new MainWindow();
            var viewModel = ActivatorUtilities.CreateInstance<MainWindowViewModel>(
                ServiceProvider, 
                ServiceProvider.GetRequiredService<IFileSystemService>(),
                ServiceProvider.GetRequiredService<StorageService>(),
                mainWindow
            );
            mainWindow.DataContext = viewModel;
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IFileSystemService, FileSystemService>();
        services.AddSingleton<StorageService>();
        services.AddSingleton<ConfigurationService>();
        services.AddTransient<MainWindowViewModel>();
    }
}