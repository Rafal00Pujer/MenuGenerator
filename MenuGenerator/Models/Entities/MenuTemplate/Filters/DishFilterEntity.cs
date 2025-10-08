using System;
using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

[EntityTypeConfiguration(typeof(DishFilterEntityConfiguration))]
public abstract class DishFilterEntity
{
	public required Guid Id { get; set; } = Guid.CreateVersion7();

	public abstract bool CanBeUsed(DishEntity dish);
}
