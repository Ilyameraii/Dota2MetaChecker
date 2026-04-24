using Entities.Classes;

namespace Services.Contracts.Deserialization;

public interface IStratzHeroParser
{
    public List<HeroStat> ParseHeroStats(string json);

    public Dictionary<int, string> ParseHeroesNames(string json);
}