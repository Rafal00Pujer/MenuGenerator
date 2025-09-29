using System;
using MenuGenerator.Models.Entities.DishType;

namespace MenuGenerator.ViewModel.DishType;

public record DishTypeEditedMessage(Guid Id, string Name, string? Description)
{
    public static DishTypeEditedMessage CreateFromEntity(DishTypeEntity entity)
    {
        return new DishTypeEditedMessage(entity.Id, entity.Name, entity.Description);
    }
}