using Dota2MetaChecker.Common.Models;

namespace Dota2MetaChecker.Common.Extensions;

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
}