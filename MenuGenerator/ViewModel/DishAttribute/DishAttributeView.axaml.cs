using Avalonia.Controls;

namespace MenuGenerator.ViewModel.DishAttribute;

public partial class DishAttributeView : UserControl
{
	public DishAttributeView()
	{
		InitializeComponent();

		SizeChanged += OnSizeChanged;
	}

	private void OnSizeChanged(object? sender, SizeChangedEventArgs args)
	{
		var addNewBtnSize = AddNewBtn.DesiredSize;

		DishAttributesScroll.Height = args.NewSize.Height - addNewBtnSize.Height;
	}
}
