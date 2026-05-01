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
    IHeroStatDeltaCalculator deltaCalculator)
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
        var withDeltas = deltaCalculator.CalculateDeltas(
            aggregated,
            oldAggregated,
            filtered.Sum(s => s.MatchCount),
            oldFiltered.Sum(s => s.MatchCount));

        // 4. Сортировка
        return query.GetSortFunction()(withDeltas).ToList();

    }
}