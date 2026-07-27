using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Atlas.Itam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Itam.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (await context.Departments.AnyAsync())
            return;

        // Departments
        var departments = new[]
        {
            Department.Create("Tecnologia da Informação"),
            Department.Create("Recursos Humanos"),
            Department.Create("Financeiro"),
            Department.Create("Operações"),
            Department.Create("Marketing"),
            Department.Create("Administrativo")
        };
        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync();

        // Locations
        var locations = new[]
        {
            Location.Create("Sede SP - Matriz", "Av. Paulista, 1000 - São Paulo, SP"),
            Location.Create("Sede RJ - Filial", "Rua das Laranjeiras, 200 - Rio de Janeiro, RJ")
        };
        await context.Locations.AddRangeAsync(locations);
        await context.SaveChangesAsync();

        // Categories
        var categories = new[]
        {
            AssetCategory.Create("Notebook", "Computador portátil"),
            AssetCategory.Create("Desktop", "Computador de mesa"),
            AssetCategory.Create("Monitor", "Tela/monitor"),
            AssetCategory.Create("Celular", "Smartphones corporativos"),
            AssetCategory.Create("Tablet", "Tablets corporativos"),
            AssetCategory.Create("Dock Station", "Estações de acoplamento"),
            AssetCategory.Create("Impressora", "Impressoras e multifuncionais"),
            AssetCategory.Create("Servidor", "Servidores físicos"),
            AssetCategory.Create("Switch", "Switches de rede"),
            AssetCategory.Create("Firewall", "Dispositivos de segurança"),
            AssetCategory.Create("Roteador", "Roteadores de rede"),
            AssetCategory.Create("Licença de Software", "Licenças de uso"),
            AssetCategory.Create("Periférico", "Mouses, teclados, headsets, webcams")
        };
        await context.AssetCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Admin user (senha: admin123 - hash bcrypt)
        var itDepartment = await context.Departments
            .FirstAsync(d => d.Name == "Tecnologia da Informação");

        var adminUser = User.Create(
            "Administrador",
            "admin@atlasitam.com",
            "$2a$11$YQ8G3Z6K5X5Y5Y5Y5Y5Y5O5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5",
            UserRole.Admin,
            itDepartment.DepartmentId);

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}
