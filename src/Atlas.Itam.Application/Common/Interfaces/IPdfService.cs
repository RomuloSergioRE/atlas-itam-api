namespace Atlas.Itam.Application.Common.Interfaces;

public interface IPdfService
{
    byte[] GenerateDeliveryTerm(
        string assetName,
        string patrimonyNumber,
        string serialNumber,
        string userName,
        string userEmail,
        string departmentName,
        DateTime deliveryDate);
}
