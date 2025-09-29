using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using MenuGenerator.Models.Database;
using MenuGenerator.Models.Entities.Allergen;
using MenuGenerator.ViewLocator;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.ViewModel.Allergen;

[View(typeof(AllergenEditView))]
public partial class AllergenEditViewModel : ViewModelBase
{
    private readonly MenuGeneratorContext _context;
    private readonly IDialogService _dialogService;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLength(50)]
    [NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand))]
    private string? _description;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [Length(3, 10)]
    [NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand))]
    private string? _displayId;

    private Guid _id = Guid.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand), nameof(CancelCommand), nameof(DeleteCommand))]
    private bool _isNew;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand), nameof(CancelCommand), nameof(DeleteCommand))]
    private bool _isProcessing;

    [ObservableProperty] private string _title = null!;

    public AllergenEditViewModel(MenuGeneratorContext context,
        IDialogService dialogService,
        IMessenger messenger)
    {
        _context = context;
        _dialogService = dialogService;
        _messenger = messenger;

        UpdateIsNewAndTitle();
    }

    public async ValueTask LoadAsync(Guid id)
    {
        IsProcessing = true;

        var allergen = await _context.Allergens.FindAsync(id);

        if (allergen is null) throw new InvalidOperationException("Allergen not found.");

        _id = allergen.Id;
        DisplayId = allergen.DisplayId;
        Description = allergen.Description;

        UpdateIsNewAndTitle();

        IsProcessing = false;
    }

    public void Clear()
    {
        _id = Guid.Empty;
        DisplayId = null;
        Description = null;

        UpdateIsNewAndTitle();
    }

    private void UpdateIsNewAndTitle()
    {
        IsNew = _id == Guid.Empty;
        Title = IsNew ? "Add New Allergen" : "Edit " + DisplayId;
    }

    private bool CanAdd()
    {
        if (!IsNew || IsProcessing) return false;

        ValidateAllProperties();

        return !HasErrors;
    }

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private async Task Add()
    {
        IsProcessing = true;

        if (await ShowMessageIfDisplayIdAlreadyExists())
        {
            IsProcessing = false;

            return;
        }

        var newAllergen = new AllergenEntity
        {
            Id = Guid.CreateVersion7(),
            DisplayId = DisplayId!,
            Description = Description
        };

        var newAllergenEntry = await _context.Allergens.AddAsync(newAllergen);
        newAllergen = newAllergenEntry.Entity;

        await _context.SaveChangesAsync();

        _id = newAllergen.Id;
        DisplayId = newAllergen.DisplayId;
        Description = newAllergen.Description;

        UpdateIsNewAndTitle();

        _messenger.Send(AllergenAddedMessage.CreateFromEntity(newAllergen));

        _ = await _dialogService.ShowMessageBoxAsync(
            null,
            "New Allergen successfully added.",
            "Allergen Added",
            MessageBoxButton.Ok,
            MessageBoxImage.Information);

        IsProcessing = false;
    }

    private bool CanCancel()
    {
        return !IsNew && !IsProcessing;
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private async Task Cancel()
    {
        await LoadAsync(_id);
    }

    private bool CanSave()
    {
        if (IsNew || IsProcessing) return false;

        ValidateAllProperties();

        return !HasErrors;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        IsProcessing = true;

        var updatedAllergen = await _context.Allergens.FindAsync(_id);

        if (updatedAllergen is null) throw new InvalidOperationException("Allergen not found.");

        // check if a new name already exists
        if (updatedAllergen.DisplayId != DisplayId
            && await ShowMessageIfDisplayIdAlreadyExists())
        {
            IsProcessing = false;

            return;
        }

        updatedAllergen.DisplayId = DisplayId!;
        updatedAllergen.Description = Description;

        UpdateIsNewAndTitle();

        await _context.SaveChangesAsync();

        _messenger.Send(AllergenEditedMessage.CreateFromEntity(updatedAllergen));

        _ = await _dialogService.ShowMessageBoxAsync(
            null,
            "Changes to allergen saved successfully.",
            "Allergen Saved",
            MessageBoxButton.Ok,
            MessageBoxImage.Information);

        IsProcessing = false;
    }

    private bool CanDelete()
    {
        return !IsNew && !IsProcessing;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task Delete()
    {
        IsProcessing = true;

        var userResponse = await _dialogService.ShowMessageBoxAsync(
            null,
            "Are you sure you want to delete this allergen?",
            "Delete Allergen",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (userResponse is not true)
        {
            IsProcessing = false;

            return;
        }

        var deletedAllergen = await _context.Allergens.FindAsync(_id);

        if (deletedAllergen is null) throw new InvalidOperationException("Allergen not found.");

        _context.Allergens.Remove(deletedAllergen);
        await _context.SaveChangesAsync();

        _id = Guid.Empty;
        DisplayId = null;
        Description = null;

        UpdateIsNewAndTitle();

        _messenger.Send(AllergenDeletedMessage.CreateFromEntity(deletedAllergen));

        _ = await _dialogService.ShowMessageBoxAsync(
            null,
            "Allergen deleted successfully.",
            "Delete Allergen",
            MessageBoxButton.Ok,
            MessageBoxImage.Information);

        IsProcessing = false;
    }

    private async Task<bool> ShowMessageIfDisplayIdAlreadyExists()
    {
        if (!await _context.Allergens.AnyAsync(x => x.DisplayId == DisplayId)) return false;

        _ = await _dialogService.ShowMessageBoxAsync(
            null,
            $"Allergen with display id: \"{DisplayId}\" already exists.",
            "Allergen Exists",
            MessageBoxButton.Ok,
            MessageBoxImage.Error);

        return true;
    }
}