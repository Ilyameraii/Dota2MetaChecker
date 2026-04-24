using Entities.Classes;
using Repository.Contracts;
using Services.Contracts.Data_sync;
using Services.Contracts.Deserialization;

namespace Services.Data_sync;

public class HeroesDataService(
    IStratzApiService stratzApiService,
    IStratzHeroParser stratzHeroParser,
    IMetaStorage metaStorage,
    HeroesDataCache cache) : IHeroesDataService
{
    public async Task UpdateDataAsync()
    {
        cache.HeroesStats = stratzHeroParser.ParseHeroStats(
            await stratzApiService.GetHeroesStats());
        cache.HeroesNames = stratzHeroParser.ParseHeroesNames(
            await stratzApiService.GetHeroesNames());
        cache.UpdateTime = DateTime.UtcNow;
    }

    public async Task SaveDataAsync()
    {
        if (cache.UpdateTime != null && cache.HeroesStats != null)
            await metaStorage.SaveDataAsync(cache.HeroesStats, cache.UpdateTime.Value);
    }

    public async Task LoadLastDataAsync()
    {
        cache.HeroesNames = stratzHeroParser.ParseHeroesNames(
            await stratzApiService.GetHeroesNames());
        var data = await metaStorage.GetLastMetaUpdateAsync();
        cache.UpdateTime = data.dateTime;
        cache.HeroesStats = (List<HeroStat>?)data.heroStats;
    }
}