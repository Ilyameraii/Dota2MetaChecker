using Dota2MetaChecker.Common.Enums;

namespace Services.Extensions;

public static class FlagExtensions
{
    public static RankFlags ToFlag(this Rank rank)
    {
        return rank switch
        {
            Rank.Uncalibrated => RankFlags.Uncalibrated,
            Rank.HeraldGuardian => RankFlags.HeraldGuardian,
            Rank.CrusaderArchon => RankFlags.CrusaderArchon,
            Rank.LegendAncient => RankFlags.LegendAncient,
            Rank.DivineImmortal => RankFlags.DivineImmortal,
            _ => RankFlags.None
        };
    }

    public static RoleFlags ToFlag(this Role role)
    {
        return role switch
        {
            Role.Safelane => RoleFlags.Safelane,
            Role.Midlane => RoleFlags.Midlane,
            Role.Offlane => RoleFlags.Offlane,
            Role.Support => RoleFlags.Support,
            Role.HardSupport => RoleFlags.HardSupport,
            _ => RoleFlags.None
        };
    }
}