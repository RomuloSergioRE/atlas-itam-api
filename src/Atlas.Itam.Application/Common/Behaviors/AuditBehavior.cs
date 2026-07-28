using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Domain.Interfaces;
using MediatR;

namespace Atlas.Itam.Application.Common.Behaviors;

public sealed class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuditRepository _auditRepository;

    public AuditBehavior(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        var action = DetermineAction(request);
        if (action.HasValue)
        {
            var entityName = request.GetType().Name.Replace("Command", "").Replace("Query", "");
            var entityId = ExtractEntityId(request, response);

            if (entityId.HasValue)
            {
                var userId = ExtractUserId(request);
                if (userId.HasValue)
                {
                    var auditLog = AuditLog.Create(
                        userId.Value,
                        action.Value,
                        entityName,
                        entityId.Value);

                    await _auditRepository.AddAsync(auditLog, cancellationToken);
                }
            }
        }

        return response;
    }

    private static AuditAction? DetermineAction(object request)
    {
        var typeName = request.GetType().Name;
        return typeName switch
        {
            var n when n.StartsWith("Create") => AuditAction.Create,
            var n when n.StartsWith("Update") => AuditAction.Update,
            var n when n.StartsWith("Delete") || n.StartsWith("Retire") => AuditAction.Delete,
            var n when n.StartsWith("Approve") => AuditAction.Approve,
            var n when n.StartsWith("Reject") => AuditAction.Reject,
            var n when n.StartsWith("Deliver") => AuditAction.Deliver,
            var n when n.StartsWith("Return") => AuditAction.Return,
            var n when n.StartsWith("Login") => AuditAction.Login,
            _ => null
        };
    }

    private static Guid? ExtractEntityId(object request, object response)
    {
        var props = request.GetType().GetProperties();
        foreach (var prop in props)
        {
            var name = prop.Name;
            if (name.EndsWith("Id") && prop.GetValue(request) is Guid guidValue && guidValue != Guid.Empty)
                return guidValue;
        }

        if (response is Guid responseGuid && responseGuid != Guid.Empty)
            return responseGuid;

        return null;
    }

    private static Guid? ExtractUserId(object request)
    {
        var props = request.GetType().GetProperties();
        foreach (var prop in props)
        {
            var name = prop.Name;
            if ((name.Contains("User") || name.Contains("Responsible") || name.Contains("RequestedBy") || name.Contains("ApprovedBy"))
                && prop.PropertyType == typeof(Guid)
                && prop.GetValue(request) is Guid guidValue && guidValue != Guid.Empty)
                return guidValue;
        }

        return null;
    }
}
