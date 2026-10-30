namespace PasswordGenerator.Api.Services;

public sealed class PasswordGenerator(IPasswordRandomSource randomSource) : IPasswordGenerator
{
    private const int PasswordLength = 16;
    private const string PunctuationCharacters = "!@#$%^&*()-_=+[]{};:,.?/";
    private const string AllowedCharacters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{};:,.?/";

    public string Generate()
    {
        var characters = new char[PasswordLength];
        characters[0] = PunctuationCharacters[randomSource.GetInt32(PunctuationCharacters.Length)];

        for (var index = 1; index < characters.Length; index++)
        {
            characters[index] = AllowedCharacters[randomSource.GetInt32(AllowedCharacters.Length)];
        }

        for (var index = characters.Length - 1; index > 0; index--)
        {
            var swapIndex = randomSource.GetInt32(index + 1);
            (characters[index], characters[swapIndex]) = (characters[swapIndex], characters[index]);
        }

        return new string(characters);
    }
}