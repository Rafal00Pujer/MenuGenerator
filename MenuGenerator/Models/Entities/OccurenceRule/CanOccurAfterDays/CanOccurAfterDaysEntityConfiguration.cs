using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.OccurenceRule.CanOccurAfterDays;

public class CanOccurAfterDaysEntityConfiguration : IEntityTypeConfiguration<CanOccurAfterDaysEntity>
{
	public void Configure(EntityTypeBuilder<CanOccurAfterDaysEntity> builder)
	{
		builder.HasBaseType<OccurenceRuleEntity>();
	}
}
