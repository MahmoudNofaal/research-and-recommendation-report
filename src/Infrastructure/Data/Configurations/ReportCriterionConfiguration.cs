using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps <see cref="ReportRequest.Criteria"/> as an owned collection stored
    /// in the <c>ReportCriteria</c> table, mirroring
    /// <see cref="ReportTopicConfiguration"/> for <see cref="ReportCriterion"/>.
    /// </summary>
    public sealed class ReportCriterionConfiguration : IEntityTypeConfiguration<ReportRequest>
    {
        public void Configure(EntityTypeBuilder<ReportRequest> builder)
        {
            builder.Navigation(r => r.Criteria)
                .HasField("_criteria")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany(r => r.Criteria, criteria =>
            {
                criteria.ToTable("ReportCriteria");
                criteria.WithOwner().HasForeignKey("ReportRequestId");
                criteria.HasKey(c => c.Id);

                criteria.Property(c => c.Id)
                    .HasConversion(id => id.Value, value => ReportCriterionId.From(value))
                    .ValueGeneratedNever();

                criteria.Property(c => c.Name)
                    .HasConversion(name => name.Value, value => ReportCriterionName.Create(value))
                    .HasColumnName("Name")
                    .HasMaxLength(ReportCriterionName.MaxLength)
                    .IsRequired();

                criteria.Property(c => c.Description)
                    .HasMaxLength(ReportCriterion.MaxDescriptionLength);

                criteria.Property(c => c.Weight)
                    .HasConversion(weight => weight.Value, value => CriterionWeight.Create(value))
                    .HasColumnName("Weight")
                    .IsRequired();

                criteria.Property(c => c.SortOrder)
                    .IsRequired();

                criteria.UsePropertyAccessMode(PropertyAccessMode.Field);
            });
        }
    }
}