using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

[EntityTypeConfiguration(typeof(DishFilterAndEntityConfiguration))]
public class DishFilterAndEntity : DishFilterWithDependentsEntity
{
	public override bool CanBeUsed(DishEntity dish) => DependentFilters.TrueForAll(x => x.CanBeUsed(dish));
}
