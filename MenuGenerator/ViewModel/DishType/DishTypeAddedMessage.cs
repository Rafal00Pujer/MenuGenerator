using CommunityToolkit.Mvvm.Messaging.Messages;
using MenuGenerator.Models.Entities.DishType;

namespace MenuGenerator.ViewModel.DishType;

public class DishTypeAddedMessage(DishTypeEntity value) : ValueChangedMessage<DishTypeEntity>(value);