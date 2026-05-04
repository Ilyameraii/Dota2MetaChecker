using Dota2MetaChecker.Common.Constants;

namespace Dota2MetaChecker.Common.Models;

/// <summary>
///     Модель персонажа с агрегированной статистикой
/// </summary>
public record Hero
{
    /// <summary>
    ///     Идентификатор персонажа
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    ///     Имя персонажа
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     Количество побед
    /// </summary>
    public int WinCount { get; init; }

    /// <summary>
    ///     Количество матчей
    /// </summary>
    public int MatchCount { get; init; }

    /// <summary>
    ///     Процент побед
    /// </summary>
    public double WinRate { get; init; }
    
    /// <summary>
    ///     Процент побед
    /// </summary>
    public double PickRate { get; init; }

    /// <summary>
    ///     Рейтинг персонажа
    /// </summary>
    public double Rating { get; init; }

    /// <summary>
    ///     Изменение пикрейта относительно предыдущего периода
    /// </summary>
    public double PickRateDelta { get; init; }

    /// <summary>
    ///     Изменение винрейта относительно предыдущего периода
    /// </summary>
    public double WinRateDelta { get; init; }

    /// <summary>
    ///     Изменение рейтинга относительно предыдущего периода
    /// </summary>
    public double RatingDelta { get; init; }
}