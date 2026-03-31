using System.Text.Json.Nodes;
using Entities.Classes;
using Entities.Enums;
using Services.Contracts.Stratz;

namespace Services.Stratz;

public class StratzHeroParser : IStratzHeroParser
{
    private static readonly Dictionary<string, HeroRank> RankMap = new()
    {
        ["UNCALIBRATED"] = HeroRank.Uncalibrated,
        ["HERALD_GUARDIAN"] = HeroRank.HeraldGuardian,
        ["CRUSADER_ARCHON"] = HeroRank.CrusaderArchon,
        ["LEGEND_ANCIENT"] = HeroRank.LegendAncient,
        ["DIVINE_IMMORTAL"] = HeroRank.DivineImmortal,
    };

    private static readonly Dictionary<string, HeroRole> RoleMap = new()
    {
        ["POSITION_1"] = HeroRole.Safelane,
        ["POSITION_2"] = HeroRole.Midlane,
        ["POSITION_3"] = HeroRole.Offlane,
        ["POSITION_4"] = HeroRole.Support,
        ["POSITION_5"] = HeroRole.HardSupport,
    };

    public List<HeroStat> ParseHeroStats(string json)
    {
        var root = JsonNode.Parse(json)
                   ?? throw new InvalidOperationException("Invalid JSON");

        var statsArray = root["data"]?["heroStats"]?["stats"]?.AsArray()
                         ?? throw new InvalidOperationException("Path data.heroStats.stats not found");

        var heroes = new List<HeroStat>();

        foreach (var item in statsArray)
        {
            if (item is null) continue;

            var heroId = item["heroId"]!.GetValue<int>();
            var rankStr = item["bracketBasicIds"]!.GetValue<string>();
            var roleStr = item["position"]!.GetValue<string>();
            var winCount = item["winCount"]!.GetValue<int>();
            var matchCount = item["matchCount"]!.GetValue<int>();

            if (!RankMap.TryGetValue(rankStr, out var rank))
            {
                throw new InvalidOperationException($"Unknown rank: {rankStr}");
            }

            if (!RoleMap.TryGetValue(roleStr, out var role))
            {
                throw new InvalidOperationException($"Unknown role: {roleStr}");
            }

            heroes.Add(new HeroStat
            {
                HeroId = heroId,
                Rank = rank,
                Role = role,
                WinCount = winCount,
                MatchCount = matchCount,
                // Name заполняется отдельно — в JSON его нет
            });
        }

        return heroes;
    }

    public Dictionary<int, string> ParseHeroesNames(string json)
    {
        var root = JsonNode.Parse(json)
                   ?? throw new InvalidOperationException("Invalid JSON");

        var heroesArray = root["data"]?["constants"]?["heroes"]?.AsArray()
                          ?? throw new InvalidOperationException("Path data.constants.heroes not found");

        return heroesArray
            .Where(item => item is not null)
            .ToDictionary(
                item => item!["id"]!.GetValue<int>(),
                item => item!["displayName"]!.GetValue<string>()
            );
    }
}