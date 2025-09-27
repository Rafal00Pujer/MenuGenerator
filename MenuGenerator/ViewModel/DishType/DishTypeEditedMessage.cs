using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MenuGenerator.Models.Entities.DishType;

namespace MenuGenerator.ViewModel.DishType;

public record DishTypeEditedMessage(Guid Id, string Name, string? Description)
{
    public static DishTypeEditedMessage CreateFromEntity(DishTypeEntity entity) =>
        new(entity.Id, entity.Name, entity.Description);
}