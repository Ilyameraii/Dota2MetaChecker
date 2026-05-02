using Dota2MetaChecker.Common.Enums;
using Entities.Models;
using Services.Contracts.Processing;
using Services.Extensions;

namespace Services.Processing;

/// <summary>
///     Сервис для фильтрации статистики персонажей по рангам и ролям
/// </summary>
public class HeroStatsFilterService : IHeroStatsFilterService
{
    /// <summary>
    ///     Применяет фильтры к статистике персонажей
    /// </summary>
    public IReadOnlyList<HeroStat> ApplyFilters(IReadOnlyList<HeroStat> heroStats, RankFlags ranks = RankFlags.None,
        RoleFlags roles = RoleFlags.None)
    {
        var result = heroStats.AsEnumerable();

        if (ranks != RankFlags.None) result = result.Where(h => h.Rank.IsIncludedIn(ranks));

        if (roles != RoleFlags.None) result = result.Where(h => h.Role.IsIncludedIn(roles));

        return result.ToList();
    }
}