using Application.Common.Models;
using MediatR;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// A use case that changes state and reports only whether it succeeded —
    /// for example <c>DeleteReportCommand</c>, which has nothing further to
    /// report once the soft delete is committed. Dispatched through MediatR's
    /// <c>ISender</c> and handled by exactly one
    /// <see cref="ICommandHandler{TCommand}"/>, which returns the non-generic
    /// <see cref="Result"/>.
    /// </summary>
    public interface ICommand : IRequest<Result>
    {
    }

    /// <summary>
    /// A use case that changes state and, on success, also hands back
    /// <typeparamref name="TResponse"/> — for example
    /// <c>CreateReportRequestCommand</c>, which needs to return the newly
    /// created request's id. Handled by exactly one
    /// <see cref="ICommandHandler{TCommand, TResponse}"/>, which returns
    /// <see cref="Result{TValue}"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of data returned on success.</typeparam>
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
