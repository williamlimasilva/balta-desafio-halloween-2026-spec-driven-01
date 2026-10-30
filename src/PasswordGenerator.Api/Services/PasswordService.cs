using PasswordGenerator.Api.Data;

namespace PasswordGenerator.Api.Services;

public sealed class PasswordService : IPasswordService
{
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IPasswordStore _passwordStore;
    private readonly Func<Guid> _newId;

    public PasswordService(IPasswordGenerator passwordGenerator, IPasswordStore passwordStore)
        : this(passwordGenerator, passwordStore, Guid.NewGuid)
    {
    }

    public PasswordService(
        IPasswordGenerator passwordGenerator,
        IPasswordStore passwordStore,
        Func<Guid> newId)
    {
        _passwordGenerator = passwordGenerator;
        _passwordStore = passwordStore;
        _newId = newId;
    }

    public async Task<Guid> CreateAsync(CancellationToken cancellationToken)
    {
        var record = new PasswordRecord
        {
            Id = _newId(),
            Password = _passwordGenerator.Generate()
        };

        await _passwordStore.AddAsync(record, cancellationToken);
        return record.Id;
    }

    public Task<PasswordRecord?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return _passwordStore.FindByIdAsync(id, cancellationToken);
    }
}