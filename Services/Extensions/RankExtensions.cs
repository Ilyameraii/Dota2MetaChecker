using Dota2MetaChecker.Common.Enums;

namespace Services.Extensions;

public static class RankExtensions
{
    public static bool IsIncludedIn(this Rank rank, RankFlags flags)
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
}