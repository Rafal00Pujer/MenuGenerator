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
            .IsRequired();

        builder
            .Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder
            .HasOne(e => e.Type)
            .WithMany(e => e.DishList)
            .HasForeignKey(e => e.TypeId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(e => e.AttributeList)
            .WithMany(e => e.DishList);

        builder
            .HasMany(e => e.AllergenList)
            .WithMany(e => e.DishList);
    }
}