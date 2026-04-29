using Services.Contracts.Formatting;
using Services.Contracts.Models;

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

    public string Format(Hero hero, int totalMatchCount, Hero oldHero, int oldTotalMatchCount)
    {
        var winRate = hero.WinRate * 100;
        var pickRate = 100.0 * hero.MatchCount / totalMatchCount;
        
        var oldWinRate = oldHero.WinRate * 100;
        var oldPickRate = 100.0 * oldHero.MatchCount / oldTotalMatchCount;

        var wr = winRate - oldWinRate;
        var pr = pickRate - oldPickRate;

        string winRateDelta = FormatDelta(wr);
        string pickRateDelta = FormatDelta(pr);

        return $"<b>{hero.Name}</b> - <b>{winRate:F2}%</b> ({winRateDelta}) побед, <b>{pickRate:F2}%</b> ({pickRateDelta}) выборов";
    }

    private static string FormatDelta(double delta)
    {
        var sign = delta > 0 ? "+" : "-";
        return $" {sign}{Math.Abs(delta):F2}%";
    }
}