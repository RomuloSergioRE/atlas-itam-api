using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Queries.Audit.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    DateTime? From = null,
    DateTime? To = null,
    int Page = 1,
    int PageSize = 50
) : IQuery<List<Atlas.Itam.Application.DTOs.Dashboard.AuditLogDto>>;
