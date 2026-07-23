namespace Atlas.Itam.Domain.Entities;

public sealed class Department
{
    public Guid DepartmentId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Department() { }

    public static Department Create(string name)
    {
        return new Department
        {
            DepartmentId = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name)
    {
        Name = name;
    }
}
