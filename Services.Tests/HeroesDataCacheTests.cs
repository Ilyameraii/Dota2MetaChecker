using Entities.Models;
using FluentAssertions;
using Services.Data_sync;
using Xunit;

namespace Services.Tests;

public class HeroesDataCacheTests
{
    [Fact]
    public void IsLoaded_ReturnsFalse_WhenDataNull()
    {
        var cache = new HeroesDataCache();
        cache.IsLoaded.Should().BeFalse();
    }

    [Fact]
    public void IsLoaded_ReturnsTrue_WhenDataLoaded()
    {
        var cache = new HeroesDataCache
        {
            NewHeroesStats = new List<HeroStat>(),
            HeroesNames = new Dictionary<int, string>()
        };
        cache.IsLoaded.Should().BeTrue();
    }

    [Fact]
    public void HeroCount_ReturnsZero_WhenNamesNull()
    {
        var cache = new HeroesDataCache();
        cache.HeroCount.Should().Be(0);
    }

    [Fact]
    public void HeroCount_ReturnsCorrectCount()
    {
        var cache = new HeroesDataCache
        {
            HeroesNames = new Dictionary<int, string> { { 1, "A" }, { 2, "B" } }
        };
        cache.HeroCount.Should().Be(2);
    }
}
