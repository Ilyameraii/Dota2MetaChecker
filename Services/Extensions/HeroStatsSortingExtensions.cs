using Entities.Classes;

namespace Services.Extensions;

public static class HeroStatsSortingExtensions
{
    public static IOrderedEnumerable<Hero> OrderByWinRate(this IEnumerable<Hero> source, bool descending = false) =>
        descending 
            ? source.OrderByDescending(h => (double)h.WinCount / h.MatchCount) 
            : source.OrderBy(h => (double)h.WinCount / h.MatchCount);
    
    public static IOrderedEnumerable<Hero> OrderByMatchCount(this IEnumerable<Hero> source, bool descending = false) =>
        descending ? source.OrderByDescending(h => h.MatchCount) : source.OrderBy(h => h.MatchCount);

    public static IOrderedEnumerable<Hero> OrderByRating(this IEnumerable<Hero> source, bool descending = false) =>
        descending ? source.OrderByDescending(h => h.Rating) : source.OrderBy(h => h.Rating);
}