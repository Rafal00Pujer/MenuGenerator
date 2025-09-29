using System;
using MenuGenerator.Models.Entities.Allergen;

namespace MenuGenerator.ViewModel.Allergen;

public record AllergenAddedMessage(Guid Id, string DisplayId, string? Description)
{
    public static AllergenAddedMessage CreateFromEntity(AllergenEntity entity)
    {
        return new AllergenAddedMessage(entity.Id, entity.DisplayId, entity.Description);
    }
}