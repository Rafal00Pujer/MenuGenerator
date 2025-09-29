using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MenuGenerator.Models.Database;
using MenuGenerator.ViewLocator;
using Microsoft.Extensions.DependencyInjection;

namespace MenuGenerator.ViewModel.Allergen;

[View(typeof(AllergenView))]
public partial class AllergenViewModel :
    ViewModelBase,
    IRecipient<AllergenAddedMessage>,
    IRecipient<AllergenEditedMessage>,
    IRecipient<AllergenDeletedMessage>,
    IMainPage,
    IDisposable
{
    private readonly MenuGeneratorContext _context;
    private readonly IMessenger _messenger;
    private readonly IServiceScope _serviceScope;

    [ObservableProperty] private AllergenEditViewModel _allergen;

    [ObservableProperty] private ObservableCollection<AllergenSummary> _allergenSummaries = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddNewCommand))]
    [NotifyCanExecuteChangedFor(nameof(SelectCommand))]
    private bool _isProcessing;

    private int _isProcessingCounter;

    public AllergenViewModel(MenuGeneratorContext context, IMessenger messenger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _context = context;
        _messenger = messenger;
        _serviceScope = serviceScopeFactory.CreateScope();

        _messenger.RegisterAll(this);

        Allergen = _serviceScope.ServiceProvider.GetRequiredService<AllergenEditViewModel>();
        Allergen.PropertyChanged += OnAllergenPropertyChanged;
    }

    public void Dispose()
    {
        _messenger.UnregisterAll(this);

        Allergen.PropertyChanged -= OnAllergenPropertyChanged;

        _serviceScope.Dispose();

        GC.SuppressFinalize(this);
    }

    public async Task LoadAsync()
    {
        IncrementIsProcessingCounter();

        AllergenSummaries.Clear();

        await foreach (var allergen in _context.Allergens)
        {
            var summary = new AllergenSummary(allergen.Id, allergen.DisplayId);
            AllergenSummaries.Add(summary);
        }

        DecrementIsProcessingCounter();
    }

    public void Receive(AllergenAddedMessage message)
    {
        var addedAllergenSummary = new AllergenSummary(message.Id, message.DisplayId);

        AllergenSummaries.Add(addedAllergenSummary);
    }

    public void Receive(AllergenDeletedMessage message)
    {
        var deletedAllergenSummary = AllergenSummaries.FirstOrDefault(x => x.Id == message.Id);

        if (deletedAllergenSummary is null) throw new InvalidOperationException("Allergen not found!");

        AllergenSummaries.Remove(deletedAllergenSummary);
    }

    public void Receive(AllergenEditedMessage message)
    {
        var editedAllergenSummary = AllergenSummaries.FirstOrDefault(x => x.Id == message.Id);

        if (editedAllergenSummary is null) throw new InvalidOperationException("Allergen not found!");

        var editedDishTypeSummaryIndex = AllergenSummaries.IndexOf(editedAllergenSummary);
        AllergenSummaries.Remove(editedAllergenSummary);

        editedAllergenSummary = editedAllergenSummary with
        {
            DisplayId = message.DisplayId
        };

        AllergenSummaries.Add(editedAllergenSummary);
        AllergenSummaries.Move(AllergenSummaries.Count - 1, editedDishTypeSummaryIndex);
    }

    private bool CanAddNew()
    {
        return !IsProcessing;
    }

    [RelayCommand(CanExecute = nameof(CanAddNew))]
    private void AddNew()
    {
        Allergen.Clear();
    }

    private bool CanSelect()
    {
        return !IsProcessing;
    }

    [RelayCommand(CanExecute = nameof(CanSelect))]
    private async Task Select(Guid id)
    {
        IncrementIsProcessingCounter();

        await Allergen.LoadAsync(id);

        DecrementIsProcessingCounter();
    }

    private void OnAllergenPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(AllergenEditViewModel.IsProcessing)
            || sender is not AllergenEditViewModel allergen) return;

        if (allergen.IsProcessing)
        {
            IncrementIsProcessingCounter();
            return;
        }

        DecrementIsProcessingCounter();
    }

    private void IncrementIsProcessingCounter()
    {
        _isProcessingCounter++;
        RecalculateIsProcessing();
    }

    private void DecrementIsProcessingCounter()
    {
        if (_isProcessingCounter <= 0)
            throw new InvalidOperationException($"{nameof(_isProcessingCounter)} can not be negative!");

        _isProcessingCounter--;
        RecalculateIsProcessing();
    }

    private void RecalculateIsProcessing()
    {
        switch (_isProcessingCounter)
        {
            case < 0:
                throw new InvalidOperationException($"{nameof(_isProcessingCounter)} is negative!");
            case > 0:
                IsProcessing = true;
                return;
            default:
                IsProcessing = false;
                break;
        }
    }

    public record AllergenSummary(Guid Id, string DisplayId);
}