using System;
using MenuGenerator.Models.Entities.Dish;

namespace MenuGenerator.ViewModel.Dish;

public record DishAddedMessage(Guid Id, string Name, string? Description, bool IncludeInNewMenus, Guid TypeId)
{
	public static DishAddedMessage CreateFromEntity
		(DishEntity entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.IncludeInNewMenus, entity.TypeId);
}
