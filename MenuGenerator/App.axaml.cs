using System.Diagnostics;
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
using MenuGenerator.ViewModel.DishType;
using MenuGenerator.ViewModel.MainWindow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MenuGenerator;

public class App : Application
{
    private IConfigurationRoot _configurationRoot = null!;
    private ServiceProvider _serviceProviderRoot = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _configurationRoot = BuildConfiguration();
        _serviceProviderRoot = BuildServiceProvider();

        MigrateDatabase();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProviderRoot.GetRequiredService<MainWindowViewModel>()
            };

            desktop.Exit += OnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    [Conditional("RELEASE")]
    private void MigrateDatabase()
    {
        using var scope = _serviceProviderRoot.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<MenuGeneratorContext>();
        context.Database.Migrate();
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _serviceProviderRoot.Dispose();
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
        collection.AddSingleton<IMessenger, WeakReferenceMessenger>();

        collection.AddScoped<DishTypeEditViewModel>();
        collection.AddScoped<DishTypeViewModel>();

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