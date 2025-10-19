using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

[EntityTypeConfiguration(typeof(DishFilterWithDependentsEntityConfiguration))]
public abstract class DishFilterWithDependentsEntity : DishFilterEntity
{
	public List<DishFilterEntity> DependentFilters { get; set; } = [];
}
