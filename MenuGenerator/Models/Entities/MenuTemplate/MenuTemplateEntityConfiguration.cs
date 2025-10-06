using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.MenuTemplate;

public class MenuTemplateEntityConfiguration : IEntityTypeConfiguration<MenuTemplateEntity>
{
	public void Configure(EntityTypeBuilder<MenuTemplateEntity> builder)
	{
		
	}
}
