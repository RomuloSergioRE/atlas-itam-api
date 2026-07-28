using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Assets;

namespace Atlas.Itam.Application.Queries.Assets.GetStockSummary;

public sealed record GetStockSummaryQuery() : IQuery<StockDto>;
