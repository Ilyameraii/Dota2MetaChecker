using Dota2MetaChecker.Common.Models;

namespace Services.Contracts.Processing;

public interface IHeroStatDeltaCalculator
{
    public IEnumerable<Hero> CalculateDeltas(
        IEnumerable<Hero> current,
        IEnumerable<Hero> old,
        int totalMatchCount,
        int oldTotalMatchCount);
}