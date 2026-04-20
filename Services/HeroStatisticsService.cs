using Entities.Classes;
using Entities.Enums;
using Repository.Contracts;
using Services.Contracts.Stratz;

namespace Services;

public class HeroStatisticsService(
    IStratzApiService stratzApiService,
    IStratzHeroParser stratzHeroParser,
    IMetaStorage metaStorage)
{
    public DateTime? UpdateTime { get; private set; }
    public List<HeroStat>? HeroesStats { get; private set; }
    public Dictionary<int, string>? HeroesNames { get; private set; }

    public async Task UpdateDataAsync()
    {
        HeroesStats = stratzHeroParser.ParseHeroStats(await stratzApiService.GetHeroesStats());
        HeroesNames = stratzHeroParser.ParseHeroesNames(await stratzApiService.GetHeroesNames());
        UpdateTime = DateTime.UtcNow;
    }

    public async Task SaveDataAsync()
    {
        if (UpdateTime != null && HeroesStats != null)
        {
            await metaStorage.SaveDataAsync(HeroesStats, (DateTime)UpdateTime);
        }
    }

    public async Task LoadLastDataAsync()
    {
        HeroesNames = stratzHeroParser.ParseHeroesNames(await stratzApiService.GetHeroesNames());
        var data = await metaStorage.GetLastMetaUpdateAsync();
        UpdateTime = data.dateTime;
        HeroesStats = (List<HeroStat>?)data.heroesStats;
    }
}