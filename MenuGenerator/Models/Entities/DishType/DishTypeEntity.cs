using System;
using System.Collections.Generic;
using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.DishType;

[EntityTypeConfiguration<DishTypeEntityConfiguration, DishTypeEntity>]
public class DishTypeEntity
{
    public required Guid Id { get; set; } = Guid.CreateVersion7();

    public required string Name { get; set; } = null!;

    public string? Description { get; set; }

    public List<DishEntity> DishList { get; set; } = [];
}