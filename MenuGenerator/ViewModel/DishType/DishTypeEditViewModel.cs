using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using MenuGenerator.Models.Database;
using MenuGenerator.Models.Entities.DishType;
using MenuGenerator.ViewLocator;

namespace MenuGenerator.ViewModel.DishType;

[View(typeof(DishTypeEditView))]
public partial class DishTypeEditViewModel(
    MenuGeneratorContext context,
    IDialogService dialogService,
    IMessenger messenger) : ViewModelBase
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLength(500)]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string? _description;

    protected Guid _id = Guid.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private bool _isProcessing;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [Length(3, 50)]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private string? _name;

    public bool IsNew => _id == Guid.Empty;

    public string Title => IsNew ? "New Dish Type" : "Edit Dish Type";

    public async ValueTask LoadAsync(Guid id)
    {
        IsProcessing = true;

        var dishType = await context.DishTypes.FindAsync(id);

        if (dishType is null) throw new InvalidOperationException("Dish type not found.");

        _id = dishType.Id;
        Name = dishType.Name;
        Description = dishType.Description;

        IsProcessing = false;
    }

    public void Clear()
    {
        _id = Guid.Empty;
        Name = null;
        Description = null;
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

        var newDishType = new DishTypeEntity
        {
            Id = Guid.CreateVersion7(),
            Name = Name!,
            Description = Description
        };

        var newDishTypeEntry = await context.DishTypes.AddAsync(newDishType);

        await context.SaveChangesAsync();

        _id = newDishTypeEntry.Entity.Id;
        Name = newDishTypeEntry.Entity.Name;
        Description = newDishTypeEntry.Entity.Description;

        messenger.Send(new DishTypeAddedMessage(newDishType));
        
        _ = await dialogService.ShowMessageBoxAsync(
            this,
            "New dish type successfully added.",
            "Dish Type Added",
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

        var updatedDishType = await context.DishTypes.FindAsync(_id);

        if (updatedDishType is null)
        {
            throw new InvalidOperationException("Dish type not found.");
        }

        updatedDishType.Name = Name!;
        updatedDishType.Description = Description;
        
        await context.SaveChangesAsync();

        messenger.Send(new DishTypeEditedMessage(updatedDishType));
        
        _ = await dialogService.ShowMessageBoxAsync(
            this,
            "Changes to dish type saved successfully.",
            "Dish Type Saved",
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

        var userResponse = await dialogService.ShowMessageBoxAsync(
            this,
            "Are you sure you want to delete this dish type?",
            "Delete Dish Type",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (userResponse is not true)
        {
            IsProcessing = false;

            return;
        }

        var deletedDishType = await context.DishTypes.FindAsync(_id);

        if (deletedDishType is null)
        {
            throw new InvalidOperationException("Dish type not found.");
        }

        context.DishTypes.Remove(deletedDishType);
        await context.SaveChangesAsync();

        _id = Guid.Empty;
        Name = null;
        Description = null;
        
        messenger.Send(new DishTypeDeletedMessage(deletedDishType));
        
        _ = await dialogService.ShowMessageBoxAsync(
            this,
            "Dish type deleted successfully.",
            "Delete Dish Type",
            MessageBoxButton.Ok,
            MessageBoxImage.Information);
        
        IsProcessing = false;
    }
}