using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.DishAttribute;

public class DishAttributeEntityConfiguration : IEntityTypeConfiguration<DishAttributeEntity>
{
	public void Configure(EntityTypeBuilder<DishAttributeEntity> builder)
	{
		builder.HasKey(e => e.Id);

		builder
			.HasIndex(e => e.Name)
			.IsUnique();

		builder
			.Property(e => e.Name)
			.HasSentinel(string.Empty)
			.HasMaxLength(50)
			.IsRequired();

		builder
			.Property(e => e.Description)
			.HasMaxLength(500)
			.IsRequired(false);
	}
}
