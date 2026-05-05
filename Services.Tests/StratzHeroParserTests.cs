using Dota2MetaChecker.Common.Enums;
using Entities.Models;
using FluentAssertions;
using Services.Deserialization;
using Xunit;

namespace Services.Tests;

public class StratzHeroParserTests
{
    private readonly StratzHeroParser _parser = new();

    [Fact]
    public void ParseHeroStats_ReturnsCorrectList()
    {
        var json = @"{
            ""data"": {
                ""heroStats"": {
                    ""stats"": [
                        {
                            ""heroId"": 1,
                            ""bracketBasicIds"": ""HERALD_GUARDIAN"",
                            ""position"": ""POSITION_1"",
                            ""winCount"": 10,
                            ""matchCount"": 20
                        },
                        {
                            ""heroId"": 2,
                            ""bracketBasicIds"": ""DIVINE_IMMORTAL"",
                            ""position"": ""POSITION_5"",
                            ""winCount"": 5,
                            ""matchCount"": 10
                        }
                    ]
                }
            }
        }";

        var result = _parser.ParseHeroStats(json);
        result.Should().HaveCount(2);

        var first = result[0];
        first.HeroId.Should().Be(1);
        first.Rank.Should().Be(Rank.HeraldGuardian);
        first.Role.Should().Be(Role.Safelane);
        first.WinCount.Should().Be(10);
        first.MatchCount.Should().Be(20);

        var second = result[1];
        second.HeroId.Should().Be(2);
        second.Rank.Should().Be(Rank.DivineImmortal);
        second.Role.Should().Be(Role.HardSupport);
        second.WinCount.Should().Be(5);
        second.MatchCount.Should().Be(10);
    }

    [Fact]
    public void ParseHeroStats_ThrowsOnInvalidJson()
    {
        Assert.Throws<InvalidOperationException>(() => _parser.ParseHeroStats("{ invalid json }"));
    }

    [Fact]
    public void ParseHeroesNames_ReturnsCorrectDictionary()
    {
        var json = @"{
            ""data"": {
                ""constants"": {
                    ""heroes"": [
                        { ""id"": 1, ""displayName"": ""Anti-Mage"" },
                        { ""id"": 2, ""displayName"": ""Axe"" }
                    ]
                }
            }
        }";

        var result = _parser.ParseHeroesNames(json);
        result.Should().HaveCount(2);
        result[1].Should().Be("Anti-Mage");
        result[2].Should().Be("Axe");
    }

    [Fact]
    public void ParseHeroesNames_ThrowsOnInvalidJson()
    {
        Assert.Throws<InvalidOperationException>(() => _parser.ParseHeroesNames("{ invalid }"));
    }
}
