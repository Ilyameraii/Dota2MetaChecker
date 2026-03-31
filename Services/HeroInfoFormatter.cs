using Entities.Classes;
using Services.Contracts;

namespace Services;

public class HeroInfoFormatter : IHeroInfoFormatter
{
    public string Format(Hero hero, int totalMatches)
    {
        return
            $"{hero.Name} - {100.0 * hero.WinCount / hero.MatchCount:F2}% winrate, {100.0 * hero.MatchCount / totalMatches:F2}% pickrate";
    }
}