using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Dashboard;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Audit.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler : IQueryHandler<GetAuditLogsQuery, List<AuditLogDto>>
{
    private readonly IAuditRepository _auditRepository;
    private readonly IMapper _mapper;

    public GetAuditLogsQueryHandler(IAuditRepository auditRepository, IMapper mapper)
    {
        _auditRepository = auditRepository;
        _mapper = mapper;
    }

    public async Task<List<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Atlas.Itam.Domain.Entities.AuditLog> logs;

        if (request.From.HasValue && request.To.HasValue)
            logs = await _auditRepository.GetByDateRangeAsync(request.From.Value, request.To.Value, cancellationToken);
        else if (request.UserId.HasValue)
            logs = await _auditRepository.GetByUserAsync(request.UserId.Value, cancellationToken);
        else if (request.Action.HasValue)
            logs = await _auditRepository.GetByActionAsync(request.Action.Value, cancellationToken);
        else
            logs = await _auditRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<List<AuditLogDto>>(logs);
    }
}
