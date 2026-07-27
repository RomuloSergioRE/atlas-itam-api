using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;

namespace Atlas.Itam.Application.Queries.Requests.GetRequestById;

public sealed record GetRequestByIdQuery(Guid RequestId) : IQuery<RequestDto>;
