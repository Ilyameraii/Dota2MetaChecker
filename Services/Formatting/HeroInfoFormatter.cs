using Services.Contracts.Formatting;
using Services.Contracts.Models;

namespace Services.Formatting;

/// <summary>
///     Форматировщик информации о персонажах для отображения
/// </summary>
public class HeroInfoFormatter : IHeroInfoFormatter
{
    /// <summary>
    ///     Форматирует информацию о персонаже (имя и винрейт)
    /// </summary>
    public string Format(Hero hero)
    {
        return
            $"{hero.Name} - {hero.WinRate}% win rate";
    }

    /// <summary>
    ///     Форматирует информацию о персонаже (имя, винрейт и пикрейт)
    /// </summary>
    public string Format(Hero hero, int totalMatchCount)
    {
        return
            $"{hero.Name} - {hero.WinRate * 100:F2}% win rate, {100.0 * hero.MatchCount / totalMatchCount:F2}% pick rate";
    }
}