using System.Collections.Generic;
using System.Linq;
using MenuGenerator.Models.Entities.Menu;
using MenuGenerator.Models.Entities.OccurenceRule.DependsOn;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.OccurenceRule.CanOccurOnlyWith;

[EntityTypeConfiguration(typeof(CanOccurOnlyWithOneOfEntityConfiguration))]
public class CanOccurOnlyWithOneOfEntity : DependsOnRuleEntity
{
    public override bool IsSatisfied(
        IEnumerable<MenuEntity> previousMonthMenus,
        IEnumerable<MenuEntity> proposedMenus,
        MenuEntity currentGeneratedMenu)
    {
        return currentGeneratedMenu.DishList
            .Select(x => x.Id)
            .Intersect(DependsOnDishList
                .Select(x => x.Id))
            .Any();
    }
}