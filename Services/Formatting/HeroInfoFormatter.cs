using Dota2MetaChecker.Common.Models;
using Services.Contracts.Formatting;
using Services.Formatting.Extensions;

namespace Services.Formatting;

/// <summary>
///     Форматировщик информации о персонажах для отображения
/// </summary>
public class HeroInfoFormatter : IHeroInfoFormatter
{
    /// <summary>
    ///     Форматирует информацию о персонаже (имя, винрейт и пикрейт)
    /// </summary>
    public string Format(Hero hero)
    {
        var winRate = hero.WinRate * 100;
        var pickRate = hero.PickRate * 100;
        
        return
            $"<b>{hero.Name}</b> - <b>{winRate:F2}%</b> win rate, <b>{pickRate:F2}%</b> pick rate";
    }

    public string FormatWithDelta(Hero hero)
    {
        var winRate = hero.WinRate * 100;
        var pickRate = hero.PickRate * 100;

        var winRateDelta = hero.WinRateDelta * 100;
        var pickRateDelta = hero.PickRateDelta * 100;

        return
            $"<b>{hero.Name}</b> - <b>{winRate:F2}%</b> (<b>{winRateDelta:F2} %</b>) побед, <b>{pickRate:F2}%</b> (<b>{pickRateDelta:F2} %</b>) выборов";
    }
}