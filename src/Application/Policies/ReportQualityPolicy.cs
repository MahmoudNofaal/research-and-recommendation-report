using Domain.Entities;
using Domain.Enums;
using Domain.Services;
using Domain.ValueObjects;

namespace Application.Policies
{
    /// <summary>
    /// A thin Application-facing entry point onto
    /// <see cref="ReportQualityDomainService"/>: handlers only ever have a
    /// <see cref="GeneratedReport"/> alongside the <see cref="ReportRequest"/>
    /// it came from, never the bare <see cref="ReportMode"/> and topic count
    /// the Domain service asks for directly, so this derives both from the
    /// request rather than every call site repeating that translation.
    ///
    /// Also centralizes the one presentation-facing quality rule that isn't
    /// really a scoring concern at all: the fixed severity order — Blocking,
    /// then Warning, then Info — every warning list is shown in throughout
    /// the UI (see ui-ux-specification.md, "8.8 Reports/Preview").
    /// </summary>
    public static class ReportQualityPolicy
    {
        public static (ReportQualityScore Score, IReadOnlyList<QualityWarning> Warnings) Evaluate
        (
            GeneratedReport report,
            ReportRequest reportRequest
        )
        {
            ArgumentNullException.ThrowIfNull(report);
            ArgumentNullException.ThrowIfNull(reportRequest);

            var (score, warnings) = ReportQualityDomainService.Evaluate
            (
                report,
                reportRequest.ReportMode,
                reportRequest.Topics.Count
            );

            return (score, OrderBySeverity(warnings));
        }

        /// <summary>
        /// Sorts <see cref="QualityWarningSeverity.Blocking"/> warnings
        /// first, then <see cref="QualityWarningSeverity.Warning"/>, then
        /// <see cref="QualityWarningSeverity.Info"/> — the order the Preview
        /// page's warnings panel always presents them in, regardless of the
        /// order <see cref="ReportQualityDomainService"/> happened to add
        /// them in.
        /// </summary>
        public static IReadOnlyList<QualityWarning> OrderBySeverity(IReadOnlyList<QualityWarning> warnings)
        {
            ArgumentNullException.ThrowIfNull(warnings);

            return warnings
                .OrderByDescending(warning => warning.Severity)
                .ToList();
        }
    }
}
