namespace Entities.Classes;

public class MetaSnapshot
{
    public List<HeroStat> HeroStats { get; init; } = [];
    public Dictionary<int, string> HeroesNames { get; init; } = [];
    public DateTime FetchedAt { get; init; }
}