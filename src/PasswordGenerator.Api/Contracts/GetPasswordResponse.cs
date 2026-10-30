namespace PasswordGenerator.Api.Contracts;

/// <summary>Contains a password and its identifier.</summary>
public sealed record GetPasswordResponse(Guid Id, string Password);