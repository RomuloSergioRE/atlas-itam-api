using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;

namespace Atlas.Itam.Domain.Tests;

[TestClass]
public class AssetTests
{
    [TestMethod]
    public void Create_ShouldSetPropertiesCorrectly()
    {
        var categoryId = Guid.NewGuid();
        var locationId = Guid.NewGuid();

        var asset = Asset.Create(
            "Dell XPS 15",
            "PAT-001",
            "SN-12345",
            new DateTime(2024, 1, 15),
            5999.99m,
            categoryId,
            locationId,
            "Dell Technologies",
            new DateTime(2026, 1, 15),
            "Test asset");

        Assert.AreEqual("Dell XPS 15", asset.Name);
        Assert.AreEqual("PAT-001", asset.PatrimonyNumber);
        Assert.AreEqual("SN-12345", asset.SerialNumber);
        Assert.AreEqual(5999.99m, asset.AcquisitionValue);
        Assert.AreEqual(AssetStatus.Available, asset.Status);
        Assert.AreEqual(categoryId, asset.CategoryId);
        Assert.AreEqual(locationId, asset.LocationId);
        Assert.AreEqual("Dell Technologies", asset.Supplier);
        Assert.IsFalse(asset.IsDeleted);
        Assert.IsNull(asset.CurrentUserId);
    }

    [TestMethod]
    public void AssignToUser_ShouldSetStatusToInUse()
    {
        var asset = Asset.Create("Test", "PAT-001", "SN-12345", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        var userId = Guid.NewGuid();

        asset.AssignToUser(userId);

        Assert.AreEqual(userId, asset.CurrentUserId);
        Assert.AreEqual(AssetStatus.InUse, asset.Status);
    }

    [TestMethod]
    public void UnassignFromUser_ShouldSetStatusToAvailable()
    {
        var asset = Asset.Create("Test", "PAT-001", "SN-12345", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        asset.AssignToUser(Guid.NewGuid());

        asset.UnassignFromUser();

        Assert.IsNull(asset.CurrentUserId);
        Assert.AreEqual(AssetStatus.Available, asset.Status);
    }

    [TestMethod]
    public void SetInMaintenance_ShouldChangeStatus()
    {
        var asset = Asset.Create("Test", "PAT-001", "SN-12345", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        asset.SetInMaintenance();

        Assert.AreEqual(AssetStatus.InMaintenance, asset.Status);
    }

    [TestMethod]
    public void ReturnFromMaintenance_ShouldChangeStatusToAvailable()
    {
        var asset = Asset.Create("Test", "PAT-001", "SN-12345", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        asset.SetInMaintenance();

        asset.ReturnFromMaintenance();

        Assert.AreEqual(AssetStatus.Available, asset.Status);
    }

    [TestMethod]
    public void Retire_ShouldClearUserAndSetStatus()
    {
        var asset = Asset.Create("Test", "PAT-001", "SN-12345", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());
        asset.AssignToUser(Guid.NewGuid());

        asset.Retire();

        Assert.AreEqual(AssetStatus.Retired, asset.Status);
        Assert.IsNull(asset.CurrentUserId);
    }

    [TestMethod]
    public void SoftDelete_ShouldSetIsDeleted()
    {
        var asset = Asset.Create("Test", "PAT-001", "SN-12345", DateTime.Now, 1000, Guid.NewGuid(), Guid.NewGuid());

        asset.SoftDelete();

        Assert.IsTrue(asset.IsDeleted);
    }
}
