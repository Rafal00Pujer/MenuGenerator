using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MenuGenerator.Models.Database;
using MenuGenerator.ViewLocator;
using Microsoft.Extensions.DependencyInjection;

namespace MenuGenerator.ViewModel.DishType;

[View(typeof(DishTypeView))]
public partial class DishTypeViewModel :
    ViewModelBase,
    IRecipient<DishTypeAddedMessage>,
    IRecipient<DishTypeEditedMessage>,
    IRecipient<DishTypeDeletedMessage>,
    IDisposable
{
    private readonly MenuGeneratorContext _context;
    private readonly IMessenger _messenger;
    private readonly IServiceScope _serviceScope;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<int> _dishTypeSummaries = [];
    
    [ObservableProperty]
    private DishTypeEditViewModel? _selectedDishType = null;
    
    public DishTypeViewModel(MenuGeneratorContext context, IMessenger messenger, IServiceScopeFactory serviceScope)
    {
        _context = context;
        _messenger = messenger;
        _serviceScope = serviceScope.CreateScope();
        _serviceProvider = _serviceScope.ServiceProvider;
        
        _messenger.RegisterAll(this);
    }

    /// <summary>
    /// Design time constructor.
    /// </summary>
    protected DishTypeViewModel()
    {
        _context = null!;
        _messenger = null!;
        _serviceScope = null!;
        _serviceProvider = null!;
    }

    public void Receive(DishTypeAddedMessage message)
    {
        throw new NotImplementedException();
    }

    public void Receive(DishTypeEditedMessage message)
    {
        throw new NotImplementedException();
    }

    public void Receive(DishTypeDeletedMessage message)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _messenger.UnregisterAll(this);
        _serviceScope.Dispose();
        
        GC.SuppressFinalize(this);
    }
}