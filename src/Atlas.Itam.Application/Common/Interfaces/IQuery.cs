using MediatR;

namespace Atlas.Itam.Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse> { }
