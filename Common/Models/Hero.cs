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
    public double Rating => CalculateRating(WinCount, MatchCount);

    /// <summary>
    ///     Изменение пикрейта относительно предыдущего периода
    /// </summary>
    public double PickRateDelta { get; init; } = 0;
    
    /// <summary>
    ///     Изменение винрейта относительно предыдущего периода
    /// </summary>
    public double WinRateDelta { get; init; } = 0;
    
    /// <summary>
    ///     Изменение рейтинга относительно предыдущего периода
    /// </summary>
    public double RatingDelta { get; init; } = 0;
    
    // Рассчитывает статистический рейтинг эффективности героя на основе нижней границы интервала Вильсона (Wilson Score Interval).
    private double CalculateRating(int wins, int totalMatches)
    {
        var p = (double)wins / totalMatches;
        var n = totalMatches;
        var z = 1.96; // 95% confidence

        // 2. Расчет нижней границы интервала Вильсона
        var score = (p + Math.Pow(z, 2) / (2 * n) -
                     z * Math.Sqrt(p * (1 - p) / n + Math.Pow(z, 2) / (4 * Math.Pow(n, 2)))) / (1 + Math.Pow(z, 2) / n);

        return score;
    }
}