using Dota2MetaChecker.Common.Models;
using Services.Contracts.Processing;
using Services.Processing.Extensions;

namespace Services.Processing;

public class HeroCalculator: IHeroCalculator
{
    public Hero Calculate(Hero hero, int totalMatchCount, Hero? previous = null)
    {
        var result = hero
            .WithWinrate()
            .WithPickRate(totalMatchCount)
            .WithRating();

        if (previous is not null)
            result = result.WithDeltas(previous);

        return result;
    }

    public IEnumerable<Hero> CalculateAll(
        IEnumerable<Hero> heroes,
        int totalMatchCount,
        IEnumerable<Hero>? previousHeroes = null)
    {
        var previousMap = previousHeroes?
            .ToDictionary(h => h.Id);

        return heroes.Select(hero =>
        {
            Hero? previous = null;
            previousMap?.TryGetValue(hero.Id, out previous);
            return Calculate(hero, totalMatchCount, previous);
        });
    }
}