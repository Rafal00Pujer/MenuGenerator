using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.Dish;

public class DishEntityConfiguration : IEntityTypeConfiguration<DishEntity>
{
    public void Configure(EntityTypeBuilder<DishEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Name)
            .HasSentinel(string.Empty)
            .HasMaxLength(50)
            .IsRequired(true);

        builder
            .Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder
            .HasMany(e => e.AllergenList)
            .WithMany(e => e.DishList);
    }
}