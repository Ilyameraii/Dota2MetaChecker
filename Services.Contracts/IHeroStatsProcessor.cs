using Entities.Classes;
using Entities.Enums;

namespace Services.Contracts.Stratz;

public interface IHeroStatsProcessor
{
    /// <summary>
    /// Выполняет полный пайплайн: фильтрация → агрегация → сортировка
    /// </summary>
    List<Hero> GetProcessedHeroStats(IReadOnlyList<HeroStat> sourceStats,
        IReadOnlyDictionary<int, string> heroNames,
        RankFlags ranks = RankFlags.None,
        RoleFlags roles = RoleFlags.None,
        Func<IEnumerable<Hero>, IOrderedEnumerable<Hero>>? sortBy = null);
}