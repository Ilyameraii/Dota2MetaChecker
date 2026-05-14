using Dota2MetaChecker.Common.Enums;

namespace Dota2MetaChecker.TelegramBot.Extensions;

public static class RankRolePairs
{
    public static readonly ( RankFlags RankFlag, RoleFlags RoleFlag)[] Default =
    [
        (RankFlags.HeraldGuardian, RoleFlags.Safelane),
        (RankFlags.CrusaderArchon, RoleFlags.Midlane),
        (RankFlags.LegendAncient, RoleFlags.Offlane),
        (RankFlags.DivineImmortal, RoleFlags.Support),
        (RankFlags.Uncalibrated, RoleFlags.HardSupport),
    ];
}