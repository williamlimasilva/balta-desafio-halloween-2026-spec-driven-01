using Microsoft.EntityFrameworkCore;

namespace PasswordGenerator.Api.Data;

public sealed class EfPasswordStore(PasswordDbContext dbContext) : IPasswordStore
{
    public async Task AddAsync(PasswordRecord record, CancellationToken cancellationToken)
    {
        await dbContext.PasswordRecords.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<PasswordRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.PasswordRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(record => record.Id == id, cancellationToken);
    }
}