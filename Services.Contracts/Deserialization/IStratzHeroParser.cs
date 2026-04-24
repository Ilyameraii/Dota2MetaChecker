using Entities.Classes;
using Entities.Models;

namespace Services.Contracts.Deserialization;

public interface IStratzHeroParser
{
    public List<HeroStat> ParseHeroStats(string json);

    public Dictionary<int, string> ParseHeroesNames(string json);
}