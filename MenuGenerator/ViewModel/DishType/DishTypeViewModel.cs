using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty] private ObservableCollection<DishTypeSummary> _dishTypeSummaries = [];

    [ObservableProperty] private DishTypeEditViewModel? _dishType;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddNewCommand))]
    [NotifyCanExecuteChangedFor(nameof(SelectCommand))]
    private bool _isProcessing;

    private bool _selectedDishTypeIsProcessing;

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

    public async Task InitializeAsync()
    {
        SetIsProcessingWithSelectedDishType(true);

        if (DishType is null)
        {
            DishType = _serviceProvider.GetRequiredService<DishTypeEditViewModel>();
            DishType.PropertyChanged += OnSelectedDishTypeOnPropertyChanged;
        }

        if (DishTypeSummaries.Count == 0)
        {
            await foreach (var dishType in _context.DishTypes)
            {
                var summary = new DishTypeSummary(dishType.Id, dishType.Name);
                DishTypeSummaries.Add(summary);
            }
        }

        SetIsProcessingWithSelectedDishType(false);
    }

    public void Receive(DishTypeAddedMessage message)
    {
        var addedDishType = message.Value;

        var addedDishTypeSummary = new DishTypeSummary(addedDishType.Id, addedDishType.Name);

        DishTypeSummaries.Add(addedDishTypeSummary);
    }

    public void Receive(DishTypeEditedMessage message)
    {
        var editedDishType = message.Value;

        var editedDishTypeSummary = DishTypeSummaries.FirstOrDefault(x => x.Id == editedDishType.Id);

        if (editedDishTypeSummary is null)
        {
            throw new InvalidOperationException("Dish Type not found!");
        }

        var editedDishTypeSummaryIndex = DishTypeSummaries.IndexOf(editedDishTypeSummary);
        DishTypeSummaries.Remove(editedDishTypeSummary);

        editedDishTypeSummary = editedDishTypeSummary with
        {
            Name = editedDishType.Name
        };

        DishTypeSummaries.Add(editedDishTypeSummary);
        DishTypeSummaries.Move(DishTypeSummaries.Count - 1, editedDishTypeSummaryIndex);
    }

    public void Receive(DishTypeDeletedMessage message)
    {
        var deletedDishType = message.Value;

        var deletedDishTypeSummary = DishTypeSummaries.FirstOrDefault(x => x.Id == deletedDishType.Id);

        if (deletedDishTypeSummary is null)
        {
            throw new InvalidOperationException("Dish Type not found!");
        }

        DishTypeSummaries.Remove(deletedDishTypeSummary);
    }

    private bool CanAddNew() => !IsProcessing;

    [RelayCommand(CanExecute = nameof(CanAddNew))]
    private void AddNew()
    {
        ThrowIfNotInitialized();

        DishType.Clear();
    }

    private bool CanSelect() => !IsProcessing;

    [RelayCommand(CanExecute = nameof(CanSelect))]
    private async Task Select(Guid id)
    {
        ThrowIfNotInitialized();

        await DishType.LoadAsync(id);
    }

    private void OnSelectedDishTypeOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (DishType is null || args.PropertyName != nameof(DishTypeEditViewModel.IsProcessing))
        {
            return;
        }

        _selectedDishTypeIsProcessing = DishType.IsProcessing;
        SetIsProcessingWithSelectedDishType(IsProcessing);
    }

    private void SetIsProcessingWithSelectedDishType(bool isProcessing) =>
        IsProcessing = isProcessing || _selectedDishTypeIsProcessing;

    [MemberNotNull(nameof(DishType))]
#pragma warning disable MVVMTK0034
    [MemberNotNull(nameof(_dishType))]
#pragma warning restore MVVMTK0034
    private void ThrowIfNotInitialized()
    {
        if (DishType is null)
        {
            throw new InvalidOperationException("View model is not initialized!");
        }
    }

    public void Dispose()
    {
        _messenger.UnregisterAll(this);

        if (DishType is not null)
        {
            DishType.PropertyChanged -= OnSelectedDishTypeOnPropertyChanged;
        }

        _serviceScope.Dispose();

        GC.SuppressFinalize(this);
    }

    public record DishTypeSummary(Guid Id, string Name);
}