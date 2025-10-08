using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

public class DishFilterTrueEntityConfiguration : IEntityTypeConfiguration<DishFilterTrueEntity>
{
	public void Configure(EntityTypeBuilder<DishFilterTrueEntity> builder)
	{
		builder.HasKey(e => e.Id);
	}
}
