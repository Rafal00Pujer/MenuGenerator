using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

public class DishFilterHasAttributeEntityConfiguration : IEntityTypeConfiguration<DishFilterHasAttributeEntity>
{
	public void Configure(EntityTypeBuilder<DishFilterHasAttributeEntity> builder)
	{
		builder.Property(x => x.Negate).IsRequired();

		builder.HasOne(x => x.Attribute)
			   .WithMany(x => x.FilterList)
			   .HasForeignKey(x => x.AttributeId)
			   .HasPrincipalKey(x => x.Id)
			   .IsRequired()
			   .OnDelete(DeleteBehavior.NoAction);
	}
}
