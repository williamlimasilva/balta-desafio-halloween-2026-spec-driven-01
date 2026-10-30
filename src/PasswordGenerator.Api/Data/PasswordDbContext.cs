using Microsoft.EntityFrameworkCore;

namespace PasswordGenerator.Api.Data;

public sealed class PasswordDbContext(DbContextOptions<PasswordDbContext> options) : DbContext(options)
{
    public DbSet<PasswordRecord> PasswordRecords => Set<PasswordRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordRecord>(entity =>
        {
            entity.ToTable("PasswordRecords");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Password).IsRequired();
        });
    }
}