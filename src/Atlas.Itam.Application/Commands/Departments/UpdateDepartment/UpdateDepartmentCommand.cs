using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Departments;

namespace Atlas.Itam.Application.Commands.Departments.UpdateDepartment;

public sealed record UpdateDepartmentCommand(Guid DepartmentId, string Name) : ICommand<DepartmentDto>;
