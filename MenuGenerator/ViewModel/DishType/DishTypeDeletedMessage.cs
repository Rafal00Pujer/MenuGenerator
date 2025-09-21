using CommunityToolkit.Mvvm.Messaging.Messages;
using MenuGenerator.Models.Entities.DishType;

namespace MenuGenerator.ViewModel.DishType;

public class DishTypeDeletedMessage(DishTypeEntity value) : ValueChangedMessage<DishTypeEntity>(value);