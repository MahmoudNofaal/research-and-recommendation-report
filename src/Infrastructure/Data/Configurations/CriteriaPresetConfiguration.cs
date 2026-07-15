using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps the admin-curated <see cref="CriteriaPreset"/> aggregate root,
    /// including its owned <see cref="CriteriaPreset.SuggestedCriteria"/>
    /// collection.
    /// </summary>
    public sealed class CriteriaPresetConfiguration : IEntityTypeConfiguration<CriteriaPreset>
    {
        public void Configure(EntityTypeBuilder<CriteriaPreset> builder)
        {
            builder.ToTable("CriteriaPresets");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => CriteriaPresetId.From(value))
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .HasMaxLength(CriteriaPreset.MaxNameLength)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(CriteriaPreset.MaxDescriptionLength);

            builder.Property(p => p.Category)
                .HasConversion(category => category.Value, value => PresetCategory.Create(value))
                .HasColumnName("Category")
                .HasMaxLength(PresetCategory.MaxLength)
                .IsRequired();

            builder.Property(p => p.IsActive).IsRequired();
            builder.Property(p => p.SortOrder).IsRequired();

            builder.Navigation(p => p.SuggestedCriteria)
                .HasField("_suggestedCriteria")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // SuggestedCriterion has no identity of its own — a preset's
            // suggestions are always replaced wholesale on edit (see
            // CriteriaPreset.ReplaceSuggestedCriteria) — so a JSON column
            // (matching the CriteriaJson column already sketched in the
            // architecture plan) fits better than a child table with a
            // synthetic key.
            builder.OwnsMany(p => p.SuggestedCriteria, suggested =>
            {
                suggested.ToJson("CriteriaJson");

                suggested.Property(s => s.Name)
                    .HasConversion(name => name.Value, value => ReportCriterionName.Create(value))
                    .HasMaxLength(ReportCriterionName.MaxLength)
                    .IsRequired();

                suggested.Property(s => s.Description)
                    .HasMaxLength(SuggestedCriterion.MaxDescriptionLength)
                    .IsRequired();
            });

            builder.HasIndex(p => p.IsActive);
            builder.HasIndex(p => p.Category);

            builder.Ignore(p => p.DomainEvents);
        }
    }
}