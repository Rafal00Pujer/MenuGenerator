using System;
using System.Collections.Generic;
using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.Allergen;

[EntityTypeConfiguration<AllergenEntityConfiguration, AllergenEntity>]
public class AllergenEntity
{
	public required Guid Id { get; set; } = Guid.CreateVersion7();

	public required string DisplayId { get; set; } = null!;

	public string? Description { get; set; }

	public List<DishEntity> DishList { get; set; } = [];
}
