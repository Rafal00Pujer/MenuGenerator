using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

public class DishFilterOrEntityConfiguration : IEntityTypeConfiguration<DishFilterOrEntity>
{
	public void Configure(EntityTypeBuilder<DishFilterOrEntity> builder)
	{
	}
}
