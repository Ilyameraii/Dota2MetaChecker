using Dota2MetaChecker.Common.Models;
using FluentAssertions;
using Services.Processing.Extensions;
using Services.Processing.Extensions.Constants;
using Xunit;

namespace Services.Tests;

public class HeroCalculatorExtensionsTests
{
    [Fact]
    public void WithWinrate_CalculatesCorrectly_WhenMatchCountPositive()
    {
        var hero = new Hero { MatchCount = 10, WinCount = 6 };
        var result = hero.WithWinrate();
        result.WinRate.Should().Be(0.6);
    }

    [Fact]
    public void WithWinrate_ReturnsZero_WhenMatchCountZero()
    {
        var hero = new Hero { MatchCount = 0, WinCount = 0 };
        var result = hero.WithWinrate();
        result.WinRate.Should().Be(0);
    }

    [Fact]
    public void WithPickRate_CalculatesCorrectly_WhenTotalPositive()
    {
        var hero = new Hero { MatchCount = 5 };
        var result = hero.WithPickRate(20);
        result.PickRate.Should().Be(0.25);
    }

    [Fact]
    public void WithPickRate_ReturnsZero_WhenTotalZero()
    {
        var hero = new Hero { MatchCount = 5 };
        var result = hero.WithPickRate(0);
        result.PickRate.Should().Be(0);
    }

    [Fact]
    public void WithRating_SetsMinValue_WhenPickRateBelowMin()
    {
        var hero = new Hero { PickRate = HeroRatingConstants.MinPickrateForRating - 0.001 };
        var result = hero.WithRating();
        result.Rating.Should().Be(double.MinValue);
    }

    [Fact]
    public void WithRating_CalculatesCorrectly_WhenPickRateAboveMin()
    {
        var hero = new Hero { WinRate = 0.6, PickRate = 0.01 };
        var result = hero.WithRating();
        result.Rating.Should().Be(HeroRatingConstants.WinrateImpactValue * 0.6 + 0.01);
    }

    [Fact]
    public void WithDeltas_CalculatesCorrectly()
    {
        var current = new Hero { WinRate = 0.6, PickRate = 0.02, Rating = 0.61 };
        var previous = new Hero { WinRate = 0.55, PickRate = 0.015, Rating = 0.565 };
        var result = current.WithDeltas(previous);
        result.WinRateDelta.Should().BeApproximately(0.05, 1e-10);
        result.PickRateDelta.Should().BeApproximately(0.005, 1e-10);
        result.RatingDelta.Should().BeApproximately(0.045, 1e-10);
    }
}