using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MenuGenerator.ViewLocator;
using MenuGenerator.ViewModel.DishType;
using Microsoft.Extensions.DependencyInjection;

namespace MenuGenerator.ViewModel.MainWindow;

[View(typeof(MainWindow))]
public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    [ObservableProperty] private IMainPage _currentMainPage = null!;

    [MaybeNull] private IServiceScope _currentMainPageScope;

    [ObservableProperty] private Tab _selectedTab;

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
        new("Dish Types", typeof(DishTypeViewModel))
        //new ("Dishes Attributes", typeof(object)),
        //new ("Allergens", typeof(object)),
        //new ("Occurence Rules", typeof(object))
    ];

    public void Dispose()
    {
        _currentMainPageScope?.Dispose();

        GC.SuppressFinalize(this);
    }

    partial void OnSelectedTabChanged(Tab value)
    {
        _currentMainPageScope?.Dispose();
        _currentMainPageScope = _serviceScopeFactory.CreateScope();

        CurrentMainPage = (IMainPage)_currentMainPageScope
            .ServiceProvider
            .GetRequiredService(value.ViewModelType);

        CurrentMainPage.LoadAsync().Wait();
    }

    public record struct Tab(string Name, Type ViewModelType);
}