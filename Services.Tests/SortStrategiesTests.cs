using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using FluentAssertions;
using Services.Processing.StrategiesOfSorting;
using Xunit;

namespace Services.Tests;

public class SortStrategiesTests
{
    private readonly List<Hero> _heroes = new()
    {
        new Hero
        {
            WinRate = 0.5, MatchCount = 10, Rating = 0.6, WinRateDelta = 0.1, PickRateDelta = 0.02, RatingDelta = 0.05
        },
        new Hero
        {
            WinRate = 0.6, MatchCount = 5, Rating = 0.7, WinRateDelta = -0.1, PickRateDelta = 0.01, RatingDelta = -0.05
        },
        new Hero
        {
            WinRate = 0.4, MatchCount = 20, Rating = 0.5, WinRateDelta = 0.2, PickRateDelta = 0.03, RatingDelta = 0.1
        }
    };

    [Fact]
    public void WinrateSortStrategy_HasCorrectSortType()
    {
        new WinrateSortStrategy().SortType.Should().Be(SortType.WinRate);
    }

    [Fact]
    public void WinrateSortStrategy_SortsCorrectly()
    {
        var strategy = new WinrateSortStrategy();
        var result = strategy.Sort(_heroes, descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.WinRate);
    }

    [Fact]
    public void MatchCountSortStrategy_HasCorrectSortType()
    {
        new MatchCountSortStrategy().SortType.Should().Be(SortType.MatchCount);
    }

    [Fact]
    public void MatchCountSortStrategy_SortsCorrectly()
    {
        var strategy = new MatchCountSortStrategy();
        var result = strategy.Sort(_heroes, descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.MatchCount);
    }

    [Fact]
    public void RatingSortStrategy_HasCorrectSortType()
    {
        new RatingSortStrategy().SortType.Should().Be(SortType.Rating);
    }

    [Fact]
    public void RatingSortStrategy_SortsCorrectly()
    {
        var strategy = new RatingSortStrategy();
        var result = strategy.Sort(_heroes, descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.Rating);
    }

    [Fact]
    public void WinrateDeltaSortStrategy_HasCorrectSortType()
    {
        new WinrateDeltaSortStrategy().SortType.Should().Be(SortType.WinrateDelta);
    }

    [Fact]
    public void WinrateDeltaSortStrategy_SortsCorrectly()
    {
        var strategy = new WinrateDeltaSortStrategy();
        var result = strategy.Sort(_heroes, descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.WinRateDelta);
    }

    [Fact]
    public void PickrateDeltaSortStrategy_HasCorrectSortType()
    {
        new PickrateDeltaSortStrategy().SortType.Should().Be(SortType.PickrateDelta);
    }

    [Fact]
    public void PickrateDeltaSortStrategy_SortsCorrectly()
    {
        var strategy = new PickrateDeltaSortStrategy();
        var result = strategy.Sort(_heroes, descending: false).ToList();
        result.Should().BeInAscendingOrder(h => h.PickRateDelta);
    }

    [Fact]
    public void RatingDeltaSortStrategy_HasCorrectSortType()
    {
        new RatingDeltaSortStrategy().SortType.Should().Be(SortType.RatingDelta);
    }

    [Fact]
    public void RatingDeltaSortStrategy_SortsCorrectly()
    {
        var strategy = new RatingDeltaSortStrategy();
        var result = strategy.Sort(_heroes, descending: true).ToList();
        result.Should().BeInDescendingOrder(h => h.RatingDelta);
    }
}