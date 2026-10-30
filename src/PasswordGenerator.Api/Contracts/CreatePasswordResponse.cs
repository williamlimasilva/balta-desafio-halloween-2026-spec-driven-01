namespace PasswordGenerator.Api.Contracts;

/// <summary>Contains the identifier assigned to a newly created password.</summary>
public sealed record CreatePasswordResponse(Guid Id);