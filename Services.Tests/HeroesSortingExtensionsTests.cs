using Dota2MetaChecker.Common.Models;
using FluentAssertions;
using Services.Processing.StrategiesOfSorting.Extensions;
using Xunit;

namespace Services.Tests;

public class HeroesSortingExtensionsTests
{
    private readonly List<Hero> _heroes = new()
    {
        new Hero { WinRate = 0.5, MatchCount = 10, Rating = 0.6, WinRateDelta = 0.1, PickRateDelta = 0.02, RatingDelta = 0.05 },
        new Hero { WinRate = 0.6, MatchCount = 5, Rating = 0.7, WinRateDelta = -0.1, PickRateDelta = 0.01, RatingDelta = -0.05 },
        new Hero { WinRate = 0.4, MatchCount = 20, Rating = 0.5, WinRateDelta = 0.2, PickRateDelta = 0.03, RatingDelta = 0.1 }
    };

    [Fact]
    public void OrderByWinRate_Ascending_SortsCorrectly()
    {
        var result = _heroes.OrderByWinRate(descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.WinRate);
    }

    [Fact]
    public void OrderByWinRate_Descending_SortsCorrectly()
    {
        var result = _heroes.OrderByWinRate(descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.WinRate);
    }

    [Fact]
    public void OrderByMatchCount_Ascending_SortsCorrectly()
    {
        var result = _heroes.OrderByMatchCount(descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.MatchCount);
    }

    [Fact]
    public void OrderByMatchCount_Descending_SortsCorrectly()
    {
        var result = _heroes.OrderByMatchCount(descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.MatchCount);
    }

    [Fact]
    public void OrderByRating_Ascending_SortsCorrectly()
    {
        var result = _heroes.OrderByRating(descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.Rating);
    }

    [Fact]
    public void OrderByRating_Descending_SortsCorrectly()
    {
        var result = _heroes.OrderByRating(descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.Rating);
    }

    [Fact]
    public void OrderByWinrateDelta_Ascending_SortsCorrectly()
    {
        var result = _heroes.OrderByWinrateDelta(descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.WinRateDelta);
    }

    [Fact]
    public void OrderByWinrateDelta_Descending_SortsCorrectly()
    {
        var result = _heroes.OrderByWinrateDelta(descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.WinRateDelta);
    }

    [Fact]
    public void OrderByPickrateDelta_Ascending_SortsCorrectly()
    {
        var result = _heroes.OrderByPickrateDelta(descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.PickRateDelta);
    }

    [Fact]
    public void OrderByPickrateDelta_Descending_SortsCorrectly()
    {
        var result = _heroes.OrderByPickrateDelta(descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.PickRateDelta);
    }

    [Fact]
    public void OrderByRatingDelta_Ascending_SortsCorrectly()
    {
        var result = _heroes.OrderByRatingDelta(descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.RatingDelta);
    }

    [Fact]
    public void OrderByRatingDelta_Descending_SortsCorrectly()
    {
        var result = _heroes.OrderByRatingDelta(descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.RatingDelta);
    }
}
