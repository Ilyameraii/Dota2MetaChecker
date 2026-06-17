using Dota2MetaChecker.Common.Enums;

namespace Dota2MetaChecker.TelegramBot.Extensions;

public static class RankFlagsExtensions
{
    public static string ToDisplayName(this RankFlags rankFlag, RankFlags selectedFlags)
    {
        var baseText = rankFlag switch
        {
            RankFlags.Uncalibrated => "Uncalibrated",
            RankFlags.HeraldGuardian => "Herald-Guardian",
            RankFlags.CrusaderArchon => "Crusader-Archon",
            RankFlags.LegendAncient => "Legend-Ancient",
            RankFlags.DivineImmortal => "Divine-Immortal",
            _ => "?"
        };


        return selectedFlags.HasFlag(rankFlag) ? "✅ " + baseText : baseText;
    }
}