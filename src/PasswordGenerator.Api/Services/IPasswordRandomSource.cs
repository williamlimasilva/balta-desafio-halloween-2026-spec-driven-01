namespace PasswordGenerator.Api.Services;

public interface IPasswordRandomSource
{
    int GetInt32(int exclusiveUpperBound);
}