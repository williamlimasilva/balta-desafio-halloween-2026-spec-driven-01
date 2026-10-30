using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PasswordGenerator.Api.Data;

namespace PasswordGenerator.IntegrationTests;

public sealed class PasswordApiFactory : WebApplicationFactory<Program>
{
    public static async Task<PasswordApiFactory> CreateInitializedAsync()
    {
        var factory = new PasswordApiFactory();

        try
        {
            await factory.InitializeDatabaseAsync();
            return factory;
        }
        catch
        {
            factory.Dispose();
            throw;
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PasswordDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<PasswordDbContext>>();
            services.RemoveAll<PasswordDbContext>();
            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("Data Source=:memory:");
                connection.Open();
                return connection;
            });
            services.AddDbContext<PasswordDbContext>((serviceProvider, options) =>
                options.UseSqlite(serviceProvider.GetRequiredService<DbConnection>()));
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PasswordDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task SeedRecordAsync(Guid id, string password)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PasswordDbContext>();
        dbContext.PasswordRecords.Add(new PasswordRecord
        {
            Id = id,
            Password = password
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task<PasswordRecord?> FindRecordAsync(Guid id)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PasswordDbContext>();
        return await dbContext.PasswordRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(record => record.Id == id);
    }
}