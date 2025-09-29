using Avalonia.Controls;

namespace MenuGenerator.ViewModel.Allergen;

public partial class AllergenView : UserControl
{
	public AllergenView()
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
