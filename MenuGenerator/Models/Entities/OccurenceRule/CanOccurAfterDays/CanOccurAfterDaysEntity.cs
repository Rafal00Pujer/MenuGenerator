using System.Collections.Generic;
using System.Linq;
using MenuGenerator.Models.Entities.Menu;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.OccurenceRule.CanOccurAfterDays;

[EntityTypeConfiguration(typeof(CanOccurAfterDaysEntityConfiguration))]
public class CanOccurAfterDaysEntity : OccurenceRuleEntity
{
	public required int Days { get; set; }

	public override bool IsSatisfied
	(
		IEnumerable<MenuEntity> previousMonthMenus,
		IEnumerable<MenuEntity> proposedMenus,
		MenuEntity currentGeneratedMenu
	)
	{
		return previousMonthMenus
			   .Concat(proposedMenus)
			   .Append(currentGeneratedMenu)
			   .OrderBy(x => x.Date)
			   .Take(Days)
			   .SelectMany(x => x.DishList)
			   .All(x => x.Id != DishId);
	}
}
