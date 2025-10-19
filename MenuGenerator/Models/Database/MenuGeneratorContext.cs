using MenuGenerator.Models.Entities.Allergen;
using MenuGenerator.Models.Entities.Dish;
using MenuGenerator.Models.Entities.DishAttribute;
using MenuGenerator.Models.Entities.DishType;
using MenuGenerator.Models.Entities.MenuTemplate;
using MenuGenerator.Models.Entities.MenuTemplate.Filters;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Database;

public class MenuGeneratorContext(DbContextOptions<MenuGeneratorContext> options) : DbContext(options)
{
	public DbSet<AllergenEntity> Allergens { get; set; }

	public DbSet<DishEntity> Dishes { get; set; }

	public DbSet<DishTypeEntity> DishTypes { get; set; }

	public DbSet<DishAttributeEntity> DishAttributes { get; set; }

	public DbSet<MenuGeneratorTemplateEntity> MenuGeneratorTemplates { get; set; }

	public DbSet<DayMenuTemplateEntity> DayMenuTemplates { get; set; }

	public DbSet<DayMenuDishEntity> DayMenuDishes { get; set; }

	// Filters
	public DbSet<DishFilterEntity> DishFilters { get; set; }

	public DbSet<DishFilterTrueEntity> DishFilterTrues { get; set; }

	public DbSet<DishFilterHasAttributeEntity> DishFilterHasAttributes { get; set; }

	public DbSet<DishFilterWithDependentsEntity> DishFilterWithDependents { get; set; }

	public DbSet<DishFilterOrEntity> DishFilterOrs { get; set; }

	public DbSet<DishFilterAndEntity> DishFilterAnds { get; set; }
}
