using Dota2MetaChecker.Common.Models;

namespace Services.Extensions;

/// <summary>
///     Методы расширения для сортировки коллекции персонажей
/// </summary>
public static class HeroesSortingExtensions
{
    /// <summary>
    ///     Сортировка по винрейту
    /// </summary>
    public static IOrderedEnumerable<Hero> OrderByWinRate(this IEnumerable<Hero> source, bool descending = false)
    {
        return descending ? source.OrderByDescending(h => h.WinRate) : source.OrderBy(h => h.WinRate);
    }

    /// <summary>
    ///     Сортировка по количеству матчей
    /// </summary>
    public static IOrderedEnumerable<Hero> OrderByMatchCount(this IEnumerable<Hero> source, bool descending = false)
    {
        return descending ? source.OrderByDescending(h => h.MatchCount) : source.OrderBy(h => h.MatchCount);
    }

    /// <summary>
    ///     Сортировка по рейтингу
    /// </summary>
    public static IOrderedEnumerable<Hero> OrderByRating(this IEnumerable<Hero> source, bool descending = false)
    {
        return descending ? source.OrderByDescending(h => h.Rating) : source.OrderBy(h => h.Rating);
    }

    /// <summary>
    ///     Сортировка по рейтингу
    /// </summary>
    public static IOrderedEnumerable<Hero> OrderByWinrateDelta(this IEnumerable<Hero> source, bool descending = false)
    {
        return descending ? source.OrderByDescending(h => h.WinRateDelta) : source.OrderBy(h => h.WinRateDelta);
    }

    /// <summary>
    ///     Сортировка по рейтингу
    /// </summary>
    public static IOrderedEnumerable<Hero> OrderByPickrateDelta(this IEnumerable<Hero> source, bool descending = false)
    {
        return descending ? source.OrderByDescending(h => h.PickRateDelta) : source.OrderBy(h => h.PickRateDelta);
    }
    
    /// <summary>
    ///     Сортировка по росту рейтинга
    /// </summary>
    public static IOrderedEnumerable<Hero> OrderByRatingDelta(this IEnumerable<Hero> source, bool descending = false)
    {
        return descending ? source.OrderByDescending(h => h.PickRateDelta) : source.OrderBy(h => h.PickRateDelta);
    }
}