using MenuGenerator.Models.Entities.Allergen;
using MenuGenerator.Models.Entities.Dish;
using MenuGenerator.Models.Entities.DishAttribute;
using MenuGenerator.Models.Entities.DishType;
using MenuGenerator.Models.Entities.Menu;
using MenuGenerator.Models.Entities.OccurenceRule;
using MenuGenerator.Models.Entities.OccurenceRule.CanNotOccurWith;
using MenuGenerator.Models.Entities.OccurenceRule.CanOccurAfterDays;
using MenuGenerator.Models.Entities.OccurenceRule.CanOccurOnlyWith;
using MenuGenerator.Models.Entities.OccurenceRule.DependsOn;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Database;

public class MenuGeneratorContext(DbContextOptions<MenuGeneratorContext> options) : DbContext(options)
{
	public DbSet<AllergenEntity> Allergens { get; set; }

	public DbSet<DishEntity> Dishes { get; set; }

	public DbSet<DishTypeEntity> DishTypes { get; set; }

	public DbSet<DishAttributeEntity> DishAttributes { get; set; }

	public DbSet<MenuEntity> Menus { get; set; }

	public DbSet<OccurenceRuleEntity> OccurenceRules { get; set; }

	public DbSet<CanOccurAfterDaysEntity> CanOccurAfterDaysRules { get; set; }

	public DbSet<DependsOnRuleEntity> DependsOnRules { get; set; }

	public DbSet<CanNotOccurWithEntity> CanNotOccurWithRules { get; set; }

	public DbSet<CanOccurOnlyWithOneOfEntity> CanOccurOnlyWithRules { get; set; }
}
