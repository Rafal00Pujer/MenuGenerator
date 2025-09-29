using System.Collections.Generic;
using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.OccurenceRule.DependsOn;

[EntityTypeConfiguration(typeof(DependsOnRuleEntityConfiguration))]
public abstract class DependsOnRuleEntity : OccurenceRuleEntity
{
	public List<DishEntity> DependsOnDishList { get; set; } = [];
}
