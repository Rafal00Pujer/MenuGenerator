using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.OccurenceRule;

public class OccurenceRuleEntityConfiguration : IEntityTypeConfiguration<OccurenceRuleEntity>
{
	public void Configure(EntityTypeBuilder<OccurenceRuleEntity> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasDiscriminator<string>("RuleType");

		builder
			.HasOne(e => e.Dish)
			.WithMany(e => e.OccurenceRuleList)
			.HasForeignKey(e => e.DishId)
			.IsRequired()
			.OnDelete(DeleteBehavior.NoAction);
	}
}
