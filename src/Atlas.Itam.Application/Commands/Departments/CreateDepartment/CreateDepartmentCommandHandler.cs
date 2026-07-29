using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Departments;
using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Commands.Departments.CreateDepartment;

public sealed class CreateDepartmentCommandHandler : ICommandHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public CreateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = Department.Create(request.Name);
        await _departmentRepository.AddAsync(department, cancellationToken);
        return _mapper.Map<DepartmentDto>(department);
    }
}
