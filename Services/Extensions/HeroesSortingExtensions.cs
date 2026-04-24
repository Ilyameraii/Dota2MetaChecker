using Entities.Classes;
using Entities.Models;

namespace Services.Extensions;

public static class HeroesSortingExtensions
{
    public static IOrderedEnumerable<Hero> OrderByWinRate(this IEnumerable<Hero> source, bool descending = false) =>
        descending ? source.OrderByDescending(h => h.WinRate) : source.OrderBy(h => h.WinRate);
    
    public static IOrderedEnumerable<Hero> OrderByMatchCount(this IEnumerable<Hero> source, bool descending = false) =>
        descending ? source.OrderByDescending(h => h.MatchCount) : source.OrderBy(h => h.MatchCount);

    public static IOrderedEnumerable<Hero> OrderByRating(this IEnumerable<Hero> source, bool descending = false) =>
        descending ? source.OrderByDescending(h => h.Rating) : source.OrderBy(h => h.Rating);
}