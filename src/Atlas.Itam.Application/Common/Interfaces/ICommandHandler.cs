namespace Atlas.Itam.Application.Common.Interfaces;

public interface ICommandHandler<in TCommand, TResponse>
    : MediatR.IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;

public interface ICommandHandler<in TCommand>
    : MediatR.IRequestHandler<TCommand>
    where TCommand : ICommand;
