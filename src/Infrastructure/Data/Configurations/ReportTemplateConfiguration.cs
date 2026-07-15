using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps the admin-curated <see cref="ReportTemplate"/> aggregate root.
    /// </summary>
    public sealed class ReportTemplateConfiguration : IEntityTypeConfiguration<ReportTemplate>
    {
        public void Configure(EntityTypeBuilder<ReportTemplate> builder)
        {
            builder.ToTable("ReportTemplates");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasConversion(id => id.Value, value => ReportTemplateId.From(value))
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .HasMaxLength(ReportTemplate.MaxNameLength)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(ReportTemplate.MaxDescriptionLength);

            builder.Property(t => t.SystemPrompt)
                .HasConversion(prompt => prompt.Value, value => SystemPromptText.Create(value))
                .HasColumnName("SystemPrompt")
                .HasMaxLength(SystemPromptText.MaxLength)
                .IsRequired();

            builder.Property(t => t.UserPromptTemplate)
                .HasConversion(prompt => prompt.Value, value => UserPromptTemplateText.Create(value))
                .HasColumnName("UserPromptTemplate")
                .HasMaxLength(UserPromptTemplateText.MaxLength)
                .IsRequired();

            builder.Property(t => t.IsActive)
                .IsRequired();

            builder.Property(t => t.CreatedAtUtc).IsRequired();
            builder.Property(t => t.UpdatedAtUtc).IsRequired();

            // Only one template is expected to be active at a time (see
            // ReportTemplate's own remarks on why Activate/Deactivate are
            // guarded), so lookups by active state are a common access path.
            builder.HasIndex(t => t.IsActive);

            builder.Ignore(t => t.DomainEvents);
        }
    }
}