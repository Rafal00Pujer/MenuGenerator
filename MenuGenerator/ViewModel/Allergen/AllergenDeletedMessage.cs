using System;
using MenuGenerator.Models.Entities.Allergen;

namespace MenuGenerator.ViewModel.Allergen;

public record AllergenDeletedMessage(Guid Id, string DisplayId, string? Description)
{
	public static AllergenDeletedMessage CreateFromEntity
		(AllergenEntity entity)
		=> new(entity.Id, entity.DisplayId, entity.Description);
}
