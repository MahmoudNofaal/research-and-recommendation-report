using Application.Common.Models;
using MediatR;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Executes a single <see cref="ICommand"/>. Exactly one implementation is
    /// expected to be registered per <typeparamref name="TCommand"/> — MediatR
    /// resolves it the same way it would any
    /// <see cref="IRequestHandler{TRequest, TResponse}"/>; this interface
    /// exists only so a handler class reads as "handles this command" rather
    /// than "handles this MediatR request".
    /// </summary>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
    }

    /// <summary>
    /// Executes a single <see cref="ICommand{TResponse}"/>.
    /// </summary>
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }
}
