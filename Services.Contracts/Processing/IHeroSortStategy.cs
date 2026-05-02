using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;

namespace Services.Contracts.Processing;

public interface IHeroSortStategy
{
    SortType SortType { get; }
    IEnumerable<Hero> Sort(IEnumerable<Hero> heroes, bool descending);
}