using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Processing;
using Services.Extensions;
using Services.Processing.StrategiesOfSorting.Extensions;

namespace Services.Processing.StrategiesOfSorting;

public class WinrateSortStrategy : IHeroSortStategy
{
    public SortType SortType => SortType.WinRate;

    public IEnumerable<Hero> Sort(IEnumerable<Hero> heroes, bool descending)
    {
        return heroes.OrderByWinRate(descending);
    }
}