using Fuse8.BackendInternship.PublicApi.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fuse8.BackendInternship.PublicApi.DataAccess.DbContextsFactories;

public class FavoriteCurrencyContextFactory : IDesignTimeDbContextFactory<FavoriteCurrencyContext>
{
    public FavoriteCurrencyContext CreateDbContext(string[] args)
    {
        var basePath = GetProjectRoot();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<FavoriteCurrencyContext>();

        var connectionString = configuration.GetConnectionString("CurrencyDb");

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "cur");
        });

        return new FavoriteCurrencyContext(optionsBuilder.Options);
    }
    
    private static string GetProjectRoot()
    {
        var dir = Directory.GetCurrentDirectory();

        while (!string.IsNullOrEmpty(dir) && !Directory.GetFiles(dir, "*.csproj").Any())
        {
            dir = Directory.GetParent(dir)?.FullName!;
        }

        return dir ?? throw new Exception("Project root not found.");
    }
}