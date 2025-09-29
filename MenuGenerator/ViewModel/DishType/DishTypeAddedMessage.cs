using System;
using MenuGenerator.Models.Entities.DishType;

namespace MenuGenerator.ViewModel.DishType;

public record DishTypeAddedMessage(Guid Id, string Name, string? Description)
{
    public static DishTypeAddedMessage CreateFromEntity(DishTypeEntity entity)
    {
        return new DishTypeAddedMessage(entity.Id, entity.Name, entity.Description);
    }
}