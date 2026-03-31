using Entities.Classes;
using Services.Contracts.Stratz;

namespace Services;

public class HeroStatisticsService(IStratzApiService stratzApiService, IStratzHeroParser stratzHeroParser)
{
    public DateTime TimeOfLastUpdate { get; private set; }
    public List<HeroStat>? HeroStats { get; private set; }
    public Dictionary<int, string>? HeroesNames { get; private set; }
 
    public async Task UpdateDataAsync()
    {
        HeroStats = stratzHeroParser.ParseHeroStats(await stratzApiService.GetHeroesStats());
        HeroesNames = stratzHeroParser.ParseHeroesNames(await stratzApiService.GetHeroesNames()); 
        
        TimeOfLastUpdate = DateTime.UtcNow;
        
        foreach (var heroStat in HeroStats)
        {
            heroStat.TimeOfLastUpdate = TimeOfLastUpdate;
        }
    }

    public async Task SaveDataAsync()
    {
        
    }
}