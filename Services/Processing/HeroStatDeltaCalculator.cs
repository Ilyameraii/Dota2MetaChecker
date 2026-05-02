using Dota2MetaChecker.Common.Models;
using Services.Contracts.Processing;

namespace Services.Processing;

public class HeroStatDeltaCalculator : IHeroStatDeltaCalculator
{
    public IEnumerable<Hero> CalculateDeltas(
        IEnumerable<Hero> current,
        IEnumerable<Hero> old,
        int totalMatchCount,
        int oldTotalMatchCount)
    {
        var oldDict = old.ToDictionary(h => h.Id);

        return current.Select(hero =>
        {
            if (!oldDict.TryGetValue(hero.Id, out var oldHero))
                return hero;

            return hero.WithDeltas(
                hero.WinRate - oldHero.WinRate,
                (double)hero.MatchCount / totalMatchCount -
                (double)oldHero.MatchCount / oldTotalMatchCount
            );
        });
    }
}