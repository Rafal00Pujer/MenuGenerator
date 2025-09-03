using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using MenuGenerator.Models.Database;
using MenuGenerator.ViewModels;
using MenuGenerator.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MenuGenerator;

public partial class App : Application
{
    private ServiceProvider _serviceProvider = null!;
    private IConfigurationRoot _configurationRoot = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _configurationRoot = BuildConfiguration();
        _serviceProvider = BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };

            desktop.Exit += OnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _serviceProvider.Dispose();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private ServiceProvider BuildServiceProvider()
    {
        var collection = new ServiceCollection();

        collection.AddSingleton<IConfiguration>(_configurationRoot);

        collection.AddSingleton<MainWindowViewModel>();

        collection.AddDbContext<MenuGeneratorContext>(options =>
        {
            options.UseSqlite($"Data Source={_configurationRoot.GetConnectionString("sqlitleDbFilePath")}");
        });

        collection.MakeReadOnly();
        return collection.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");

#if DEVELOPMENT
        builder.AddJsonFile("appsettings.development.json", optional: true);
#endif

        return builder.Build();
    }
}