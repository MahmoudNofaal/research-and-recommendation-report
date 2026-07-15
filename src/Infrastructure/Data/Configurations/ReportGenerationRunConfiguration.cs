using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps the <see cref="ReportGenerationRun"/> aggregate root: its own
    /// scalar state plus the small owned value objects
    /// (<see cref="GenerationTiming"/>, <see cref="TokenUsage"/>,
    /// <see cref="ErrorDetail"/>) that only ever exist attached to a single
    /// run and never need a table of their own.
    /// </summary>
    public sealed class ReportGenerationRunConfiguration : IEntityTypeConfiguration<ReportGenerationRun>
    {
        public void Configure(EntityTypeBuilder<ReportGenerationRun> builder)
        {
            builder.ToTable("ReportGenerationRuns");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasConversion(id => id.Value, value => ReportGenerationRunId.From(value))
                .ValueGeneratedNever();

            builder.Property(r => r.ReportRequestId)
                .HasConversion(id => id.Value, value => ReportRequestId.From(value))
                .IsRequired();

            builder.Property(r => r.GeneratedReportId)
                .HasConversion(id => id.Value, value => GeneratedReportId.From(value))
                .IsRequired();

            builder.Property(r => r.UserId)
                .HasConversion(id => id.Value, value => UserId.From(value))
                .IsRequired();

            builder.Property(r => r.AiProvider)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.ModelName)
                .HasMaxLength(ReportGenerationRun.MaxModelNameLength)
                .IsRequired();

            builder.Property(r => r.PromptVersion)
                .HasConversion(version => version.Value, value => PromptVersion.Create(value))
                .HasMaxLength(PromptVersion.MaxLength)
                .IsRequired();

            builder.Property(r => r.PromptText)
                .HasMaxLength(ReportGenerationRun.MaxPromptTextLength)
                .IsRequired();

            builder.Property(r => r.RawResponse)
                .HasMaxLength(ReportGenerationRun.MaxRawResponseLength);

            builder.Property(r => r.Status)
                .HasConversion<int>()
                .IsRequired();

            // Timing, TokenUsage, and ErrorDetail are all optional — a run
            // that never got past Pending has no Timing yet, and only a
            // failed run has an ErrorDetail — so each is mapped as an
            // optional owned reference rather than a required one.
            builder.OwnsOne(r => r.Timing, timing =>
            {
                timing.Property(t => t.StartedAtUtc).HasColumnName("StartedAtUtc");
                timing.Property(t => t.CompletedAtUtc).HasColumnName("CompletedAtUtc");
            });
            builder.Navigation(r => r.Timing).IsRequired(false);

            builder.OwnsOne(r => r.TokenUsage, tokenUsage =>
            {
                tokenUsage.Property(t => t.InputTokens).HasColumnName("InputTokens");
                tokenUsage.Property(t => t.OutputTokens).HasColumnName("OutputTokens");
            });
            builder.Navigation(r => r.TokenUsage).IsRequired(false);

            builder.OwnsOne(r => r.ErrorDetail, errorDetail =>
            {
                errorDetail.Property(e => e.ErrorCode).HasColumnName("ErrorCode");

                errorDetail.Property(e => e.ErrorMessage)
                    .HasColumnName("ErrorMessage")
                    .HasMaxLength(ErrorDetail.MaxMessageLength);
            });
            builder.Navigation(r => r.ErrorDetail).IsRequired(false);

            builder.HasIndex(r => r.ReportRequestId);
            builder.HasIndex(r => r.GeneratedReportId);
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.Status);

            builder.Ignore(r => r.DomainEvents);
        }
    }
}