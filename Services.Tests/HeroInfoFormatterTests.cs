using Dota2MetaChecker.Common.Models;
using FluentAssertions;
using Services.Formatting;
using Xunit;

namespace Services.Tests;

public class HeroInfoFormatterTests
{
    private readonly HeroInfoFormatter _formatter = new();

    [Fact]
    public void Format_ReturnsCorrectString()
    {
        var hero = new Hero { Name = "Anti-Mage", WinRate = 0.5678, PickRate = 0.1234 };
        var result = _formatter.Format(hero);
        result.Should().Contain("<b>Anti-Mage</b>");
        result.Should().Contain("56.78%");
        result.Should().Contain("12.34%");
    }

    [Fact]
    public void FormatWithDelta_ReturnsCorrectString()
    {
        var hero = new Hero 
        { 
            Name = "Axe", 
            WinRate = 0.5123, 
            PickRate = 0.0987,
            WinRateDelta = 0.0123,
            PickRateDelta = -0.0045
        };
        var result = _formatter.FormatWithDelta(hero);
        result.Should().Contain("<b>Axe</b>");
        result.Should().Contain("51.23%");
        result.Should().Contain("+1.23 %");
        result.Should().Contain("-0.45 %");
    }
}
