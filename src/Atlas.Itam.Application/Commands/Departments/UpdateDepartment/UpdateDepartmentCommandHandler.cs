using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Departments;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Commands.Departments.UpdateDepartment;

public sealed class UpdateDepartmentCommandHandler : ICommandHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public UpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Department not found");

        department.Update(request.Name);
        await _departmentRepository.UpdateAsync(department, cancellationToken);

        return _mapper.Map<DepartmentDto>(department);
    }
}
