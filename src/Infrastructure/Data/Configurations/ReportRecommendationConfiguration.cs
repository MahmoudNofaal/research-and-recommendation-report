using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps <see cref="GeneratedReport.Recommendations"/> as an owned
    /// collection stored in the <c>ReportRecommendations</c> table. Owned
    /// exclusively by its parent report and replaced wholesale on
    /// regeneration.
    /// </summary>
    public sealed class ReportRecommendationConfiguration : IEntityTypeConfiguration<GeneratedReport>
    {
        public void Configure(EntityTypeBuilder<GeneratedReport> builder)
        {
            builder.Navigation(r => r.Recommendations)
                .HasField("_recommendations")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany(r => r.Recommendations, recommendations =>
            {
                recommendations.ToTable("ReportRecommendations");
                recommendations.WithOwner().HasForeignKey("GeneratedReportId");
                recommendations.HasKey(r => r.Id);

                recommendations.Property(r => r.Id)
                    .HasConversion(id => id.Value, value => ReportRecommendationId.From(value))
                    .ValueGeneratedNever();

                recommendations.Property(r => r.Scenario)
                    .HasMaxLength(ReportRecommendation.MaxScenarioLength)
                    .IsRequired();

                recommendations.Property(r => r.RecommendedOption)
                    .HasMaxLength(ReportRecommendation.MaxRecommendedOptionLength)
                    .IsRequired();

                recommendations.Property(r => r.Reasoning)
                    .HasMaxLength(ReportRecommendation.MaxReasoningLength)
                    .IsRequired();

                recommendations.Property(r => r.Strength)
                    .HasConversion<int>()
                    .IsRequired();

                recommendations.Property(r => r.SortOrder)
                    .IsRequired();

                recommendations.UsePropertyAccessMode(PropertyAccessMode.Field);
            });
        }
    }
}