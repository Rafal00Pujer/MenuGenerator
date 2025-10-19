using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

public class DishFilterWithDependentsEntityConfiguration : IEntityTypeConfiguration<DishFilterWithDependentsEntity>
{
	public void Configure(EntityTypeBuilder<DishFilterWithDependentsEntity> builder)
	{
		builder.HasMany(x => x.DependentFilters)
			   .WithOne()
			   .IsRequired()
			   .OnDelete(DeleteBehavior.Cascade);
		
		builder.Navigation(x => x.DependentFilters).AutoInclude();
	}
}
