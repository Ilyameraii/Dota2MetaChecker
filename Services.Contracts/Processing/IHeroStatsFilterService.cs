using Entities.Classes;
using Entities.Enums;
using Entities.Models;

namespace Services.Contracts.Processing;

public interface IHeroStatsFilterService
{
    public IEnumerable<HeroStat> ApplyFilters(IReadOnlyList<HeroStat> heroStats, RankFlags ranks = RankFlags.None,
        RoleFlags roles = RoleFlags.None);
}