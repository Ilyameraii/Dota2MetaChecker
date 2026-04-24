using Entities.Classes;
using Entities.Models;

namespace Services.Contracts.Processing;

public interface IHeroStatsAggregator
{
    /// <summary>
    /// Агрегирует статистику по героям: группирует по HeroId, суммирует победы и матчи
    /// </summary>
    IEnumerable<Hero> AggregateByHero(IEnumerable<HeroStat> stats, IReadOnlyDictionary<int, string> heroNames);
}