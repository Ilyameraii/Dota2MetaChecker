using Dota2MetaChecker.Common.Enums;

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
            SortType.WinrateDelta => "Рост винрейта",
            SortType.PickrateDelta => "Рост пикрейта",
            SortType.RatingDelta=>"Рост рейтинга",
            _ => "?"
        };

        if (currentSort == sortType) return "✅ " + baseText + " " + (isDescending ? "↓" : "↑");

        return baseText + " ↓";
    }
}