using System;
using System.Collections.Generic;
using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.Menu;

[EntityTypeConfiguration<MenuEntityConfiguration, MenuEntity>]
public class MenuEntity
{
    public required DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public HashSet<DishEntity> DishList { get; set; } = new(new MenuDishEqualityComparer());
}