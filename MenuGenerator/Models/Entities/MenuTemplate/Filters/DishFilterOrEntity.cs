using System.Linq;
using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

[EntityTypeConfiguration(typeof(DishFilterOrEntityConfiguration))]
public class DishFilterOrEntity : DishFilterWithDependentsEntity
{
	public override bool CanBeUsed(DishEntity dish) => DependentFilters.Any(x => x.CanBeUsed(dish));
}
