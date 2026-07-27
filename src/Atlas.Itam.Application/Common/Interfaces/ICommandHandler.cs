namespace Atlas.Itam.Application.Common.Interfaces;

public interface ICommandHandler<in TCommand, TResponse>
    : MediatR.IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;
