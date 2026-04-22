using Entities.Classes;
using Entities.Enums;
using Services.Contracts.Stratz;

namespace Services.Stratz;

public class HeroStatsFilterService: IHeroStatsFilterService
{
    public IEnumerable<HeroStat> ApplyFilters(IReadOnlyList<HeroStat> heroStats, RankFlags ranks = RankFlags.None, RoleFlags roles = RoleFlags.None)
    {
        var result = heroStats.AsEnumerable();
        
        if (ranks != RankFlags.None)
        {
            result = result.Where(h => ranks.HasFlag((RankFlags)h.Rank));
        }
        
        // Фильтр по ролям: аналогично
        if (roles != RoleFlags.None)
        {
            result = result.Where(h => roles.HasFlag((RoleFlags)h.Role));
        }

        return result;
    }
}