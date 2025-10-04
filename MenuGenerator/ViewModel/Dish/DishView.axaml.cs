using Avalonia.Controls;

namespace MenuGenerator.ViewModel.Dish;

public partial class DishView : UserControl
{
	public DishView()
	{
		InitializeComponent();

		SizeChanged += OnSizeChanged;
	}

	private void OnSizeChanged(object? sender, SizeChangedEventArgs args)
	{
		var addNewBtnSize = AddNewBtn.DesiredSize;

		AllergensScroll.Height = args.NewSize.Height - addNewBtnSize.Height;
	}
}
