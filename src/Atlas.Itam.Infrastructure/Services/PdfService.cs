using Atlas.Itam.Application.Common.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Atlas.Itam.Infrastructure.Services;

public sealed class PdfService : IPdfService
{
    public byte[] GenerateDeliveryTerm(
        string assetName,
        string patrimonyNumber,
        string serialNumber,
        string userName,
        string userEmail,
        string departmentName,
        DateTime deliveryDate)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().AlignCenter().Text("Termo de Responsabilidade de Equipamento")
                    .Bold().FontSize(16);

                page.Content().Column(col =>
                {
                    col.Item().PaddingVertical(10).Text($"Data: {deliveryDate:dd/MM/yyyy}");

                    col.Item().PaddingVertical(5).Text("Dados do Equipamento:").Bold();
                    col.Item().PaddingLeft(10).Text($"Nome: {assetName}");
                    col.Item().PaddingLeft(10).Text($"Patrimônio: {patrimonyNumber}");
                    col.Item().PaddingLeft(10).Text($"Serial: {serialNumber}");

                    col.Item().PaddingVertical(5).Text("Dados do Colaborador:").Bold();
                    col.Item().PaddingLeft(10).Text($"Nome: {userName}");
                    col.Item().PaddingLeft(10).Text($"E-mail: {userEmail}");
                    col.Item().PaddingLeft(10).Text($"Departamento: {departmentName}");

                    col.Item().PaddingVertical(15).Text(
                        "Declaro que recebi o equipamento acima descrito em perfeitas condições de uso. " +
                        "Estou ciente de que sou responsável por sua guarda e conservação, e que qualquer " +
                        "dano ou extravio será de minha responsabilidade.");

                    col.Item().PaddingVertical(30).AlignCenter()
                        .Text("___________________________________");
                    col.Item().AlignCenter().Text("Assinatura do Colaborador");
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Atlas ITAM - ");
                    x.CurrentPageNumber();
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }
}
