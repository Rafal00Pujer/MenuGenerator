using System;
using MenuGenerator.Models.Entities.DishAttribute;

namespace MenuGenerator.ViewModel.DishAttribute;

public record DishAttributeDeletedMessage(Guid Id, string Name, string? Description)
{
	public static DishAttributeDeletedMessage CreateFromEntity
		(DishAttributeEntity entity)
		=> new(entity.Id, entity.Name, entity.Description);
}
