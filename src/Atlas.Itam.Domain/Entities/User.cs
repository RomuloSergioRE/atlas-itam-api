using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Entities;

public sealed class User
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public Guid DepartmentId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public Department Department { get; private set; } = null!;

    private User() { }

    public static User Create(
        string name,
        string email,
        string passwordHash,
        UserRole role,
        Guid departmentId)
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            DepartmentId = departmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string email, UserRole role, Guid departmentId)
    {
        Name = name;
        Email = email;
        Role = role;
        DepartmentId = departmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
