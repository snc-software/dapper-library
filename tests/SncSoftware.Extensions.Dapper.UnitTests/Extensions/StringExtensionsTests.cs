using FluentAssertions;
using SncSoftware.Extensions.Dapper.Extensions;

namespace SncSoftware.Extensions.Dapper.UnitTests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void Pluralise_Will_Pluralise_The_Value()
    {
        var value = "test";

        var pluralised = value.Pluralise();

        pluralised.Should().Be("tests");
    }
}