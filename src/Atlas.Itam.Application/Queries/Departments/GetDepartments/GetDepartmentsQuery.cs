using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Queries.Departments.GetDepartments;

public sealed record GetDepartmentsQuery() : IQuery<IReadOnlyList<Atlas.Itam.Application.DTOs.Departments.DepartmentDto>>;
