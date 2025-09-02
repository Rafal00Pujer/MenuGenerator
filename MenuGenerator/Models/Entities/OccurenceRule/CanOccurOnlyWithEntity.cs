using System.Collections.Generic;
using System.Linq;
using MenuGenerator.Models.Entities.Menu;

namespace MenuGenerator.Models.Entities.OccurenceRule;

public class CanOccurOnlyWithEntity : DependsOnRuleEntity
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