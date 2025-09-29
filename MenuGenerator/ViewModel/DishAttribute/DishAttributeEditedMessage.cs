using System;
using MenuGenerator.Models.Entities.DishAttribute;

namespace MenuGenerator.ViewModel.DishAttribute;

public record DishAttributeEditedMessage(Guid Id, string Name, string? Description)
{
	public static DishAttributeEditedMessage CreateFromEntity
		(DishAttributeEntity entity)
		=> new(entity.Id, entity.Name, entity.Description);
}
