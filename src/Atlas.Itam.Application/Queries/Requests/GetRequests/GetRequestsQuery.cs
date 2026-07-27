using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;

namespace Atlas.Itam.Application.Queries.Requests.GetRequests;

public sealed record GetRequestsQuery(
    Guid? UserId = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<List<RequestDto>>;
