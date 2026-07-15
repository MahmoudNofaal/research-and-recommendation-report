namespace Domain.Enums
{
    /// <summary>
    /// Describes the analytical shape of a report request. The mode controls
    /// which domain invariants apply before generation and which report sections
    /// are expected during quality evaluation.
    /// </summary>
    public enum ReportMode
    {
        /// <summary>
        /// A focused research report about one topic. Additional topics may be
        /// included when they provide context, but only one topic is required.
        /// </summary>
        SingleTopicResearch = 1,

        /// <summary>
        /// A report that compares two or more topics and recommends the best
        /// option for different scenarios.
        /// </summary>
        Comparison = 2
    }
}
