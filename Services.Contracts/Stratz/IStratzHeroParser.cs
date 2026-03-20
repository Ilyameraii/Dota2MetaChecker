using System.Text.Json;
using System.Text.Json.Nodes;
using Entities.Classes;

namespace Services.Contracts.Stratz;

public interface IStratzHeroParser
{
    public List<Hero> ParseHeroStats(string json, Dictionary<int, string> names);

    public Dictionary<int, string> ParseHeroesNames(string json);
}