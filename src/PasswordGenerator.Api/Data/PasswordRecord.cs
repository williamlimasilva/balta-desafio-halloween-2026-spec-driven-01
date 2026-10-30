namespace PasswordGenerator.Api.Data;

public sealed class PasswordRecord
{
    public Guid Id { get; set; }

    public required string Password { get; set; }
}