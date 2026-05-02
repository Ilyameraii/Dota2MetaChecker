using Dota2MetaChecker.Common.Enums;
using Entities.Models;

namespace Services.Contracts.Processing;

/// <summary>
///     Сервис для фильтрации статистики персонажей по рангам и ролям
/// </summary>
public interface IHeroStatsFilterService
{
    /// <summary>
    ///     Применяет фильтры к списку статистики персонажей
    /// </summary>
    public IReadOnlyList<HeroStat> ApplyFilters(IReadOnlyList<HeroStat> heroStats, RankFlags ranks = RankFlags.None,
        RoleFlags roles = RoleFlags.None);
}