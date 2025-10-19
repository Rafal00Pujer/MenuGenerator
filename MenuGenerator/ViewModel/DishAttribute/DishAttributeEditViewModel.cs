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
using MenuGenerator.Models.Entities.DishAttribute;
using MenuGenerator.ViewLocator;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.ViewModel.DishAttribute;

[View(typeof(DishAttributeEditView))]
public partial class DishAttributeEditViewModel : ViewModelBase
{
	private readonly MenuGeneratorContext _context;
	private readonly IDialogService _dialogService;
	private readonly IMessenger _messenger;

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[MaxLength(500)]
	[NotifyCanExecuteChangedFor(nameof(AddCommand), nameof(SaveCommand))]
	private string? _description;

	private Guid _id = Guid.Empty;

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

	public DishAttributeEditViewModel(MenuGeneratorContext context, IDialogService dialogService, IMessenger messenger)
	{
		_context = context;
		_dialogService = dialogService;
		_messenger = messenger;

		UpdateIsNewAndTitle();
	}

	public async ValueTask LoadAsync(Guid id)
	{
		IsProcessing = true;

		var dishAttribute = await _context.DishAttributes.FindAsync(id);

		if (dishAttribute is null) throw new InvalidOperationException("Dish attribute not found.");

		_id = dishAttribute.Id;
		Name = dishAttribute.Name;
		Description = dishAttribute.Description;

		UpdateIsNewAndTitle();

		IsProcessing = false;
	}

	public void Clear()
	{
		_id = Guid.Empty;
		Name = null;
		Description = null;

		UpdateIsNewAndTitle();
	}

	private void UpdateIsNewAndTitle()
	{
		IsNew = _id == Guid.Empty;
		Title = IsNew ? "Add New Dish Attribute" : "Edit " + Name;
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

		if (await CheckAndShowMessageIfNameAlreadyExists())
		{
			IsProcessing = false;

			return;
		}

		var newDishAttribute = new DishAttributeEntity
		{
			Id = Guid.CreateVersion7(), Name = Name!, Description = Description
		};

		var newDishAttributeEntry = await _context.DishAttributes.AddAsync(newDishAttribute);
		newDishAttribute = newDishAttributeEntry.Entity;

		await _context.SaveChangesAsync();

		_id = newDishAttribute.Id;
		Name = newDishAttribute.Name;
		Description = newDishAttribute.Description;

		UpdateIsNewAndTitle();

		_messenger.Send(DishAttributeAddedMessage.CreateFromEntity(newDishAttribute));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"New dish attribute successfully added.",
			"Dish Attribute Added",
			MessageBoxButton.Ok,
			MessageBoxImage.Information
		);

		IsProcessing = false;
	}

	private bool CanCancel() => !IsNew && !IsProcessing;

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

		var updatedDishAttribute = await _context.DishAttributes.FindAsync(_id);

		if (updatedDishAttribute is null) throw new InvalidOperationException("Dish attribute not found.");

		// check if a new name already exists
		if (updatedDishAttribute.Name != Name
			&& await CheckAndShowMessageIfNameAlreadyExists())
		{
			IsProcessing = false;

			return;
		}

		updatedDishAttribute.Name = Name!;
		updatedDishAttribute.Description = Description;

		UpdateIsNewAndTitle();

		await _context.SaveChangesAsync();

		_messenger.Send(DishAttributeEditedMessage.CreateFromEntity(updatedDishAttribute));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"Changes to dish attribute saved successfully.",
			"Dish Attribute Saved",
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
			"Are you sure you want to delete this dish attribute?",
			"Delete Dish Attribute",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning
		);

		if (userResponse is not true)
		{
			IsProcessing = false;

			return;
		}

		var deletedDishAttribute = await _context.DishAttributes.FindAsync(_id);

		if (deletedDishAttribute is null) throw new InvalidOperationException("Dish attribute not found.");

		if (await _context.DishAttributes.AnyAsync(x => x.DishList.Any()))
		{
			_ = await _dialogService.ShowMessageBoxAsync
			(
				null,
				"Dish attribute is used by dishes.",
				"Delete Dish Attribute",
				MessageBoxButton.Ok,
				MessageBoxImage.Error
			);

			IsProcessing = false;

			return;
		}

		//TODO - check if attribute is used by filters
		
		_context.DishAttributes.Remove(deletedDishAttribute);
		await _context.SaveChangesAsync();

		_id = Guid.Empty;
		Name = null;
		Description = null;

		UpdateIsNewAndTitle();

		_messenger.Send(DishAttributeDeletedMessage.CreateFromEntity(deletedDishAttribute));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"Dish attribute deleted successfully.",
			"Delete Dish Attribute",
			MessageBoxButton.Ok,
			MessageBoxImage.Information
		);

		IsProcessing = false;
	}

	private async Task<bool> CheckAndShowMessageIfNameAlreadyExists()
	{
		if (!await _context.DishAttributes.AnyAsync(x => x.Name == Name)) return false;

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			$"Dish attribute with name: \"{Name}\" already exists.",
			"Dish Attribute Exists",
			MessageBoxButton.Ok,
			MessageBoxImage.Error
		);

		return true;
	}
}
