using PasswordGenerator.Api.Data;

namespace PasswordGenerator.Api.Services;

public interface IPasswordService
{
    Task<Guid> CreateAsync(CancellationToken cancellationToken);

    Task<PasswordRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
}