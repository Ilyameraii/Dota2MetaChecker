using Dota2MetaChecker.Common.Models;
using Entities.Models;
using Services.Contracts.Processing;

namespace Services.Processing;

/// <summary>
///     Сервис для агрегации статистики персонажей по идентификатору персонажа
/// </summary>
public class HeroStatsAggregator : IHeroStatsAggregator
{
    /// <summary>
    ///     Агрегирует статистику: группирует по HeroId, суммирует победы и матчи
    /// </summary>
    public IEnumerable<Hero> AggregateByHero(IReadOnlyList<HeroStat> stats, IReadOnlyDictionary<int, string> heroNames)
    {
        return stats
            .GroupBy(s => s.HeroId)
            .Select(g => new Hero
            {
                Id = g.Key,
                Name = heroNames.TryGetValue(g.Key, out var name) ? name : $"Hero #{g.Key}",
                WinCount = g.Sum(x => x.WinCount),
                MatchCount = g.Sum(x => x.MatchCount)
            });
    }
}