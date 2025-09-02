using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.Allergen;

public class AllergenEntityConfiguration : IEntityTypeConfiguration<AllergenEntity>
{
    public void Configure(EntityTypeBuilder<AllergenEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .HasIndex(e => e.DisplayId)
            .IsUnique(true);

        builder
            .Property(e => e.DisplayId)
            .HasMaxLength(10)
            .HasSentinel(string.Empty)
            .IsRequired(true);

        builder
            .Property(e => e.Description)
            .HasMaxLength(50)
            .IsRequired(false);
    }
}