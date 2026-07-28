using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Tests;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void Create_ShouldSetPropertiesCorrectly()
    {
        var departmentId = Guid.NewGuid();

        var user = User.Create(
            "João Silva",
            "joao@email.com",
            "hashed_password",
            UserRole.Facilities,
            departmentId);

        Assert.AreEqual("João Silva", user.Name);
        Assert.AreEqual("joao@email.com", user.Email);
        Assert.AreEqual("hashed_password", user.PasswordHash);
        Assert.AreEqual(UserRole.Facilities, user.Role);
        Assert.AreEqual(departmentId, user.DepartmentId);
        Assert.IsTrue(user.IsActive);
    }

    [TestMethod]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var user = User.Create("Test", "test@email.com", "hash", UserRole.HR, Guid.NewGuid());

        user.Deactivate();

        Assert.IsFalse(user.IsActive);
    }

    [TestMethod]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var user = User.Create("Test", "test@email.com", "hash", UserRole.HR, Guid.NewGuid());
        user.Deactivate();

        user.Activate();

        Assert.IsTrue(user.IsActive);
    }

    [TestMethod]
    public void Update_ShouldChangeAllFields()
    {
        var user = User.Create("Old Name", "old@email.com", "hash", UserRole.HR, Guid.NewGuid());
        var newDeptId = Guid.NewGuid();

        user.Update("New Name", "new@email.com", UserRole.Admin, newDeptId, true);

        Assert.AreEqual("New Name", user.Name);
        Assert.AreEqual("new@email.com", user.Email);
        Assert.AreEqual(UserRole.Admin, user.Role);
        Assert.AreEqual(newDeptId, user.DepartmentId);
        Assert.IsTrue(user.IsActive);
    }
}
