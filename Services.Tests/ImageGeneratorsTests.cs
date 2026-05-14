using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using FluentAssertions;
using Services.Formatting.ImageGenerators;
using Xunit;

namespace Services.Tests;

public class ImageGeneratorsTests
{
    [Fact]
    public void HeroOptionsImageGenerator_Generate_ReturnsNonEmptyBytes()
    {
        var generator = new HeroOptionsImageGenerator();
        var heroes = new List<Hero> { new() { Id = 1, Name = "Anti-Mage", WinRate = 0.56, PickRate = 0.1 } };
        var options = new HeroProcessingOptions { SortBy = SortType.WinRate, IsDescending = true };

        var result = generator.Generate(heroes, title: "ТОП-1", options: options);
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(100); // PNG header is 8 bytes, but actual image is larger
    }

    [Fact]
    public void HeroImageGenerator_Generate_ReturnsNonEmptyBytes()
    {
        var generator = new HeroImageGenerator();
        var heroes = new List<Hero> { new() { Id = 1, Name = "Axe", WinRate = 0.51, PickRate = 0.08 } };

        var result = generator.Generate(heroes, title: "ТОП-1");
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(100);
    }
}