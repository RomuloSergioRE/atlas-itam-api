using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Domain.Interfaces;

namespace Atlas.Itam.Application.Commands.Departments.DeleteDepartment;

public sealed class DeleteDepartmentCommandHandler : ICommandHandler<DeleteDepartmentCommand>
{
    private readonly IDepartmentRepository _departmentRepository;

    public DeleteDepartmentCommandHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Department not found");

        await _departmentRepository.DeleteAsync(department, cancellationToken);
    }
}
