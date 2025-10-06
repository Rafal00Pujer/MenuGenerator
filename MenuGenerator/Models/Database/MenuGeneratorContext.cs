using MenuGenerator.Models.Entities.Allergen;
using MenuGenerator.Models.Entities.Dish;
using MenuGenerator.Models.Entities.DishAttribute;
using MenuGenerator.Models.Entities.DishType;
using MenuGenerator.Models.Entities.MenuTemplate;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Database;

public class MenuGeneratorContext(DbContextOptions<MenuGeneratorContext> options) : DbContext(options)
{
	public DbSet<AllergenEntity> Allergens { get; set; }

	public DbSet<DishEntity> Dishes { get; set; }

	public DbSet<DishTypeEntity> DishTypes { get; set; }

	public DbSet<DishAttributeEntity> DishAttributes { get; set; }

	public DbSet<MenuTemplateEntity> Menus { get; set; }
}
