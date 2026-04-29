using System.Text.Json.Nodes;
using Entities.Enums;
using Entities.Models;
using Services.Contracts.Deserialization;

namespace Services.Deserialization;

/// <summary>
///     Парсер для преобразования JSON-ответа STRATZ API в модели предметной области
/// </summary>
public class StratzHeroParser : IStratzHeroParser
{
    private static readonly Dictionary<string, Rank> RankMap = new()
    {
        ["UNCALIBRATED"] = Rank.Uncalibrated,
        ["HERALD_GUARDIAN"] = Rank.HeraldGuardian,
        ["CRUSADER_ARCHON"] = Rank.CrusaderArchon,
        ["LEGEND_ANCIENT"] = Rank.LegendAncient,
        ["DIVINE_IMMORTAL"] = Rank.DivineImmortal
    };

    private static readonly Dictionary<string, Role> RoleMap = new()
    {
        ["POSITION_1"] = Role.Safelane,
        ["POSITION_2"] = Role.Midlane,
        ["POSITION_3"] = Role.Offlane,
        ["POSITION_4"] = Role.Support,
        ["POSITION_5"] = Role.HardSupport
    };

    /// <summary>
    ///     Парсит JSON со статистикой персонажей и возвращает список HeroStat
    /// </summary>
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
                throw new InvalidOperationException($"Unknown rank: {rankStr}");

            if (!RoleMap.TryGetValue(roleStr, out var role))
                throw new InvalidOperationException($"Unknown role: {roleStr}");

            heroes.Add(new HeroStat
            {
                HeroId = heroId,
                Rank = rank,
                Role = role,
                WinCount = winCount,
                MatchCount = matchCount
                // Name заполняется отдельно — в JSON его нет
            });
        }

        return heroes;
    }

    /// <summary>
    ///     Парсит JSON с именами персонажей и возвращает словарь id -> name
    /// </summary>
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