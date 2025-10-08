using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

[EntityTypeConfiguration(typeof(DishFilterTrueEntityConfiguration))]
public class DishFilterTrueEntity : DishFilterEntity
{
	public override bool CanBeUsed(DishEntity dish) => true;
}
