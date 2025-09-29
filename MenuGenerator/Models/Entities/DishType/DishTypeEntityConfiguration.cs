using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.DishType;

public class DishTypeEntityConfiguration : IEntityTypeConfiguration<DishTypeEntity>
{
	public void Configure(EntityTypeBuilder<DishTypeEntity> builder)
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
