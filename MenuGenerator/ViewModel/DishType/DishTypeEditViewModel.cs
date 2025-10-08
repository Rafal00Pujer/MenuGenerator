using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using MenuGenerator.Models.Database;
using MenuGenerator.Models.Entities.DishType;
using MenuGenerator.ViewLocator;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.ViewModel.DishType;

[View(typeof(DishTypeEditView))]
public partial class DishTypeEditViewModel : ViewModelBase
{
	private readonly MenuGeneratorContext _context;
	private readonly IDialogService _dialogService;
	private readonly IMessenger _messenger;

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[MaxLength(500)]
	[NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand))]
	private string? _description;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand), nameof(CancelCommand), nameof(DeleteCommand))]
	private bool _isNew;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand), nameof(CancelCommand), nameof(DeleteCommand))]
	private bool _isProcessing;

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[Required]
	[Length(3, 50)]
	[NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand))]
	private string? _name;

	[ObservableProperty]
	private string _title = null!;

	protected Guid Id = Guid.Empty;

	public DishTypeEditViewModel
	(
		MenuGeneratorContext context,
		IDialogService dialogService,
		IMessenger messenger
	)
	{
		_context = context;
		_dialogService = dialogService;
		_messenger = messenger;

		UpdateIsNewAndTitle();
	}

	public async ValueTask LoadAsync(Guid id)
	{
		IsProcessing = true;

		var dishType = await _context.DishTypes.FindAsync(id);

		if (dishType is null) throw new InvalidOperationException("Dish type not found.");

		Id = dishType.Id;
		Name = dishType.Name;
		Description = dishType.Description;

		UpdateIsNewAndTitle();

		IsProcessing = false;
	}

	public void Clear()
	{
		Id = Guid.Empty;
		Name = null;
		Description = null;

		UpdateIsNewAndTitle();
	}

	private void UpdateIsNewAndTitle()
	{
		IsNew = Id == Guid.Empty;
		Title = IsNew ? "Add New Dish Type" : "Edit " + Name;
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

		if (await ShowMessageIfNameAlreadyExists())
		{
			IsProcessing = false;

			return;
		}

		var newDishType = new DishTypeEntity
		{
			Id = Guid.CreateVersion7(), Name = Name!, Description = Description
		};

		var newDishTypeEntry = await _context.DishTypes.AddAsync(newDishType);

		await _context.SaveChangesAsync();

		Id = newDishTypeEntry.Entity.Id;
		Name = newDishTypeEntry.Entity.Name;
		Description = newDishTypeEntry.Entity.Description;

		UpdateIsNewAndTitle();

		_messenger.Send(DishTypeAddedMessage.CreateFromEntity(newDishType));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"New dish type successfully added.",
			"Dish Type Added",
			MessageBoxButton.Ok,
			MessageBoxImage.Information
		);

		IsProcessing = false;
	}

	private bool CanCancel() => !IsNew && !IsProcessing;

	[RelayCommand(CanExecute = nameof(CanCancel))]
	private async Task Cancel()
	{
		await LoadAsync(Id);
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

		var updatedDishType = await _context.DishTypes.FindAsync(Id);

		if (updatedDishType is null) throw new InvalidOperationException("Dish type not found.");

		// check if a new name already exists
		if (updatedDishType.Name != Name
			&& await ShowMessageIfNameAlreadyExists())
		{
			IsProcessing = false;

			return;
		}

		updatedDishType.Name = Name!;
		updatedDishType.Description = Description;

		UpdateIsNewAndTitle();

		await _context.SaveChangesAsync();

		_messenger.Send(DishTypeEditedMessage.CreateFromEntity(updatedDishType));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"Changes to dish type saved successfully.",
			"Dish Type Saved",
			MessageBoxButton.Ok,
			MessageBoxImage.Information
		);

		IsProcessing = false;
	}

	private bool CanDelete() => !IsNew && !IsProcessing;

	[RelayCommand(CanExecute = nameof(CanDelete))]
	private async Task Delete()
	{
		IsProcessing = true;

		var userResponse = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"Are you sure you want to delete this dish type?",
			"Delete Dish Type",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning
		);

		if (userResponse is not true)
		{
			IsProcessing = false;

			return;
		}

		var deletedDishType = await _context.DishTypes.FindAsync(Id);

		if (deletedDishType is null) throw new InvalidOperationException("Dish type not found.");

		if (await _context.DishTypes.AnyAsync(x => x.DishList.Any()))
		{
			_ = await _dialogService.ShowMessageBoxAsync
			(
				null,
				"Dish type is used by dishes.",
				"Delete Dish Type",
				MessageBoxButton.Ok,
				MessageBoxImage.Error
			);

			IsProcessing = false;

			return;
		}

		// TODO - check if day menu dish type are using this dish type

		_context.DishTypes.Remove(deletedDishType);
		await _context.SaveChangesAsync();

		Id = Guid.Empty;
		Name = null;
		Description = null;

		UpdateIsNewAndTitle();

		_messenger.Send(DishTypeDeletedMessage.CreateFromEntity(deletedDishType));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"Dish type deleted successfully.",
			"Delete Dish Type",
			MessageBoxButton.Ok,
			MessageBoxImage.Information
		);

		IsProcessing = false;
	}

	private async Task<bool> ShowMessageIfNameAlreadyExists()
	{
		if (!await _context.DishTypes.AnyAsync(x => x.Name == Name)) return false;

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			$"Dish type with name: \"{Name}\" already exists.",
			"Dish Type Exists",
			MessageBoxButton.Ok,
			MessageBoxImage.Error
		);

		return true;
	}
}
