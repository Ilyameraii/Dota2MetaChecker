using Dota2MetaChecker.Common.Enums;

namespace Dota2MetaChecker.TelegramBot.Extensions;

public static class RankFlagsExtensions
{
    public static string ToDisplayName(this RankFlags rankFlag, RankFlags selectedFlags)
    {
        var baseText = rankFlag switch
        {
            RankFlags.Uncalibrated => "Неоткалиброванный",
            RankFlags.HeraldGuardian => "Рекрут-Страж",
            RankFlags.CrusaderArchon => "Рыцарь-Герой",
            RankFlags.LegendAncient => "Легенда-Властелин",
            RankFlags.DivineImmortal => "Божество-Титан",
            _ => "?"
        };


        return selectedFlags.HasFlag(rankFlag) ? "✅ " + baseText : baseText;
    }
    
    
}