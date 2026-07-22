namespace Application.Abstractions.Auth
{
    /// <summary>
    /// Sends transactional email on behalf of Application features. The concrete
    /// adapter may use SMTP or a third-party service, but Application never knows
    /// which transport was selected.
    /// </summary>
    public interface IEmailSender
    {
        Task SendAsync(EmailMessage message, CancellationToken cancellationToken);
    }

    public sealed record EmailMessage
    (
        string To,
        string Subject,
        string HtmlBody,
        string PlainTextBody
    );
}
