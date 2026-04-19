using Entities.Classes;
using Repository.Contracts;
using Services.Contracts.Stratz;

namespace Services;

public class HeroStatisticsService(IStratzApiService stratzApiService, IStratzHeroParser stratzHeroParser, IMetaStorage metaStorage)
{
    public DateTime UpdateTime { get; private set; }
    public List<HeroStat>? HeroStats { get; private set; }
    public Dictionary<int, string>? HeroesNames { get; private set; }
 
    public async Task UpdateDataAsync()
    {
        HeroStats = stratzHeroParser.ParseHeroStats(await stratzApiService.GetHeroesStats());
        HeroesNames = stratzHeroParser.ParseHeroesNames(await stratzApiService.GetHeroesNames()); 
        UpdateTime = DateTime.UtcNow;
    }

    public async Task SaveDataAsync()
    {
        await metaStorage.SaveDataAsync(HeroStats, UpdateTime);
    }
}