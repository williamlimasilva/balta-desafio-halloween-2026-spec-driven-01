using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using PasswordGenerator.Api.Contracts;
using PasswordGenerator.Api.Services;

namespace PasswordGenerator.Api.Endpoints;

public static class PasswordEndpoints
{
    private const string InvalidIdMessage = "The id must be a valid GUID.";

    public static void MapPasswordEndpoints(this WebApplication app)
    {
        app.MapPost("/api/passwords", CreateAsync)
            .WithName("CreatePassword");

        app.MapGet("/api/passwords/{id}", GetAsync)
            .WithName("GetPassword");
    }

    private static async Task<Created<CreatePasswordResponse>> CreateAsync(
        IPasswordService passwordService,
        CancellationToken cancellationToken)
    {
        var id = await passwordService.CreateAsync(cancellationToken);
        return TypedResults.Created($"/api/passwords/{id:D}", new CreatePasswordResponse(id));
    }

    private static async Task<Results<Ok<GetPasswordResponse>, ValidationProblem, ProblemHttpResult>> GetAsync(
        string id,
        IPasswordService passwordService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParseExact(id, "D", out var passwordId))
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]> { ["id"] = [InvalidIdMessage] },
                detail: InvalidIdMessage,
                instance: httpContext.Request.Path.ToString(),
                title: "Validation failed",
                type: "https://httpstatuses.com/400");
        }

        var password = await passwordService.GetAsync(passwordId, cancellationToken);
        if (password is null)
        {
            return TypedResults.Problem(
                detail: "A password with the specified id was not found.",
                instance: httpContext.Request.Path.ToString(),
                statusCode: StatusCodes.Status404NotFound,
                title: "Not Found",
                type: "https://httpstatuses.com/404");
        }

        return TypedResults.Ok(new GetPasswordResponse(password.Id, password.Password));
    }
}