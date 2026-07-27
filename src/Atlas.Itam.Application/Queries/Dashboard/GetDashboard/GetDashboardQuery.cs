using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Dashboard;

namespace Atlas.Itam.Application.Queries.Dashboard.GetDashboard;

public sealed record GetDashboardQuery() : IQuery<DashboardDto>;
