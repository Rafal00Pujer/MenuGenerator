using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

public class DishFilterEntityConfiguration : IEntityTypeConfiguration<DishFilterEntity>
{
	public void Configure(EntityTypeBuilder<DishFilterEntity> builder)
	{
		builder.HasKey(e => e.Id);
	}
}
