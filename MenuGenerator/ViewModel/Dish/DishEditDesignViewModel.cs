using System;
using System.Linq;

namespace MenuGenerator.ViewModel.Dish;

public class DishEditDesignViewModel : DishEditViewModel
{
	public DishEditDesignViewModel()
	{
		foreach (var i in Enumerable.Range(1, 10))
		{
			var newDishTypeSummary = new DishTypeSummary(Guid.NewGuid(), $"Dish Type {i}");
			Types.Add(newDishTypeSummary);
		}

		foreach (var i in Enumerable.Range(1, 10))
		{
			var newDishAttributeSummary = new DishAttributeSummary(Guid.NewGuid(), $"Dish Attribute {i}");
			Attributes.Add(newDishAttributeSummary);
		}

		foreach (var i in Enumerable.Range(1, 10))
		{
			var newAllergenSummary = new AllergenSummary(Guid.NewGuid(), $"Allergen {i}");
			Allergens.Add(newAllergenSummary);
		}

		Name = "Dish Design Name";
		Description = "Dish Design Description";
		IncludeInNewMenus = false;
		SelectedDishType = Types.First();

		UpdateIsNewAndTitle();
	}
}
