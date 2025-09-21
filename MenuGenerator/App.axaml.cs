using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;
using MenuGenerator.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MainWindow = MenuGenerator.ViewModel.MainWindow.MainWindow;
using MainWindowViewModel = MenuGenerator.ViewModel.MainWindow.MainWindowViewModel;

namespace MenuGenerator;

public class App : Application
{
    private IConfigurationRoot _configurationRoot = null!;
    private ServiceProvider _serviceProvider = null!;

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
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>()
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
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
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

        collection.AddSingleton<IDialogService>(x => new DialogService(
            new DialogManager(
                new ViewLocatorBase(),
                new DialogFactory()
                    .AddMessageBox(MessageBoxMode.Popup)
                    .AddDialogHost()),
            x.GetService));

        collection.AddSingleton<IMessenger, WeakReferenceMessenger>();

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
        builder.AddJsonFile("appsettings.development.json", true);
#endif

        return builder.Build();
    }
}