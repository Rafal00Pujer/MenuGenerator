using MenuGenerator.Models.Entities.OccurenceRule.DependsOn;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.OccurenceRule.CanNotOccurWith;

public class CanNotOccurWithEntityConfiguration : IEntityTypeConfiguration<CanNotOccurWithEntity>
{
	public void Configure(EntityTypeBuilder<CanNotOccurWithEntity> builder)
	{
		builder.HasBaseType<DependsOnRuleEntity>();
	}
}
