using Entities.Classes;
using Entities.Enums;
using Entities.Models;
using Services.Contracts.Processing;

namespace Services.Processing;

/// <summary>
/// Сервис для фильтрации статистики персонажей по рангам и ролям
/// </summary>
public class HeroStatsFilterService: IHeroStatsFilterService
{
    /// <summary>
    /// Применяет фильтры к статистике персонажей
    /// </summary>
    public IEnumerable<HeroStat> ApplyFilters(IReadOnlyList<HeroStat> heroStats, RankFlags ranks = RankFlags.None, RoleFlags roles = RoleFlags.None)
    {
        var result = heroStats.AsEnumerable();
        
        if (ranks != RankFlags.None)
        {
            result = result.Where(h => ranks.HasFlag((RankFlags)h.Rank ));
        }
        
        // Фильтр по ролям: аналогично
        if (roles != RoleFlags.None)
        {
            result = result.Where(h => roles.HasFlag((RoleFlags)h.Role));
        }

        return result;
    }
}