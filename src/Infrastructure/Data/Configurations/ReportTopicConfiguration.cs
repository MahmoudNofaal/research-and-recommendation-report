using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps <see cref="ReportRequest.Topics"/> as an owned collection stored in
    /// the <c>ReportTopics</c> table. <see cref="ReportTopic"/> has no
    /// repository or independent lifecycle of its own — see its type-level
    /// remarks — so it is configured as an EF Core owned entity rather than a
    /// separate aggregate root with its own configuration class targeting
    /// itself.
    /// </summary>
    public sealed class ReportTopicConfiguration : IEntityTypeConfiguration<ReportRequest>
    {
        public void Configure(EntityTypeBuilder<ReportRequest> builder)
        {
            // ReportRequest.Topics is a read-only view over the private
            // _topics field (Entity/EF materialization uses the private
            // constructors, never a public setter), so EF must be told
            // explicitly which field backs the navigation.
            builder.Navigation(r => r.Topics)
                .HasField("_topics")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany(r => r.Topics, topics =>
            {
                topics.ToTable("ReportTopics");
                topics.WithOwner().HasForeignKey("ReportRequestId");
                topics.HasKey(t => t.Id);

                topics.Property(t => t.Id)
                    .HasConversion(id => id.Value, value => ReportTopicId.From(value))
                    .ValueGeneratedNever();

                topics.Property(t => t.Name)
                    .HasConversion(name => name.Value, value => ReportTopicName.Create(value))
                    .HasColumnName("Name")
                    .HasMaxLength(ReportTopicName.MaxLength)
                    .IsRequired();

                topics.Property(t => t.Description)
                    .HasMaxLength(ReportTopic.MaxDescriptionLength);

                topics.Property(t => t.SortOrder)
                    .IsRequired();

                topics.UsePropertyAccessMode(PropertyAccessMode.Field);
            });
        }
    }
}