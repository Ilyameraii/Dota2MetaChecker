using Dota2MetaChecker.Common.Models;
using Entities.Models;

namespace Services.Contracts.Processing;

/// <summary>
///     Сервис для агрегации статистики персонажей по идентификатору персонажа
/// </summary>
public interface IHeroStatsAggregator
{
    /// <summary>
    ///     Агрегирует статистику по героям
    /// </summary>
    IEnumerable<Hero> AggregateByHero(IEnumerable<HeroStat> stats, IReadOnlyDictionary<int, string> heroNames);
}