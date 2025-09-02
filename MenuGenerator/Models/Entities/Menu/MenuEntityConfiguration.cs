using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.Menu;

public class MenuEntityConfiguration : IEntityTypeConfiguration<MenuEntity>
{
    public void Configure(EntityTypeBuilder<MenuEntity> builder)
    {
        builder.HasKey(e => e.Date);

        var leftNavigation = builder
            .HasMany(e => e.DishList)
            .WithMany(e => e.MenuList)
            .LeftNavigation;

        leftNavigation.SetField(MenuEntity.DishListFieldName);
        leftNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

        // builder
        //     .Property(e => e.DishList)
        //     .HasField(MenuEntity.DishListFieldName)
        //     .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}