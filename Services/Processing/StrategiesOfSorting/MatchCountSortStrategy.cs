using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Processing;
using Services.Extensions;

namespace Services.Processing.StrategiesOfSorting;

public class MatchCountSortStrategy : IHeroSortStategy
{
    public SortType SortType => SortType.MatchCount;

    public IEnumerable<Hero> Sort(IEnumerable<Hero> heroes, bool descending)
    {
        return heroes.OrderByMatchCount(descending);
    }
}