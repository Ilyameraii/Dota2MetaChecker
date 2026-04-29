using Services.Contracts.Models;

namespace Services.Contracts.Formatting;

/// <summary>
///     Форматировщик информации о персонажах
/// </summary>
public interface IHeroInfoFormatter
{
    /// <summary>
    ///     Форматирует информацию о персонаже (имя и винрейт)
    /// </summary>
    string Format(Hero hero);

    /// <summary>
    ///     Форматирует информацию о персонаже (имя, винрейт и пикрейт)
    /// </summary>
    string Format(Hero hero, int totalMatchCount);
}