using System;
using MenuGenerator.Models.Entities.DishAttribute;

namespace MenuGenerator.ViewModel.DishAttribute;

public record DishAttributeAddedMessage(Guid Id, string Name, string? Description)
{
	public static DishAttributeAddedMessage CreateFromEntity
		(DishAttributeEntity entity)
		=> new(entity.Id, entity.Name, entity.Description);
}
