using System.Collections.Generic;
using MenuGenerator.Models.Entities.Dish;

namespace MenuGenerator.Models.Entities.OccurenceRule;

public abstract class DependsOnRuleEntity : OccurenceRuleEntity
{
    public List<DishEntity> DependsOnDishList { get; set; } = [];
}