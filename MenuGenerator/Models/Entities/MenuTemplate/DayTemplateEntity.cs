using System;
using MenuGenerator.Models.Entities.DishType;

namespace MenuGenerator.Models.Entities.MenuTemplate;

public class DayTemplateEntity
{
	public required int Id { get; set; }

	public required TemplateDayFlag TemplateDays { get; set; } = TemplateDayFlag.Week;
	
	public required int Order { get; set; }

	// TODO - Remove after testing
	public required string Name { get; set; } = null!;

	// TODO - Maybe move dish type and filter rule to a separate dependent type

	// TODO - Uncomment after testing
	//public required Guid DishTypeId { get; set; } = Guid.Empty;

	//public required DishTypeEntity DishType { get; set; } = null!;

	// TODO - Add filter rule

	// TODO - Add dish composition
}
