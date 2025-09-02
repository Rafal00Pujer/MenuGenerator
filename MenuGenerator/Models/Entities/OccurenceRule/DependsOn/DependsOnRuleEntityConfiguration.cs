using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuGenerator.Models.Entities.OccurenceRule.DependsOn;

public class DependsOnRuleEntityConfiguration : IEntityTypeConfiguration<DependsOnRuleEntity>
{
    public void Configure(EntityTypeBuilder<DependsOnRuleEntity> builder)
    {
        builder.HasBaseType<OccurenceRuleEntity>();

        builder
            .HasMany(e => e.DependsOnDishList)
            .WithMany();
    }
}