using Entities.Classes;
using Entities.Enums;

namespace Services.Contracts.Stratz;

public interface IHeroStatsFilterService
{
    public IEnumerable<HeroStat> ApplyFilters(IReadOnlyList<HeroStat> heroStats, RankFlags ranks = RankFlags.None,
        RoleFlags roles = RoleFlags.None);
}