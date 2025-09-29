using MenuGenerator.Models.Entities.OccurenceRule.DependsOn;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.OccurenceRule.CanOccurOnlyWith;

public class CanOccurOnlyWithOneOfEntityConfiguration : IEntityTypeConfiguration<CanOccurOnlyWithOneOfEntity>
{
	public void Configure(EntityTypeBuilder<CanOccurOnlyWithOneOfEntity> builder)
	{
		builder.HasBaseType<DependsOnRuleEntity>();
	}
}
