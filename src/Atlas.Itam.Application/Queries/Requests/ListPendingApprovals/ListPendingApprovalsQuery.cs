using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Requests;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Requests.ListPendingApprovals;

public sealed record ListPendingApprovalsQuery(
    Guid DepartmentId
) : IQuery<List<RequestDto>>;
