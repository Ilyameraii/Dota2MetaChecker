using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Entities.Models;
using Services.Contracts.Processing;

namespace Services.Processing;

/// <summary>
///     Сервис для обработки статистики персонажей: фильтрация, агрегация, сортировка
/// </summary>
public class HeroStatsProcessor(
    IHeroStatsFilterService filterService,
    IHeroStatsAggregator aggregator,
    IHeroCalculator calculator,
    IEnumerable<IHeroSortStategy> sortStrategies)
    : IHeroStatsProcessor
{
    /// <summary>
    ///     Выполняет полный пайплайн обработки статистики персонажей
    /// </summary>
    public List<Hero> GetProcessedHeroStats(
        IReadOnlyList<HeroStat> sourceStats,
        IReadOnlyList<HeroStat> oldSourceStats,
        IReadOnlyDictionary<int, string> heroNames,
        HeroProcessingOptions query)
    {
        // 1. Фильтрация
        var filtered = filterService.ApplyFilters(sourceStats, query.Ranks, query.Roles);
        var oldFiltered = filterService.ApplyFilters(oldSourceStats, query.Ranks, query.Roles);

        // 2. Агрегация
        var aggregated = aggregator.AggregateByHero(filtered, heroNames);
        var oldAggregated = aggregator.AggregateByHero(oldFiltered, heroNames);

        // 3. Дельты
        var oldCalculated = calculator.CalculateAll(oldAggregated,
            oldFiltered.Sum(o => o.MatchCount));

        var calculated = calculator.CalculateAll(aggregated,
            filtered.Sum(o => o.MatchCount),
            oldCalculated);

        // 4. Сортировка
        var strategy = sortStrategies.FirstOrDefault(s => s.SortType == query.SortBy)
                       ?? sortStrategies.First(s => s.SortType == SortType.Rating);

        var sorted = strategy.Sort(calculated, query.IsDescending);
        return sorted.ToList();
    }
}