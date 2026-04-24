using Entities.Classes;
using Services.Contracts.Processing;

namespace Services.Processing;

public class HeroStatsAggregator : IHeroStatsAggregator
{
    public IEnumerable<Hero> AggregateByHero(IEnumerable<HeroStat> stats, IReadOnlyDictionary<int, string> heroNames)
    {
        return stats
            .GroupBy(s => s.HeroId)
            .Select(g => new Hero
            {
                Id = g.Key,
                Name = heroNames.TryGetValue(g.Key, out var name) ? name : $"Hero #{g.Key}",
                WinCount = g.Sum(x => x.WinCount),
                MatchCount = g.Sum(x => x.MatchCount)   
            });
    }
}