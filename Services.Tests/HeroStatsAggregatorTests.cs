using Dota2MetaChecker.Common.Models;
using Entities.Models;
using FluentAssertions;
using Services.Processing;
using Xunit;

namespace Services.Tests;

public class HeroStatsAggregatorTests
{
    private readonly HeroStatsAggregator _aggregator = new();
    private readonly Dictionary<int, string> _heroNames = new() { { 1, "Anti-Mage" }, { 2, "Axe" } };

    [Fact]
    public void AggregateByHero_GroupsAndSumsCorrectly()
    {
        var stats = new List<HeroStat>
        {
            new() { HeroId = 1, WinCount = 5, MatchCount = 10 },
            new() { HeroId = 1, WinCount = 3, MatchCount = 6 },
            new() { HeroId = 2, WinCount = 4, MatchCount = 8 }
        };

        var result = _aggregator.AggregateByHero(stats, _heroNames).ToList();
        result.Should().HaveCount(2);

        var antiMage = result.First(h => h.Id == 1);
        antiMage.WinCount.Should().Be(8);
        antiMage.MatchCount.Should().Be(16);
        antiMage.Name.Should().Be("Anti-Mage");

        var axe = result.First(h => h.Id == 2);
        axe.WinCount.Should().Be(4);
        axe.MatchCount.Should().Be(8);
        axe.Name.Should().Be("Axe");
    }

    [Fact]
    public void AggregateByHero_UnknownHeroId_UsesDefaultName()
    {
        var stats = new List<HeroStat> { new() { HeroId = 99, WinCount = 1, MatchCount = 2 } };
        var result = _aggregator.AggregateByHero(stats, _heroNames).First();
        result.Name.Should().Be("Hero #99");
    }
}
