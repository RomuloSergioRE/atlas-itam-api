using Atlas.Itam.Domain.ValueObjects;

namespace Atlas.Itam.Domain.Tests;

[TestClass]
public class ValueObjectTests
{
    [TestMethod]
    public void PatrimonyNumber_Create_ShouldReturnValid()
    {
        var patrimony = PatrimonyNumber.Create("PAT-001");
        Assert.AreEqual("PAT-001", patrimony.Value);
    }

    [TestMethod]
    public void PatrimonyNumber_Create_ShouldTrim()
    {
        var patrimony = PatrimonyNumber.Create("  PAT-001  ");
        Assert.AreEqual("PAT-001", patrimony.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PatrimonyNumber_Create_ShouldThrowOnEmpty()
    {
        PatrimonyNumber.Create("");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PatrimonyNumber_Create_ShouldThrowOnTooLong()
    {
        PatrimonyNumber.Create(new string('A', 51));
    }

    [TestMethod]
    public void PatrimonyNumber_Equals_ShouldBeCaseInsensitive()
    {
        var p1 = PatrimonyNumber.Create("PAT-001");
        var p2 = PatrimonyNumber.Create("pat-001");
        Assert.IsTrue(p1 == p2);
    }

    [TestMethod]
    public void SerialNumber_Create_ShouldReturnValid()
    {
        var serial = SerialNumber.Create("SN-12345");
        Assert.AreEqual("SN-12345", serial.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void SerialNumber_Create_ShouldThrowOnEmpty()
    {
        SerialNumber.Create("");
    }

    [TestMethod]
    public void Money_Create_ShouldReturnValid()
    {
        var money = Money.Create(1500.50m, "BRL");
        Assert.AreEqual(1500.50m, money.Amount);
        Assert.AreEqual("BRL", money.Currency);
    }

    [TestMethod]
    public void Money_Create_ShouldRoundToTwoDecimals()
    {
        var money = Money.Create(1500.555m, "BRL");
        Assert.AreEqual(1500.56m, money.Amount);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Money_Create_ShouldThrowOnZero()
    {
        Money.Create(0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Money_Create_ShouldThrowOnInvalidCurrency()
    {
        Money.Create(100, "BR");
    }

    [TestMethod]
    public void Money_Add_ShouldSumAmounts()
    {
        var m1 = Money.Create(100, "BRL");
        var m2 = Money.Create(200, "BRL");
        var result = m1.Add(m2);
        Assert.AreEqual(300m, result.Amount);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Money_Add_ShouldThrowOnDifferentCurrencies()
    {
        var m1 = Money.Create(100, "BRL");
        var m2 = Money.Create(100, "USD");
        m1.Add(m2);
    }
}
