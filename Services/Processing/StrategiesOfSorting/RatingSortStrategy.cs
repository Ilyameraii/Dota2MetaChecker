using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Extensions;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Processing;

namespace Services.Processing.StrategiesOfSorting;

public class RatingSortStrategy : IHeroSortStategy
{
    public SortType SortType => SortType.Rating;

    public IEnumerable<Hero> Sort(IEnumerable<Hero> heroes, bool descending)
    {
        return heroes.OrderByRating(descending);
    }
}