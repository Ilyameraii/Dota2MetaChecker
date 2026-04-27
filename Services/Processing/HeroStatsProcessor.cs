using Entities.Models;
using Services.Contracts.Models;
using Services.Contracts.Processing;

namespace Services.Processing;

/// <summary>
/// Сервис для обработки статистики персонажей: фильтрация, агрегация, сортировка
/// </summary>
public class HeroStatsProcessor(
    IHeroStatsFilterService filterService,
    IHeroStatsAggregator aggregator)
    : IHeroStatsProcessor
{
    /// <summary>
    /// Выполняет полный пайплайн обработки статистики персонажей
    /// </summary>
    public List<Hero> GetProcessedHeroStats(
        IReadOnlyList<HeroStat> sourceStats,
        IReadOnlyDictionary<int, string> heroNames,
        HeroProcessingOptions query)
    {
        // 1. Фильтрация
        var filtered = filterService.ApplyFilters(sourceStats, query.Ranks, query.Roles);
        
        // 2. Агрегация
        var aggregated = aggregator.AggregateByHero(filtered, heroNames);
        
        // 3. Сортировка
        var sorted = query.GetSortFunction()(aggregated);
        
        return sorted.ToList();
    }
}