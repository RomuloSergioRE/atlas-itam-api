using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Departments;

namespace Atlas.Itam.Application.Queries.Departments.GetDepartmentById;

public sealed record GetDepartmentByIdQuery(Guid Id) : IQuery<DepartmentDto>;
