using System;
using MenuGenerator.Models.Entities.Dish;
using MenuGenerator.Models.Entities.DishType;
using MenuGenerator.Models.Entities.MenuTemplate.Filters;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate;

[EntityTypeConfiguration<DayMenuDishEntityConfiguration, DayMenuDishEntity>]
public class DayMenuDishEntity
{
	public required Guid Id { get; set; } = Guid.CreateVersion7();

	public required Guid DishTypeId { get; set; } = Guid.Empty;

	public required DishTypeEntity DishType { get; set; } = null!;

	public required Guid DishFilterId { get; set; } = Guid.Empty;

	public required DishFilterEntity DishFilter { get; set; }

	public bool DishCanBeUsed(DishEntity dish) => dish.TypeId == DishTypeId && DishFilter.CanBeUsed(dish);
}
