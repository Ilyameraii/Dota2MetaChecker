using Entities.Classes;
using Services.Contracts.Formatting;

namespace Services.Formatting;

public class HeroInfoFormatter : IHeroInfoFormatter
{
    public string Format(Hero hero)
    {
        return
            $"{hero.Name} - {hero.WinRate}% win rate";
    }
    
    public string Format(Hero hero, int totalMatchCount)
    {
        return
            $"{hero.Name} - {hero.WinRate*100:F2}% win rate, {100.0 * hero.MatchCount / totalMatchCount:F2}% pick rate";
    }
}