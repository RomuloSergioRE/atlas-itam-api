using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;

namespace Atlas.Itam.Application.Commands.Requests.CreateRequest;

public sealed record CreateRequestCommand(
    string Justification,
    Guid AssetId,
    Guid RequestedById
) : ICommand<RequestDto>;
