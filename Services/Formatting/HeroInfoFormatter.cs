using System.Globalization;
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
        var winRate = hero.WinRate;
        var pickRate = hero.PickRate;

        return
            $"<b>{hero.Name}</b> - <b>{(winRate * 100).ToString("F2", CultureInfo.InvariantCulture)}%</b> " +
            $" побед, <b>{(pickRate * 100).ToString("F2", CultureInfo.InvariantCulture)}%</b>  выборов";
    }

    public string FormatWithDelta(Hero hero)
    {
        var winRate = hero.WinRate;
        var pickRate = hero.PickRate;

        var winRateDelta = hero.WinRateDelta;
        var pickRateDelta = hero.PickRateDelta;

        return
            $"<b>{hero.Name}</b> - <b>{(winRate * 100).ToString("F2", CultureInfo.InvariantCulture)}%</b> " +
            $"(<b>{winRateDelta.FormatDelta()} %</b>) побед, " +
            $"<b>{(pickRate * 100).ToString("F2", CultureInfo.InvariantCulture)}%</b> " +
            $"(<b>{pickRateDelta.FormatDelta()} %</b>) выборов";
    }
}