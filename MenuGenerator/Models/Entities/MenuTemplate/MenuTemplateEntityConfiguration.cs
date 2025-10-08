using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate;

public class MenuTemplateEntityConfiguration : IEntityTypeConfiguration<MenuGeneratorTemplateEntity>
{
	public void Configure(EntityTypeBuilder<MenuGeneratorTemplateEntity> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Ignore(x => x.DayTemplatesReadOnly);

		builder.Property(x => x.Name).HasMaxLength(50).IsRequired();

		builder.OwnsMany<OrderedDayMenuTemplateEntity>
		(
			MenuGeneratorTemplateEntity.DayTemplatesName, x =>
			{
				x.WithOwner().HasForeignKey(y => y.MenuGeneratorTemplateId);

				x.HasKey
				(
					y => new
					{
						y.MenuGeneratorTemplateId, y.Order
					}
				);

				x.HasOne(y => y.DayMenuTemplateEntity)
				 .WithMany()
				 .HasForeignKey(y => y.DayMenuTemplateId)
				 .HasPrincipalKey(y => y.Id)
				 .OnDelete(DeleteBehavior.Cascade)
				 .IsRequired();

				x.Navigation(y => y.DayMenuTemplateEntity).AutoInclude();
			}
		);
	}
}
