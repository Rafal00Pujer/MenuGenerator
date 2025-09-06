using System;
using System.Collections.Generic;
using MenuGenerator.Models.Entities.Allergen;
using MenuGenerator.Models.Entities.DishAttribute;
using MenuGenerator.Models.Entities.DishType;
using MenuGenerator.Models.Entities.Menu;
using MenuGenerator.Models.Entities.OccurenceRule;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.Dish;

[EntityTypeConfiguration<DishEntityConfiguration, DishEntity>]
public class DishEntity
{
    public required Guid Id { get; set; } = Guid.CreateVersion7();

    public required string Name { get; set; } = null!;

    public string? Description { get; set; }

    public required bool IncludeInNewMenus { get; set; }
    
    public required Guid TypeId { get; set; }
    
    public required DishTypeEntity Type { get; set; } = null!;
    
    public List<DishAttributeEntity> AttributeList { get; set; } = [];
    
    public List<AllergenEntity> AllergenList { get; set; } = [];

    public List<MenuEntity> MenuList { get; set; } = [];

    public List<OccurenceRuleEntity> OccurenceRuleList { get; set; } = [];
}