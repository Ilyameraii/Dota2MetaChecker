using Dota2MetaChecker.Common.Models;
using Services.Contracts.Formatting;

namespace Services.Formatting;

/// <summary>
///     Форматировщик информации о персонажах для отображения
/// </summary>
public class HeroInfoFormatter : IHeroInfoFormatter
{
    /// <summary>
    ///     Форматирует информацию о персонаже (имя, винрейт и пикрейт)
    /// </summary>
    public string Format(Hero hero, int totalMatchCount)
    {
        return
            $"{hero.Name} - {hero.WinRate * 100:F2}% win rate, {100.0 * hero.MatchCount / totalMatchCount:F2}% pick rate";
    }

    public string FormatWithDelta(Hero hero, int totalMatchCount)
    {
        var winRate = hero.WinRate * 100;
        var pickRate = 100.0 * hero.MatchCount / totalMatchCount;

        var winRateDelta = FormatDelta(hero.WinRateDelta);
        var pickRateDelta = FormatDelta(hero.PickRateDelta);

        return
            $"<b>{hero.Name}</b> - <b>{winRate:F2}%</b> ({winRateDelta}) побед, <b>{pickRate:F2}%</b> ({pickRateDelta}) выборов";
    }

    private static string FormatDelta(double delta)
    {
        delta = Math.Round(delta * 100, 2);
        var sign = delta > 0 ? "+" : "-";
        return $" {sign}{Math.Abs(delta):F2}%";
    }
}