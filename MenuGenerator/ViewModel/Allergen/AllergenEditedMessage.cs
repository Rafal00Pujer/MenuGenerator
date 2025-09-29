using System;
using MenuGenerator.Models.Entities.Allergen;

namespace MenuGenerator.ViewModel.Allergen;

public record AllergenEditedMessage(Guid Id, string DisplayId, string? Description)
{
    public static AllergenEditedMessage CreateFromEntity(AllergenEntity entity)
    {
        return new AllergenEditedMessage(entity.Id, entity.DisplayId, entity.Description);
    }
}