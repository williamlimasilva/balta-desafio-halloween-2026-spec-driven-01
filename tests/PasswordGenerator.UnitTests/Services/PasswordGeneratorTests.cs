using PasswordGenerator.Api.Services;
using Xunit;
using PasswordGeneratorImpl = PasswordGenerator.Api.Services.PasswordGenerator;

namespace PasswordGenerator.UnitTests.Services;

public sealed class PasswordGeneratorTests
{
    [Fact]
    public void Generate_ReturnsExactlySixteenCharacters()
    {
        var generator = CreateGenerator();

        var password = generator.Generate();

        Assert.Equal(16, password.Length);
    }

    [Fact]
    public void Generate_ContainsPunctuation()
    {
        var generator = CreateGenerator();

        var password = generator.Generate();

        Assert.Contains(password, char.IsPunctuation);
    }

    [Fact]
    public void Generate_ContainsNoWhitespace()
    {
        var generator = CreateGenerator();

        var password = generator.Generate();

        Assert.DoesNotContain(password, char.IsWhiteSpace);
    }

    [Fact]
    public void Generate_UsesInjectedRandomSourceForSelectionAndShuffling()
    {
        var randomSource = new CountingRandomSource();
        var generator = new PasswordGeneratorImpl(randomSource);

        generator.Generate();

        Assert.Equal(31, randomSource.CallCount);
    }

    [Fact]
    public void Generate_UsesOnlyTheAllowedAlphabet()
    {
        var generator = CreateGenerator();

        var password = generator.Generate();

        Assert.All(password, character => Assert.True(char.IsAsciiLetterOrDigit(character) || char.IsPunctuation(character)));
    }

    private static PasswordGeneratorImpl CreateGenerator()
    {
        return new PasswordGeneratorImpl(new CountingRandomSource());
    }

    private sealed class CountingRandomSource : IPasswordRandomSource
    {
        public int CallCount { get; private set; }

        public int GetInt32(int exclusiveUpperBound)
        {
            CallCount++;
            return 0;
        }
    }
}