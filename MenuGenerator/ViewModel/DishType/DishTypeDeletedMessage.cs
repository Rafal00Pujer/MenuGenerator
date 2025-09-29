using System;
using MenuGenerator.Models.Entities.DishType;

namespace MenuGenerator.ViewModel.DishType;

public record DishTypeDeletedMessage(Guid Id, string Name, string? Description)
{
    public static DishTypeDeletedMessage CreateFromEntity(DishTypeEntity entity)
    {
        return new DishTypeDeletedMessage(entity.Id, entity.Name, entity.Description);
    }
}