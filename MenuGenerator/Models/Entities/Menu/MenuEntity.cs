using System;
using System.Collections.Generic;
using MenuGenerator.Models.Entities.Dish;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.Menu;

[EntityTypeConfiguration<MenuEntityConfiguration, MenuEntity>]
public class MenuEntity
{
    internal static string DishListFieldName => nameof(_dishList);
    
    private List<DishEntity> _dishList = [];

    public required DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public IReadOnlyList<DishEntity> DishList => _dishList.AsReadOnly();
}