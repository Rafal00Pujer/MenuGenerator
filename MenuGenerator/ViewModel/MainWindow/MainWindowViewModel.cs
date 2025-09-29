using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MenuGenerator.ViewLocator;
using MenuGenerator.ViewModel.Allergen;
using MenuGenerator.ViewModel.DishAttribute;
using MenuGenerator.ViewModel.DishType;
using Microsoft.Extensions.DependencyInjection;

namespace MenuGenerator.ViewModel.MainWindow;

[View(typeof(MainWindow))]
public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
	private readonly IServiceScopeFactory _serviceScopeFactory;

#pragma warning disable CS0657 // Not a valid attribute location for this declaration
	[MaybeNull]
	[property: MaybeNull]
	[ObservableProperty]
#pragma warning restore CS0657 // Not a valid attribute location for this declaration
	private IMainPage _currentMainPage = null!;

	[MaybeNull]
	private IServiceScope _currentMainPageScope;

	[ObservableProperty]
	private Tab _selectedTab;

	public MainWindowViewModel(IServiceScopeFactory serviceScopeFactory)
	{
		_serviceScopeFactory = serviceScopeFactory;

		SelectedTab = Tabs.First();
	}

	public List<Tab> Tabs { get; set; } =
	[
		//new ("Menu History", typeof(object)),
		//new ("Menu Templates", typeof(object)),
		//new ("Dishes", typeof(object)),
		new("Dish Types", typeof(DishTypeViewModel)),
		new("Dishes Attributes", typeof(DishAttributeViewModel)),
		new("Allergens", typeof(AllergenViewModel))

		//new ("Occurence Rules", typeof(object))
	];

	public void Dispose()
	{
		_currentMainPageScope?.Dispose();

		GC.SuppressFinalize(this);
	}

	partial void OnSelectedTabChanged(Tab value)
	{
		// don't change the page if it is processing something
		if (CurrentMainPage is not null
			&& CurrentMainPage.IsProcessing)
			return;

		_currentMainPageScope?.Dispose();
		_currentMainPageScope = _serviceScopeFactory.CreateScope();

		CurrentMainPage = (IMainPage)_currentMainPageScope
									 .ServiceProvider
									 .GetRequiredService(value.ViewModelType);

		CurrentMainPage.LoadAsync().Wait();
	}

	public record struct Tab(string Name, Type ViewModelType);
}
