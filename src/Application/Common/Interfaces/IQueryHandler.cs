using Application.Common.Models;
using MediatR;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Executes a single <see cref="IQuery{TResponse}"/>.
    /// </summary>
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>
    {
    }
}
