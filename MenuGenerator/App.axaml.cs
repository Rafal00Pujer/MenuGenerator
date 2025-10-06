using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using MenuGenerator.Models.Database;
using MenuGenerator.Models.Entities.MenuTemplate;
using MenuGenerator.ViewModel.Allergen;
using MenuGenerator.ViewModel.Dish;
using MenuGenerator.ViewModel.DishAttribute;
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

		MigrateProdDatabase();

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

		var firstTemplate = new DayTemplateEntity
		{
			Id = 1, TemplateDays = TemplateDayFlag.Tuesday | TemplateDayFlag.Friday, Order = 1, Name = "Tuesday, Friday"
		};
		
		var secondTemplate = new DayTemplateEntity
		{
			Id = 2, TemplateDays = TemplateDayFlag.Monday | TemplateDayFlag.Tuesday | TemplateDayFlag.Wednesday, Order = 3, Name = "Monday, Tuesday, Wednesday"
		};
		
		var thirdTemplate = new DayTemplateEntity
		{
			Id = 3, TemplateDays = TemplateDayFlag.Monday | TemplateDayFlag.Friday, Order = 2, Name = "Monday, Friday"
		};

		var menuTemplate = new MenuTemplateEntity
		{
			Id = Guid.Empty, Name = string.Empty
		};

		menuTemplate.TryAddDayTemplate(firstTemplate);
		menuTemplate.TryAddDayTemplate(secondTemplate);
		menuTemplate.TryAddDayTemplate(thirdTemplate);
		
		menuTemplate.ReorderDayTemplates();
	}

	[Conditional("RELEASE")]
	private void MigrateProdDatabase()
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

		collection.AddSingleton<IConfiguration>(_configurationRoot)
				  .AddSingleton<MainWindowViewModel>()
				  .AddSingleton<IMessenger, WeakReferenceMessenger>()
				  .AddScoped<DishTypeEditViewModel>()
				  .AddScoped<DishTypeViewModel>()
				  .AddScoped<AllergenEditViewModel>()
				  .AddScoped<AllergenViewModel>()
				  .AddScoped<DishAttributeEditViewModel>()
				  .AddScoped<DishAttributeViewModel>()
				  .AddScoped<DishEditViewModel>()
				  .AddScoped<DishViewModel>()
				  .AddDbContext<MenuGeneratorContext>
				  (
					  options =>
					  {
						  options.UseSqlite
							  ($"Data Source={_configurationRoot.GetConnectionString("sqliteDbFilePath")}");
					  }
				  )
				  .AddSingleton<IDialogService>
				  (
					  x => new DialogService
					  (
						  new DialogManager
						  (
							  new ViewLocatorBase(),
							  new DialogFactory()
								  .AddMessageBox()
								  .AddDialogHost()
						  ),
						  x.GetService
					  )
				  );

		collection.MakeReadOnly();

		return collection.BuildServiceProvider
		(
			new ServiceProviderOptions
			{
				ValidateOnBuild = true, ValidateScopes = true
			}
		);
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
