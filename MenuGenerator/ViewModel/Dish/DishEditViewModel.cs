using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using MenuGenerator.Models.Database;
using MenuGenerator.Models.Entities.Dish;
using MenuGenerator.ViewLocator;

namespace MenuGenerator.ViewModel.Dish;

[View(typeof(DishEditView))]
public partial class DishEditViewModel : ViewModelBase
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
	[NotifyDataErrorInfo]
	[Required]
	private bool? _includeInNewMenus = true;

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
	[NotifyDataErrorInfo]
	[Required]
	private DishTypeSummary? _selectedDishType;

	private bool _summariesLoaded;

	[ObservableProperty]
	private string _title = null!;

	protected Guid Id = Guid.Empty;

	public DishEditViewModel
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

	/// <summary>
	///     Design-time constructor.
	/// </summary>
	protected DishEditViewModel()
	{
		_context = null!;
		_dialogService = null!;
		_messenger = null!;
	}

	public ObservableCollection<AllergenSummary> SelectedAllergens { get; } = [];
	public ObservableCollection<DishAttributeSummary> SelectedAttributes { get; } = [];

	public ObservableCollection<DishTypeSummary> Types { get; } = [];
	public ObservableCollection<AllergenSummary> Allergens { get; } = [];
	public ObservableCollection<DishAttributeSummary> Attributes { get; } = [];

	public async Task LoadAsync(Guid id)
	{
		await LoadSummariesAsync();

		IsProcessing = true;

		var dish = await _context.Dishes.FindAsync(id);

		if (dish is null) throw new InvalidOperationException("Dish not found.");

		Id = dish.Id;
		Name = dish.Name;
		Description = dish.Description;
		IncludeInNewMenus = dish.IncludeInNewMenus;

		SelectedDishType = Types.FirstOrDefault(x => x.Id == dish.TypeId)
						   ?? throw new InvalidOperationException("Dish type not found.");

		SelectedAllergens.Clear();

		await foreach (var allergenId in _context.Allergens
												 .Where(x => x.DishList.Any(y => y.Id == dish.Id))
												 .Select(x => x.Id)
												 .ToAsyncEnumerable())
		{
			var allergenSummary = Allergens.FirstOrDefault(x => x.Id == allergenId);

			if (allergenSummary is null) throw new InvalidOperationException("Allergen not found.");

			SelectedAllergens.Add(allergenSummary);
		}

		SelectedAttributes.Clear();

		await foreach (var attributeId in _context.DishAttributes
												  .Where(x => x.DishList.Any(y => y.Id == dish.Id))
												  .Select(x => x.Id)
												  .ToAsyncEnumerable())
		{
			var attributeSummary = Attributes.FirstOrDefault(x => x.Id == attributeId);

			if (attributeSummary is null) throw new InvalidOperationException("Attribute not found.");

			SelectedAttributes.Add(attributeSummary);
		}

		UpdateIsNewAndTitle();

		IsProcessing = false;
	}

	public async Task LoadSummariesAsync()
	{
		if (_summariesLoaded) return;

		IsProcessing = true;

		await foreach (var dishType in _context.DishTypes.ToAsyncEnumerable())
		{
			var dishTypeSummary = new DishTypeSummary(dishType.Id, dishType.Name);
			Types.Add(dishTypeSummary);
		}

		await foreach (var allergen in _context.Allergens.ToAsyncEnumerable())
		{
			var allergenSummary = new AllergenSummary(allergen.Id, allergen.DisplayId);
			Allergens.Add(allergenSummary);
		}

		await foreach (var attribute in _context.DishAttributes.ToAsyncEnumerable())
		{
			var dishAttributeSummary = new DishAttributeSummary(attribute.Id, attribute.Name);
			Attributes.Add(dishAttributeSummary);
		}

		_summariesLoaded = true;

		SelectedDishType = Types.FirstOrDefault();

		IsProcessing = false;
	}

	public void Clear()
	{
		Id = Guid.Empty;
		Name = null;
		Description = null;
		SelectedDishType = null;
		IncludeInNewMenus = true;

		SelectedDishType = Types.FirstOrDefault();

		SelectedAllergens.Clear();
		SelectedAttributes.Clear();

		UpdateIsNewAndTitle();
	}

	protected void UpdateIsNewAndTitle()
	{
		IsNew = Id == Guid.Empty;
		Title = IsNew ? "Add New Dish" : "Edit " + Name;
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

		var newDish = new DishEntity
		{
			Id = Guid.CreateVersion7(),
			Name = Name!,
			Description = Description,
			IncludeInNewMenus = IncludeInNewMenus ?? false,
			TypeId = SelectedDishType!.Id
		};

		var newDishEntry = await _context.Dishes.AddAsync(newDish);
		newDish = newDishEntry.Entity;

		await AddAllergens(newDish);
		await AddDishAttributes(newDish);

		await _context.SaveChangesAsync();

		Id = newDish.Id;

		UpdateIsNewAndTitle();

		_messenger.Send(DishAddedMessage.CreateFromEntity(newDish));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"New dish successfully added.",
			"Dish Added",
			MessageBoxButton.Ok,
			MessageBoxImage.Information
		);

		IsProcessing = false;
	}

	private async Task AddAllergens(DishEntity dishEntity)
	{
		var selectedAllergenEntities = await _context.Allergens
													 .AsAsyncEnumerable() // EF Core sqlite does not support observable collection
													 .Where(x => SelectedAllergens.Any(y => y.Id == x.Id))
													 .ToArrayAsync();

		if (selectedAllergenEntities.Length != SelectedAllergens.Count)
			throw new InvalidOperationException("Allergens not found.");

		dishEntity.AllergenList.AddRange(selectedAllergenEntities);
	}

	private async Task AddDishAttributes(DishEntity dishEntity)
	{
		var selectedAttributeEntities = await _context.DishAttributes
													  .AsAsyncEnumerable() // EF Core sqlite does not support observable collection
													  .Where(x => SelectedAttributes.Any(y => y.Id == x.Id))
													  .ToArrayAsync();

		if (selectedAttributeEntities.Length != SelectedAttributes.Count)
			throw new InvalidOperationException("Attributes not found.");

		dishEntity.AttributeList.AddRange(selectedAttributeEntities);
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

		var updatedDish = await _context.Dishes.FindAsync(Id);

		if (updatedDish is null) throw new InvalidOperationException("Dish not found.");

		updatedDish.Name = Name!;
		updatedDish.Description = Description;
		updatedDish.IncludeInNewMenus = IncludeInNewMenus ?? false;
		updatedDish.TypeId = SelectedDishType!.Id;

		UpdateIsNewAndTitle();

		await _context.Entry(updatedDish).Collection(x => x.AllergenList).LoadAsync();
		await _context.Entry(updatedDish).Collection(x => x.AttributeList).LoadAsync();

		updatedDish.AllergenList.Clear();
		updatedDish.AttributeList.Clear();

		await AddAllergens(updatedDish);
		await AddDishAttributes(updatedDish);

		await _context.SaveChangesAsync();

		_messenger.Send(DishEditedMessage.CreateFromEntity(updatedDish));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"Changes to dish saved successfully.",
			"Dish Saved",
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
			"Are you sure you want to delete this dish?",
			"Delete Dish",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning
		);

		if (userResponse is not true)
		{
			IsProcessing = false;

			return;
		}

		var deletedDish = await _context.Dishes.FindAsync(Id);

		if (deletedDish is null) throw new InvalidOperationException("Dish not found.");

		// TODO: check if dish is referenced by other entities

		_context.Dishes.Remove(deletedDish);
		await _context.SaveChangesAsync();

		Clear();

		_messenger.Send(DishDeletedMessage.CreateFromEntity(deletedDish));

		_ = await _dialogService.ShowMessageBoxAsync
		(
			null,
			"Dish deleted successfully.",
			"Delete Dish",
			MessageBoxButton.Ok,
			MessageBoxImage.Information
		);

		IsProcessing = false;
	}

	[RelayCommand]
	private void ClearSelectedAllergens() => SelectedAllergens.Clear();

	[RelayCommand]
	private void ClearSelectedAttributes() => SelectedAttributes.Clear();

	public record DishTypeSummary(Guid Id, string Name);

	public record AllergenSummary(Guid Id, string DisplayId);

	public record DishAttributeSummary(Guid Id, string Name);
}
