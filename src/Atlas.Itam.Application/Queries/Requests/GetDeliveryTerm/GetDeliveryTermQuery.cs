using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Queries.Requests.GetDeliveryTerm;

public sealed record GetDeliveryTermQuery(Guid RequestId) : IQuery<byte[]>;
