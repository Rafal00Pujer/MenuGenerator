using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate;

[EntityTypeConfiguration<DayMenuTemplateEntityConfiguration, DayMenuTemplateEntity>]
public class DayMenuTemplateEntity
{
	public required Guid Id { get; set; } = Guid.CreateVersion7();

	public required TemplateDayFlag TemplateDays { get; set; } = TemplateDayFlag.Week;

	public required List<DayMenuDishEntity> DishTypeList { get; set; } = [];

	// TODO - Add dish composition
}
