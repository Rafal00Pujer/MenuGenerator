using System;
using System.Collections.Generic;
using MenuGenerator.Models.Entities.Dish;
using MenuGenerator.Models.Entities.Menu;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.OccurenceRule;

[EntityTypeConfiguration(typeof(OccurenceRuleEntityConfiguration))]
public abstract class OccurenceRuleEntity
{
    public required Guid Id { get; set; } = Guid.CreateVersion7();

    public required Guid DishId { get; set; }

    public required DishEntity Dish { get; set; } = null!;

    public abstract bool IsSatisfied(
        IEnumerable<MenuEntity> previousMonthMenus, 
        IEnumerable<MenuEntity> proposedMenus,
        MenuEntity currentGeneratedMenu);
}