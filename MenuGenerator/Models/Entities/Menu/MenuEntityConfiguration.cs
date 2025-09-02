using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.Menu;

public class MenuEntityConfiguration : IEntityTypeConfiguration<MenuEntity>
{
    public void Configure(EntityTypeBuilder<MenuEntity> builder)
    {
        builder.HasKey(e => e.Date);

        builder
            .HasMany(e => e.DishList)
            .WithMany(e => e.MenuList);
    }
}