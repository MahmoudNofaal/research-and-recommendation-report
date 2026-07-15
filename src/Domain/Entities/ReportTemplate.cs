using Domain.Common;
using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// An admin-curated prompt template: the system prompt and user-prompt
    /// pattern used to compose AI generation requests. Activation is guarded
    /// (redundant activate/deactivate throws) because, unlike the reference-data
    /// toggles on <c>CriteriaPreset</c>/<c>ReportStylePreset</c>, which template is
    /// active directly controls the prompt sent for every live generation — a
    /// state change here should always be a deliberate, singular action.
    /// </summary>
    public sealed class ReportTemplate : AuditableEntity<ReportTemplateId>
    {
        public const int MaxNameLength = 150;
        public const int MaxDescriptionLength = 500;

        private ReportTemplate(
            ReportTemplateId id,
            string name,
            string? description,
            SystemPromptText systemPrompt,
            UserPromptTemplateText userPromptTemplate)
            : base(id)
        {
            Name = name;
            Description = description;
            SystemPrompt = systemPrompt;
            UserPromptTemplate = userPromptTemplate;
            IsActive = true;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportTemplate()
            : base(default!)
        {
            Name = null!;
            SystemPrompt = null!;
            UserPromptTemplate = null!;
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public SystemPromptText SystemPrompt { get; private set; }

        public UserPromptTemplateText UserPromptTemplate { get; private set; }

        public bool IsActive { get; private set; }

        public static ReportTemplate Create(
            string name,
            string? description,
            SystemPromptText systemPrompt,
            UserPromptTemplateText userPromptTemplate)
        {
            ArgumentNullException.ThrowIfNull(systemPrompt);
            ArgumentNullException.ThrowIfNull(userPromptTemplate);

            return new ReportTemplate(
                ReportTemplateId.New(), NormalizeName(name), NormalizeDescription(description), systemPrompt, userPromptTemplate);
        }

        public void UpdateContent(
            string name,
            string? description,
            SystemPromptText systemPrompt,
            UserPromptTemplateText userPromptTemplate)
        {
            ArgumentNullException.ThrowIfNull(systemPrompt);
            ArgumentNullException.ThrowIfNull(userPromptTemplate);

            Name = NormalizeName(name);
            Description = NormalizeDescription(description);
            SystemPrompt = systemPrompt;
            UserPromptTemplate = userPromptTemplate;
        }

        public void Activate()
        {
            if (IsActive)
            {
                throw new InvalidReportStateException(ReportDomainError.ReportTemplate.AlreadyActive);
            }

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new InvalidReportStateException(ReportDomainError.ReportTemplate.AlreadyInactive);
            }

            IsActive = false;
        }

        private static string NormalizeName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var trimmed = name.Trim();
            if (trimmed.Length > MaxNameLength)
            {
                throw new ArgumentOutOfRangeException(nameof(name), $"Template name cannot exceed {MaxNameLength} characters.");
            }

            return trimmed;
        }

        private static string? NormalizeDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            var trimmed = description.Trim();
            if (trimmed.Length > MaxDescriptionLength)
            {
                throw new ArgumentOutOfRangeException(nameof(description), $"Template description cannot exceed {MaxDescriptionLength} characters.");
            }

            return trimmed;
        }
    }
}
