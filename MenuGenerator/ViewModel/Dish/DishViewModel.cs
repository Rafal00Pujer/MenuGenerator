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

namespace MenuGenerator.ViewModel.Dish;

[View(typeof(DishView))]
public partial class DishViewModel :
	ViewModelBase,
	IRecipient<DishAddedMessage>,
	IRecipient<DishEditedMessage>,
	IRecipient<DishDeletedMessage>,
	IMainPage,
	IDisposable
{
	private readonly MenuGeneratorContext _context;
	private readonly IMessenger _messenger;
	private readonly IServiceScope _serviceScope;

	[ObservableProperty]
	private DishEditViewModel _dish;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AddNewCommand))]
	[NotifyCanExecuteChangedFor(nameof(SelectCommand))]
	private bool _isProcessing;

	private int _isProcessingCounter;

	public DishViewModel
	(
		MenuGeneratorContext context, IMessenger messenger,
		IServiceScopeFactory serviceScopeFactory
	)
	{
		_context = context;
		_messenger = messenger;
		_serviceScope = serviceScopeFactory.CreateScope();

		_messenger.RegisterAll(this);

		Dish = _serviceScope.ServiceProvider.GetRequiredService<DishEditViewModel>();
		Dish.PropertyChanged += OnDishPropertyChanged;
		Dish.LoadSummariesAsync().Wait();
	}

	public ObservableCollection<DishSummary> DishSummaries { get; } = [];

	public void Dispose()
	{
		_messenger.UnregisterAll(this);

		Dish.PropertyChanged -= OnDishPropertyChanged;

		_serviceScope.Dispose();

		GC.SuppressFinalize(this);
	}

	public async Task LoadAsync()
	{
		IncrementIsProcessingCounter();

		DishSummaries.Clear();

		await foreach (var dish in _context.Dishes)
		{
			var summary = new DishSummary(dish.Id, dish.Name);
			DishSummaries.Add(summary);
		}

		DecrementIsProcessingCounter();
	}

	public void Receive(DishAddedMessage message)
	{
		var addedDishSummary = new DishSummary(message.Id, message.Name);

		DishSummaries.Add(addedDishSummary);
	}

	public void Receive(DishDeletedMessage message)
	{
		var deletedDishSummary = DishSummaries.FirstOrDefault(x => x.Id == message.Id);

		if (deletedDishSummary is null) throw new InvalidOperationException("Dish not found!");

		DishSummaries.Remove(deletedDishSummary);
	}

	public void Receive(DishEditedMessage message)
	{
		var editedDishSummary = DishSummaries.FirstOrDefault(x => x.Id == message.Id);

		if (editedDishSummary is null) throw new InvalidOperationException("Dish not found!");

		var editedDishSummaryIndex = DishSummaries.IndexOf(editedDishSummary);
		DishSummaries.Remove(editedDishSummary);

		editedDishSummary = editedDishSummary with
		{
			Name = message.Name
		};

		DishSummaries.Add(editedDishSummary);
		DishSummaries.Move(DishSummaries.Count - 1, editedDishSummaryIndex);
	}

	private bool CanAddNew() => !IsProcessing;

	[RelayCommand(CanExecute = nameof(CanAddNew))]
	private void AddNew()
	{
		Dish.Clear();
	}

	private bool CanSelect() => !IsProcessing;

	[RelayCommand(CanExecute = nameof(CanSelect))]
	private async Task Select(Guid id)
	{
		IncrementIsProcessingCounter();

		await Dish.LoadAsync(id);

		DecrementIsProcessingCounter();
	}

	private void OnDishPropertyChanged(object? sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName != nameof(DishEditViewModel.IsProcessing)
			|| sender is not DishEditViewModel dish)
			return;

		if (dish.IsProcessing)
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

	public record DishSummary(Guid Id, string Name);
}
