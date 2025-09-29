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

namespace MenuGenerator.ViewModel.DishAttribute;

[View(typeof(DishAttributeView))]
public partial class DishAttributeViewModel :
	ViewModelBase,
	IRecipient<DishAttributeAddedMessage>,
	IRecipient<DishAttributeEditedMessage>,
	IRecipient<DishAttributeDeletedMessage>,
	IMainPage,
	IDisposable
{
	private readonly MenuGeneratorContext _context;
	private readonly IMessenger _messenger;
	private readonly IServiceScope _serviceScope;

	[ObservableProperty]
	private DishAttributeEditViewModel _dishAttribute;

	[ObservableProperty]
	private ObservableCollection<DishAttributeSummary> _dishAttributeSummaries = [];

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AddNewCommand))]
	[NotifyCanExecuteChangedFor(nameof(SelectCommand))]
	private bool _isProcessing;

	private int _isProcessingCounter;

	public DishAttributeViewModel
		(MenuGeneratorContext context, IMessenger messenger, IServiceScopeFactory serviceScopeFactory)
	{
		_context = context;
		_messenger = messenger;
		_serviceScope = serviceScopeFactory.CreateScope();

		_messenger.RegisterAll(this);

		DishAttribute = _serviceScope.ServiceProvider.GetRequiredService<DishAttributeEditViewModel>();
		DishAttribute.PropertyChanged += OnDishAttributePropertyChanged;
	}

	public void Dispose()
	{
		_messenger.UnregisterAll(this);

		DishAttribute.PropertyChanged -= OnDishAttributePropertyChanged;

		_serviceScope.Dispose();

		GC.SuppressFinalize(this);
	}

	public async Task LoadAsync()
	{
		IncrementIsProcessingCounter();

		DishAttributeSummaries.Clear();

		await foreach (var dishAttribute in _context.DishAttributes)
		{
			var summary = new DishAttributeSummary(dishAttribute.Id, dishAttribute.Name);
			DishAttributeSummaries.Add(summary);
		}

		DecrementIsProcessingCounter();
	}

	public void Receive(DishAttributeAddedMessage message)
	{
		var addedDishAttributeSummary = new DishAttributeSummary(message.Id, message.Name);

		DishAttributeSummaries.Add(addedDishAttributeSummary);
	}

	public void Receive(DishAttributeDeletedMessage message)
	{
		var deletedDishAttributeSummary = DishAttributeSummaries.FirstOrDefault(x => x.Id == message.Id);

		if (deletedDishAttributeSummary is null) throw new InvalidOperationException("Dish attribute not found!");

		DishAttributeSummaries.Remove(deletedDishAttributeSummary);
	}

	public void Receive(DishAttributeEditedMessage message)
	{
		var editedDishAttributeSummary = DishAttributeSummaries.FirstOrDefault(x => x.Id == message.Id);

		if (editedDishAttributeSummary is null) throw new InvalidOperationException("Dish attribute not found!");

		var editedDishTypeSummaryIndex = DishAttributeSummaries.IndexOf(editedDishAttributeSummary);
		DishAttributeSummaries.Remove(editedDishAttributeSummary);

		editedDishAttributeSummary = editedDishAttributeSummary with
		{
			DisplayId = message.Name
		};

		DishAttributeSummaries.Add(editedDishAttributeSummary);
		DishAttributeSummaries.Move(DishAttributeSummaries.Count - 1, editedDishTypeSummaryIndex);
	}

	private bool CanAddNew() => !IsProcessing;

	[RelayCommand(CanExecute = nameof(CanAddNew))]
	private void AddNew()
	{
		DishAttribute.Clear();
	}

	private bool CanSelect() => !IsProcessing;

	[RelayCommand(CanExecute = nameof(CanSelect))]
	private async Task Select(Guid id)
	{
		IncrementIsProcessingCounter();

		await DishAttribute.LoadAsync(id);

		DecrementIsProcessingCounter();
	}

	private void OnDishAttributePropertyChanged(object? sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName != nameof(DishAttributeEditViewModel.IsProcessing)
			|| sender is not DishAttributeEditViewModel dishAttributeEditViewModel)
			return;

		if (dishAttributeEditViewModel.IsProcessing)
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

	public record DishAttributeSummary(Guid Id, string DisplayId);
}
