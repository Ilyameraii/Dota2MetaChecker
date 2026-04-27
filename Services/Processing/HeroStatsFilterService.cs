using Entities.Enums;
using Services.Contracts.Enums;
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
            result = result.Where(h => HasRankFlag(h.Rank, ranks));
        }
        
        if (roles != RoleFlags.None)
        {
            result = result.Where(h => HasRoleFlag(h.Role, roles));
        }

        return result;
    }

    private static bool HasRankFlag(Rank rank, RankFlags flags)
    {
        var rankFlag = rank switch
        {
            Rank.Uncalibrated => RankFlags.Uncalibrated,
            Rank.HeraldGuardian => RankFlags.HeraldGuardian,
            Rank.CrusaderArchon => RankFlags.CrusaderArchon,
            Rank.LegendAncient => RankFlags.LegendAncient,
            Rank.DivineImmortal => RankFlags.DivineImmortal,
            _ => RankFlags.None
        };
        return flags.HasFlag(rankFlag);
    }

    private static bool HasRoleFlag(Role role, RoleFlags flags)
    {
        var roleFlag = role switch
        {
            Role.Safelane => RoleFlags.Safelane,
            Role.Midlane => RoleFlags.Midlane,
            Role.Offlane => RoleFlags.Offlane,
            Role.Support => RoleFlags.Support,
            Role.HardSupport => RoleFlags.HardSupport,
            _ => RoleFlags.None
        };
        return flags.HasFlag(roleFlag);
    }
}