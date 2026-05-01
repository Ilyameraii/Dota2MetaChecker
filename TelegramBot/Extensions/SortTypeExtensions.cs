using Dota2MetaChecker.Common.Models;

namespace Dota2MetaChecker.TelegramBot.Extensions;

public static class SortTypeExtensions
{
    public static string ToDisplayName(this SortType sortType, SortType currentSort, bool isDescending)
    {
        var baseText = sortType switch
        {
            SortType.MatchCount => "Матчи",
            SortType.WinRate => "Винрейт",
            SortType.Rating => "Рейтинг",
            _ => "?"
        };

        if (currentSort == sortType) return "✅ " + baseText + (isDescending ? "↓" : "↑");

        return baseText + " ↓";
    }
}