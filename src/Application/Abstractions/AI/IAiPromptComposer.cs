using Domain.Entities;

namespace Application.Abstractions.AI
{
    /// <summary>
    /// Turns a submitted <see cref="ReportRequest"/> and the currently active
    /// <see cref="ReportTemplate"/> into the concrete
    /// <see cref="AiGenerationRequest"/> sent to a provider — substituting the
    /// template's <c>{{Topics}}</c>/<c>{{Criteria}}</c>/<c>{{Audience}}</c>
    /// placeholders (see
    /// <see cref="Domain.ValueObjects.UserPromptTemplateText.RequiredPlaceholders"/>)
    /// with the request's actual topics, criteria, and audience.
    ///
    /// This composition is pure — no I/O, no external calls — so unlike the
    /// rest of <c>Application.Abstractions.AI</c> it is expected to be
    /// implemented directly in Application itself (see the not-yet-built
    /// <c>Application.Policies.ReportPromptComposer</c>), with no
    /// Infrastructure adapter needed at all (see architecture-plan.md,
    /// "AI Design": "Prompt composition is pure and can live in
    /// Application"). The interface still lives alongside the other AI ports
    /// so a command handler depends on an abstraction here exactly as it does
    /// for every other AI concern.
    /// </summary>
    public interface IAiPromptComposer
    {
        AiGenerationRequest Compose(ReportRequest reportRequest, ReportTemplate template);
    }
}
