using Atlas.Itam.Application.Common.Interfaces;

namespace Atlas.Itam.Application.Commands.Departments.DeleteDepartment;

public sealed record DeleteDepartmentCommand(Guid Id) : ICommand;
