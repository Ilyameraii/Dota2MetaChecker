using System.Text.Json;
using System.Text.Json.Nodes;
using Entities.Classes;

namespace Services.Contracts.Stratz;

public interface IStratzHeroParser
{
    public List<HeroStat> ParseHeroStats(string json);

    public Dictionary<int, string> ParseHeroesNames(string json);
}