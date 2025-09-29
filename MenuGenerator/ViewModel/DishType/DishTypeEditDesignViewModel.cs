using System;

namespace MenuGenerator.ViewModel.DishType;

public class DishTypeEditDesignViewModel : DishTypeEditViewModel
{
	public DishTypeEditDesignViewModel() : this(true, string.Empty)
	{
	}

	public DishTypeEditDesignViewModel(bool empty = true, string suffix = "") : base(null!, null!, null!)
	{
		if (empty) return;

		Id = Guid.NewGuid();

		Name = "Dish Name";
		Description = "Dish Description";

		if (string.IsNullOrEmpty(suffix)) return;

		Name = Name + " " + suffix;
	}
}
