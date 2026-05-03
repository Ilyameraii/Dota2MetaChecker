using Dota2MetaChecker.Common.Constants;

namespace Dota2MetaChecker.Common.Models;

/// <summary>
///     Модель персонажа с агрегированной статистикой
/// </summary>
public class Hero
{
    public Hero WithDeltas(double winRateDelta, double pickRateDelta, double ratingDelta)
    {
        return new Hero
        {
            Id = Id,
            Name = Name,
            WinCount = WinCount,
            MatchCount = MatchCount,
            WinRateDelta = winRateDelta,
            PickRateDelta = pickRateDelta,
            RatingDelta = ratingDelta
        };
    }

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
    public float WinRate => MatchCount > 0 ? (float)WinCount / MatchCount : 0;

    /// <summary>
    ///     Рейтинг персонажа
    /// </summary>
    public double Rating
    {
        get
        {
            if (MatchCount < HeroRatingConstants.MinMatchesForRating)
            {
                return double.MinValue;
            }
            return HeroRatingConstants.WinrateImpactValue * (WinRate - 0.50) + Math.Log(MatchCount);
        }
    }

    /// <summary>
    ///     Изменение пикрейта относительно предыдущего периода
    /// </summary>
    public double PickRateDelta { get; private init; }

    /// <summary>
    ///     Изменение винрейта относительно предыдущего периода
    /// </summary>
    public double WinRateDelta { get; private init; }

    /// <summary>
    ///     Изменение рейтинга относительно предыдущего периода
    /// </summary>
    public double RatingDelta { get; private init; }
}