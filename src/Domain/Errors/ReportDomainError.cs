namespace Domain.Errors
{
    /// <summary>
    /// Named, ubiquitous-language catalogue of the business rule violations that
    /// can occur while working with report requests, generated reports, generation
    /// runs, templates, and presets. Grouped by the aggregate that raises them so
    /// the Application layer can map a specific <see cref="DomainError.Code"/> to a
    /// specific user-facing message without string-matching free text.
    /// </summary>
    public static class ReportDomainError
    {
        public static class ReportRequest
        {
            public static DomainError MinimumTopicsRequired(int minimum) => new
            (
                "ReportRequest.MinimumTopicsRequired",
                $"A report request must include at least {minimum} topic."
            );

            public static DomainError MinimumComparisonTopicsRequired(int minimum) => new
            (
                "ReportRequest.MinimumComparisonTopicsRequired",
                $"A comparison report request must include at least {minimum} topics."
            );

            public static DomainError MaximumTopicsExceeded(int maximum) => new
            (
                "ReportRequest.MaximumTopicsExceeded",
                $"A report request cannot include more than {maximum} topics."
            );

            public static DomainError DuplicateTopicName(string name) => new
            (
                "ReportRequest.DuplicateTopicName",
                $"Topic '{name}' has already been added to this report request."
            );

            public static DomainError TopicNotFound => new
            (
                "ReportRequest.TopicNotFound",
                "The specified topic does not belong to this report request."
            );

            public static DomainError MinimumCriteriaRequired(int minimum) => new
            (
                "ReportRequest.MinimumCriteriaRequired",
                $"A comparison report request must define at least {minimum} evaluation criteria."
            );

            public static DomainError DuplicateCriterionName(string name) => new
            (
                "ReportRequest.DuplicateCriterionName",
                $"Criterion '{name}' has already been added to this report request."
            );

            public static DomainError CriterionNotFound => new
            (
                "ReportRequest.CriterionNotFound",
                "The specified criterion does not belong to this report request."
            );

            public static DomainError AlreadySubmitted => new
            (
                "ReportRequest.AlreadySubmitted",
                "This report request has already been submitted and can no longer be edited."
            );
        }

        public static class GeneratedReport
        {
            public static DomainError ContentTooShort(int minimumLength) => new
            (
                "GeneratedReport.ContentTooShort",
                $"Generated report content must contain at least {minimumLength} characters of substantive Markdown."
            );

            public static DomainError AtLeastOneRecommendationRequired => new
            (
                "GeneratedReport.AtLeastOneRecommendationRequired",
                "A generated report must include at least one scenario-based recommendation."
            );

            public static DomainError CannotModifyDeletedReport => new
            (
                "GeneratedReport.CannotModifyDeletedReport",
                "This report has been deleted and can no longer be modified."
            );

            public static DomainError AlreadyDeleted => new
            (
                "GeneratedReport.AlreadyDeleted",
                "This report has already been deleted."
            );
        }

        public static class ReportGenerationRun
        {
            public static DomainError InvalidStatusTransition(string from, string to) => new
            (
                "ReportGenerationRun.InvalidStatusTransition",
                $"A generation run cannot move from '{from}' to '{to}'."
            );

            public static DomainError ConcreteProviderRequired => new
            (
                "ReportGenerationRun.ConcreteProviderRequired",
                "A generation run must record the specific AI provider that executed it, not a preference placeholder."
            );
        }

        public static class ReportTemplate
        {
            public static DomainError MissingRequiredPlaceholder(string placeholder) => new
            (
                "ReportTemplate.MissingRequiredPlaceholder",
                $"The user prompt template must contain the '{placeholder}' placeholder."
            );

            public static DomainError AlreadyActive => new
            (
                "ReportTemplate.AlreadyActive",
                "This report template is already active."
            );

            public static DomainError AlreadyInactive => new
            (
                "ReportTemplate.AlreadyInactive",
                "This report template is already inactive."
            );
        }

        public static class CriteriaPreset
        {
            public static DomainError AtLeastOneSuggestedCriterionRequired => new
            (
                "CriteriaPreset.AtLeastOneSuggestedCriterionRequired",
                "A criteria preset must suggest at least one criterion."
            );
        }
    }
}
