using PasswordGenerator.Api.Data;
using PasswordGenerator.Api.Services;
using Xunit;

namespace PasswordGenerator.UnitTests.Services;

public sealed class PasswordServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsGeneratedPasswordBeforeReturningId()
    {
        var id = Guid.NewGuid();
        var store = new FakePasswordStore();
        var service = CreateService(store, [id]);
        using var cancellationSource = new CancellationTokenSource();

        var createdId = await service.CreateAsync(cancellationSource.Token);

        Assert.Equal(id, createdId);
        var savedRecord = Assert.Single(store.AddedRecords);
        Assert.Equal(id, savedRecord.Id);
        Assert.Equal("literal !Password 16", savedRecord.Password);
        Assert.Equal(cancellationSource.Token, store.AddCancellationToken);
    }

    [Fact]
    public async Task CreateAsync_UsesADistinctIdForEachCreation()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var store = new FakePasswordStore();
        var service = CreateService(store, ids);

        var firstId = await service.CreateAsync(CancellationToken.None);
        var secondId = await service.CreateAsync(CancellationToken.None);

        Assert.NotEqual(firstId, secondId);
        Assert.Equal(ids, store.AddedRecords.Select(record => record.Id));
    }

    [Fact]
    public async Task CreateAsync_DoesNotReturnSuccessWhenPersistenceFails()
    {
        var store = new FakePasswordStore { AddFailure = new InvalidOperationException("Storage failed.") };
        var service = CreateService(store, [Guid.NewGuid()]);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_DelegatesLookupAndReturnsTheStoredRecord()
    {
        var id = Guid.NewGuid();
        var record = new PasswordRecord { Id = id, Password = "stored !Password 16" };
        var store = new FakePasswordStore { RecordToFind = record };
        var service = CreateService(store, []);
        using var cancellationSource = new CancellationTokenSource();

        var result = await service.GetAsync(id, cancellationSource.Token);

        Assert.Same(record, result);
        Assert.Equal(id, store.FindId);
        Assert.Equal(cancellationSource.Token, store.FindCancellationToken);
    }

    private static PasswordService CreateService(FakePasswordStore store, IEnumerable<Guid> ids)
    {
        var idEnumerator = ids.GetEnumerator();
        return new PasswordService(
            new FakePasswordGenerator("literal !Password 16"),
            store,
            () => idEnumerator.MoveNext()
                ? idEnumerator.Current
                : throw new InvalidOperationException("No test ID is available."));
    }

    private sealed class FakePasswordGenerator(string password) : IPasswordGenerator
    {
        public string Generate() => password;
    }

    private sealed class FakePasswordStore : IPasswordStore
    {
        public List<PasswordRecord> AddedRecords { get; } = [];

        public PasswordRecord? RecordToFind { get; init; }

        public Exception? AddFailure { get; init; }

        public Guid? FindId { get; private set; }

        public CancellationToken AddCancellationToken { get; private set; }

        public CancellationToken FindCancellationToken { get; private set; }

        public Task AddAsync(PasswordRecord record, CancellationToken cancellationToken)
        {
            AddedRecords.Add(record);
            AddCancellationToken = cancellationToken;
            return AddFailure is null ? Task.CompletedTask : Task.FromException(AddFailure);
        }

        public Task<PasswordRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            FindId = id;
            FindCancellationToken = cancellationToken;
            return Task.FromResult(RecordToFind);
        }
    }
}