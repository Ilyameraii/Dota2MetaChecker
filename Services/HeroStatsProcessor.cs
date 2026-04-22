using Entities.Classes;
using Entities.Enums;
using Services.Contracts.Stratz;

namespace Services;

public class HeroStatsProcessor(
    IHeroStatsFilterService filterService,
    IHeroStatsAggregator aggregator)
    : IHeroStatsProcessor
{
    public List<Hero> GetProcessedHeroStats(IReadOnlyList<HeroStat> sourceStats,
        IReadOnlyDictionary<int, string> heroNames,
        RankFlags ranks = RankFlags.None,
        RoleFlags roles = RoleFlags.None,
        Func<IEnumerable<Hero>, IOrderedEnumerable<Hero>>? sortBy = null)
    {
        // 1. Фильтрация
        var filtered = filterService.ApplyFilters(sourceStats, ranks, roles);
        
        // 2. Агрегация
        var aggregated = aggregator.AggregateByHero(filtered, heroNames);
        
        // 3. Сортировка (если передана)
        var sorted = sortBy != null ? sortBy(aggregated) : aggregated;
        
        return sorted.ToList();
    }
}