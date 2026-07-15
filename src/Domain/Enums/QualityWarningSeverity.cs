namespace Domain.Enums
{
    /// <summary>
    /// How seriously a single quality warning should be taken. A <see cref="Blocking"/>
    /// warning is severe enough, on its own, to keep a <c>GeneratedReport</c> out of
    /// <see cref="ReportStatus.Ready"/> status regardless of its numeric quality score.
    /// </summary>
    public enum QualityWarningSeverity
    {
        Info = 0,
        Warning = 1,
        Blocking = 2
    }
}
