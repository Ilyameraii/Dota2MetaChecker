using Dota2MetaChecker.Common.Models;

namespace Services.Contracts.Processing;

public interface IHeroCalculator
{
    public Hero Calculate(Hero hero, int totalMatchCount, Hero? previous = null);

    public IEnumerable<Hero> CalculateAll(
        IEnumerable<Hero> heroes,
        int totalMatchCount,
        IEnumerable<Hero>? previousHeroes = null);
}