using Dota2MetaChecker.Common.Enums;
using Entities.Models;
using FluentAssertions;
using Services.Processing;
using Xunit;

namespace Services.Tests;

public class HeroStatsFilterServiceTests
{
    private readonly HeroStatsFilterService _filterService = new();

    private readonly List<HeroStat> _stats = new()
    {
        new HeroStat { HeroId = 1, Rank = Rank.HeraldGuardian, Role = Role.Safelane },
        new HeroStat { HeroId = 2, Rank = Rank.DivineImmortal, Role = Role.Offlane },
        new HeroStat { HeroId = 3, Rank = Rank.HeraldGuardian, Role = Role.Support }
    };

    [Fact]
    public void ApplyFilters_NoFilters_ReturnsAll()
    {
        var result = _filterService.ApplyFilters(_stats);
        result.Should().HaveCount(3);
    }

    [Fact]
    public void ApplyFilters_FilterByRank_ReturnsMatching()
    {
        var result = _filterService.ApplyFilters(_stats, ranks: RankFlags.HeraldGuardian);
        result.Should().OnlyContain(h => h.Rank == Rank.HeraldGuardian);
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyFilters_FilterByRole_ReturnsMatching()
    {
        var result = _filterService.ApplyFilters(_stats, roles: RoleFlags.Safelane);
        result.Should().OnlyContain(h => h.Role == Role.Safelane);
        result.Should().HaveCount(1);
    }

    [Fact]
    public void ApplyFilters_FilterByRankAndRole_ReturnsMatching()
    {
        var result = _filterService.ApplyFilters(_stats,
            ranks: RankFlags.HeraldGuardian, roles: RoleFlags.Support);
        result.Should().HaveCount(1);
        result.Should().OnlyContain(h => h.Rank == Rank.HeraldGuardian && h.Role == Role.Support);
    }
}