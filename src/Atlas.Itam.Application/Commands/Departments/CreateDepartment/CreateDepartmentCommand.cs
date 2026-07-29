using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Departments;

namespace Atlas.Itam.Application.Commands.Departments.CreateDepartment;

public sealed record CreateDepartmentCommand(string Name) : ICommand<DepartmentDto>;
