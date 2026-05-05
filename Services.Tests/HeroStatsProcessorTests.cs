using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Entities.Models;
using FluentAssertions;
using Moq;
using Services.Contracts.Processing;
using Services.Processing;
using Xunit;

namespace Services.Tests;

public class HeroStatsProcessorTests
{
    private readonly Mock<IHeroStatsFilterService> _filterMock = new();
    private readonly Mock<IHeroStatsAggregator> _aggregatorMock = new();
    private readonly Mock<IHeroCalculator> _calculatorMock = new();
    private readonly List<IHeroSortStategy> _sortStrategies = new();
    private readonly HeroStatsProcessor _processor;

    public HeroStatsProcessorTests()
    {
        _processor = new HeroStatsProcessor(
            _filterMock.Object,
            _aggregatorMock.Object,
            _calculatorMock.Object,
            _sortStrategies);
    }

    [Fact]
    public void GetProcessedHeroStats_CallsFilterAndAggregate()
    {
        // Setup
        var sourceStats = new List<HeroStat> { new() };
        var oldSourceStats = new List<HeroStat> { new() };
        var heroNames = new Dictionary<int, string> { { 1, "Test" } };
        var query = new HeroProcessingOptions { Ranks = RankFlags.None, Roles = RoleFlags.None, SortBy = SortType.Rating, IsDescending = true };

        var filtered = new List<HeroStat> { new() };
        var oldFiltered = new List<HeroStat> { new() };
        var aggregated = new List<Hero> { new() { Id = 1 } };
        var oldAggregated = new List<Hero> { new() { Id = 1 } };
        var calculated = new List<Hero> { new() { Id = 1 } };

        _filterMock.Setup(f => f.ApplyFilters(sourceStats, query.Ranks, query.Roles)).Returns(filtered);
        _filterMock.Setup(f => f.ApplyFilters(oldSourceStats, query.Ranks, query.Roles)).Returns(oldFiltered);
        _aggregatorMock.Setup(a => a.AggregateByHero(filtered, heroNames)).Returns(aggregated);
        _aggregatorMock.Setup(a => a.AggregateByHero(oldFiltered, heroNames)).Returns(oldAggregated);
        _calculatorMock.Setup(c => c.CalculateAll(oldAggregated, It.IsAny<int>(), null)).Returns(oldAggregated);
        _calculatorMock.Setup(c => c.CalculateAll(aggregated, It.IsAny<int>(), oldAggregated)).Returns(calculated);

        // Execute
        var result = _processor.GetProcessedHeroStats(sourceStats, oldSourceStats, heroNames, query);

        // Assert
        result.Should().BeEquivalentTo(calculated);
        _filterMock.VerifyAll();
        _aggregatorMock.VerifyAll();
        _calculatorMock.VerifyAll();
    }

    [Fact]
    public void GetProcessedHeroStats_UsesDefaultSortStrategy_WhenNotFound()
    {
        // Setup sort strategy
        var mockStrategy = new Mock<IHeroSortStategy>();
        mockStrategy.Setup(s => s.SortType).Returns(SortType.Rating);
        mockStrategy.Setup(s => s.Sort(It.IsAny<IEnumerable<Hero>>(), It.IsAny<bool>())).Returns(new List<Hero>());
        _sortStrategies.Add(mockStrategy.Object);

        var sourceStats = new List<HeroStat>();
        var oldSourceStats = new List<HeroStat>();
        var heroNames = new Dictionary<int, string>();
        var query = new HeroProcessingOptions { SortBy = SortType.WinRate }; // No matching strategy

        _filterMock.Setup(f => f.ApplyFilters(It.IsAny<IReadOnlyList<HeroStat>>(), It.IsAny<RankFlags>(), It.IsAny<RoleFlags>()))
            .Returns(new List<HeroStat>());
        _aggregatorMock.Setup(a => a.AggregateByHero(It.IsAny<IReadOnlyList<HeroStat>>(), It.IsAny<IReadOnlyDictionary<int, string>>()))
            .Returns(new List<Hero>());
        _calculatorMock.Setup(c => c.CalculateAll(It.IsAny<IEnumerable<Hero>>(), It.IsAny<int>(), It.IsAny<IEnumerable<Hero>>()))
            .Returns(new List<Hero>());

        _processor.GetProcessedHeroStats(sourceStats, oldSourceStats, heroNames, query);

        mockStrategy.Verify(s => s.Sort(It.IsAny<IEnumerable<Hero>>(), It.IsAny<bool>()), Times.Once);
    }
}
