namespace Atlas.Itam.Application.DTOs.Departments;

public sealed class DepartmentDto
{
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
