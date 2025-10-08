using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate;

public class DayMenuTemplateEntityConfiguration : IEntityTypeConfiguration<DayMenuTemplateEntity>
{
	public void Configure(EntityTypeBuilder<DayMenuTemplateEntity> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.TemplateDays).IsRequired();

		builder.HasMany(x => x.DishTypeList)
			   .WithOne()
			   .OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(x => x.DishTypeList).AutoInclude();
	}
}
