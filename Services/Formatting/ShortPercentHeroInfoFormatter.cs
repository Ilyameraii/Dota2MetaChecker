using Dota2MetaChecker.Common.Models;
using Services.Contracts.Formatting;
using Services.Formatting.Extensions;

namespace Services.Formatting;

public class ShortPercentHeroInfoFormatter:IHeroInfoFormatter
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

        var winRateDelta = hero.WinRateDelta.FormatDelta();
        var pickRateDelta = hero.PickRateDelta.FormatDelta();

        return
            $"<b>{hero.Name}</b> - <b>{winRate:F2}</b> (<b>{winRateDelta}</b>) % побед, <b>{pickRate:F2}</b> (<b>{pickRateDelta}</b>) % выборов";
    }
}