namespace PasswordGenerator.Api.Data;

public interface IPasswordStore
{
    Task AddAsync(PasswordRecord record, CancellationToken cancellationToken);

    Task<PasswordRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
}