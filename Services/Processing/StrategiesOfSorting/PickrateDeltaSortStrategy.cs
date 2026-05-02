using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Extensions;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Processing;

namespace Services.Processing.StrategiesOfSorting;

public class PickrateDeltaSortStrategy : IHeroSortStategy
{
    public SortType SortType => SortType.PickrateDelta;

    public IEnumerable<Hero> Sort(IEnumerable<Hero> heroes, bool descending)
    {
        return heroes.OrderByPickrateDelta(descending);
    }
}