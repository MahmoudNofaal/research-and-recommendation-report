using Application.Common.Models;
using MediatR;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// A read-only use case that returns <typeparamref name="TResponse"/>
    /// without changing state. Every query in this system returns data, so
    /// unlike <c>ICommand</c> there is no non-generic counterpart. Handled by
    /// exactly one <see cref="IQueryHandler{TQuery, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of data returned.</typeparam>
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
