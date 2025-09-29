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

namespace MenuGenerator.ViewModel.DishType;

[View(typeof(DishTypeView))]
public partial class DishTypeViewModel :
	ViewModelBase,
	IRecipient<DishTypeAddedMessage>,
	IRecipient<DishTypeEditedMessage>,
	IRecipient<DishTypeDeletedMessage>,
	IMainPage,
	IDisposable
{
	private readonly MenuGeneratorContext _context;
	private readonly IMessenger _messenger;
	private readonly IServiceScope _serviceScope;

	[ObservableProperty]
	private DishTypeEditViewModel _dishType;

	[ObservableProperty]
	private ObservableCollection<DishTypeSummary> _dishTypeSummaries = [];

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AddNewCommand))]
	[NotifyCanExecuteChangedFor(nameof(SelectCommand))]
	private bool _isProcessing;

	private int _isProcessingCounter;

	public DishTypeViewModel
	(
		MenuGeneratorContext context, IMessenger messenger,
		IServiceScopeFactory serviceScopeFactory
	)
	{
		_context = context;
		_messenger = messenger;
		_serviceScope = serviceScopeFactory.CreateScope();

		_messenger.RegisterAll(this);

		DishType = _serviceScope.ServiceProvider.GetRequiredService<DishTypeEditViewModel>();
		DishType.PropertyChanged += OnDishTypePropertyChanged;
	}

	/// <summary>
	///     Design time constructor.
	/// </summary>
	protected DishTypeViewModel()
	{
		_context = null!;
		_messenger = null!;
		_serviceScope = null!;
		DishType = null!;
	}

	public void Dispose()
	{
		_messenger.UnregisterAll(this);

		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		// Just in case that design view model didn't set dish type
		if (DishType is not null) DishType.PropertyChanged -= OnDishTypePropertyChanged;

		_serviceScope.Dispose();

		GC.SuppressFinalize(this);
	}

	public async Task LoadAsync()
	{
		IncrementIsProcessingCounter();

		DishTypeSummaries.Clear();

		await foreach (var dishType in _context.DishTypes)
		{
			var summary = new DishTypeSummary(dishType.Id, dishType.Name);
			DishTypeSummaries.Add(summary);
		}

		DecrementIsProcessingCounter();
	}

	public void Receive(DishTypeAddedMessage message)
	{
		var addedDishTypeSummary = new DishTypeSummary(message.Id, message.Name);

		DishTypeSummaries.Add(addedDishTypeSummary);
	}

	public void Receive(DishTypeDeletedMessage message)
	{
		var deletedDishTypeSummary = DishTypeSummaries.FirstOrDefault(x => x.Id == message.Id);

		if (deletedDishTypeSummary is null) throw new InvalidOperationException("Dish Type not found!");

		DishTypeSummaries.Remove(deletedDishTypeSummary);
	}

	public void Receive(DishTypeEditedMessage message)
	{
		var editedDishTypeSummary = DishTypeSummaries.FirstOrDefault(x => x.Id == message.Id);

		if (editedDishTypeSummary is null) throw new InvalidOperationException("Dish Type not found!");

		var editedDishTypeSummaryIndex = DishTypeSummaries.IndexOf(editedDishTypeSummary);
		DishTypeSummaries.Remove(editedDishTypeSummary);

		editedDishTypeSummary = editedDishTypeSummary with
		{
			Name = message.Name
		};

		DishTypeSummaries.Add(editedDishTypeSummary);
		DishTypeSummaries.Move(DishTypeSummaries.Count - 1, editedDishTypeSummaryIndex);
	}

	private bool CanAddNew() => !IsProcessing;

	[RelayCommand(CanExecute = nameof(CanAddNew))]
	private void AddNew()
	{
		DishType.Clear();
	}

	private bool CanSelect() => !IsProcessing;

	[RelayCommand(CanExecute = nameof(CanSelect))]
	private async Task Select(Guid id)
	{
		IncrementIsProcessingCounter();

		await DishType.LoadAsync(id);

		DecrementIsProcessingCounter();
	}

	private void OnDishTypePropertyChanged(object? sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName != nameof(DishTypeEditViewModel.IsProcessing)) return;

		if (DishType.IsProcessing)
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
			case < 0: throw new InvalidOperationException($"{nameof(_isProcessingCounter)} is negative!");

			case > 0:
				IsProcessing = true;

				return;

			default: IsProcessing = false; break;
		}
	}

	public record DishTypeSummary(Guid Id, string Name);
}
