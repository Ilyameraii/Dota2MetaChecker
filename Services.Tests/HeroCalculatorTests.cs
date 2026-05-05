using Dota2MetaChecker.Common.Models;
using FluentAssertions;
using Services.Processing;
using Xunit;

namespace Services.Tests;

public class HeroCalculatorTests
{
    private readonly HeroCalculator _calculator = new();

    [Fact]
    public void Calculate_SetsWinratePickrateRating()
    {
        var hero = new Hero { MatchCount = 10, WinCount = 6 };
        var result = _calculator.Calculate(hero, totalMatchCount: 100);
        result.WinRate.Should().Be(0.6);
        result.PickRate.Should().Be(0.1);
        result.Rating.Should().Be(0.6 + 0.1); // WinrateImpactValue is 1
    }

    [Fact]
    public void Calculate_WithPrevious_SetsDeltas()
    {
        var hero = new Hero { MatchCount = 20, WinCount = 14, WinRate = 0.7, PickRate = 0.2, Rating = 0.9 };
        var previous = new Hero { WinRate = 0.6, PickRate = 0.15, Rating = 0.75 };
        var result = _calculator.Calculate(hero, totalMatchCount: 100, previous);
        result.WinRateDelta.Should().Be(0.1);
        result.PickRateDelta.Should().Be(0.05);
        result.RatingDelta.Should().Be(0.15);
    }

    [Fact]
    public void Calculate_WithLowPickRate_SetsRatingMinValue()
    {
        var hero = new Hero { MatchCount = 1, WinCount = 1 };
        var result = _calculator.Calculate(hero, totalMatchCount: 1000); // PickRate = 0.001 < 0.002
        result.Rating.Should().Be(double.MinValue);
    }

    [Fact]
    public void CalculateAll_MapsPreviousById()
    {
        var heroes = new List<Hero> { new() { Id = 1, MatchCount = 10, WinCount = 6 } };
        var previousHeroes = new List<Hero> { new() { Id = 1, WinRate = 0.5, PickRate = 0.05, Rating = 0.55 } };
        var result = _calculator.CalculateAll(heroes, totalMatchCount: 100, previousHeroes).First();
        result.WinRateDelta.Should().Be(0.6 - 0.5);
    }

    [Fact]
    public void CalculateAll_WithoutPrevious_DoesNotSetDeltas()
    {
        var heroes = new List<Hero> { new() { Id = 1, MatchCount = 10, WinCount = 6 } };
        var result = _calculator.CalculateAll(heroes, totalMatchCount: 100).First();
        result.WinRateDelta.Should().Be(0);
        result.PickRateDelta.Should().Be(0);
        result.RatingDelta.Should().Be(0);
    }
}
