using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Extensions;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Processing;

namespace Services.Processing.StrategiesOfSorting;

public class WinrateDeltaSortStrategy : IHeroSortStategy
{
    public SortType SortType => SortType.WinrateDelta;

    public IEnumerable<Hero> Sort(IEnumerable<Hero> heroes, bool descending)
    {
        return heroes.OrderByWinrateDelta(descending);
    }
}