using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps the admin-curated <see cref="ReportStylePreset"/> aggregate root.
    /// </summary>
    public sealed class ReportStylePresetConfiguration : IEntityTypeConfiguration<ReportStylePreset>
    {
        public void Configure(EntityTypeBuilder<ReportStylePreset> builder)
        {
            builder.ToTable("ReportStylePresets");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => ReportStylePresetId.From(value))
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .HasMaxLength(ReportStylePreset.MaxNameLength)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(ReportStylePreset.MaxDescriptionLength);

            builder.Property(p => p.RecommendedAudience)
                .HasConversion(audience => audience.Value, value => TargetAudience.Create(value))
                .HasColumnName("RecommendedAudience")
                .HasMaxLength(TargetAudience.MaxLength)
                .IsRequired();

            builder.Property(p => p.DefaultStyle)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.DefaultDepth)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.IsActive).IsRequired();
            builder.Property(p => p.SortOrder).IsRequired();

            builder.HasIndex(p => p.IsActive);

            builder.Ignore(p => p.DomainEvents);
        }
    }
}