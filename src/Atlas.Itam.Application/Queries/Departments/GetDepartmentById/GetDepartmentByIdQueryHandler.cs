using Atlas.Itam.Application.Common.Interfaces;
using Atlas.Itam.Application.DTOs.Departments;
using Atlas.Itam.Domain.Interfaces;
using AutoMapper;

namespace Atlas.Itam.Application.Queries.Departments.GetDepartmentById;

public sealed class GetDepartmentByIdQueryHandler : IQueryHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public GetDepartmentByIdQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new Atlas.Itam.Domain.Errors.NotFoundError("Department not found");

        return _mapper.Map<DepartmentDto>(department);
    }
}
