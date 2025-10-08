using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate;

public class DayMenuDishEntityConfiguration : IEntityTypeConfiguration<DayMenuDishEntity>
{
	public void Configure(EntityTypeBuilder<DayMenuDishEntity> builder)
	{
		builder.HasKey(x => x.Id);

		builder.HasOne(x => x.DishType)
			   .WithMany(x => x.DayMenus)
			   .HasForeignKey(x => x.DishTypeId)
			   .HasPrincipalKey(x => x.Id)
			   .OnDelete(DeleteBehavior.NoAction)
			   .IsRequired();

		builder.HasOne(x => x.DishFilter)
			   .WithOne()
			   .OnDelete(DeleteBehavior.Cascade)
			   .IsRequired();

		builder.Navigation(x => x.DishType).AutoInclude();
		builder.Navigation(x => x.DishFilter).AutoInclude();
	}
}
