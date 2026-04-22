using Entities.Classes;
using Entities.Enums;
using Repository.Contracts;
using Services.Contracts.Stratz;

namespace Services;

public class HeroesDataHolder(
    IStratzApiService stratzApiService,
    IStratzHeroParser stratzHeroParser,
    IMetaStorage metaStorage)
{
    private DateTime? updateTime;
    private List<HeroStat>? heroesStats;
    private Dictionary<int, string>? heroesNames;

    
    public IReadOnlyList<HeroStat> HeroesStats => 
        heroesStats?.AsReadOnly() 
        ?? throw new InvalidOperationException("Данные не загружены. Вызовите LoadLastDataAsync или UpdateDataAsync.");

    public IReadOnlyDictionary<int, string> HeroesNames => 
        heroesNames 
        ?? throw new InvalidOperationException("Имена героев не загружены. Вызовите LoadLastDataAsync или UpdateDataAsync.");

    public async Task UpdateDataAsync()
    {
        heroesStats = stratzHeroParser.ParseHeroStats(await stratzApiService.GetHeroesStats());
        heroesNames = stratzHeroParser.ParseHeroesNames(await stratzApiService.GetHeroesNames());
        updateTime = DateTime.UtcNow;
    }

    public async Task SaveDataAsync()
    {
        if (updateTime != null && heroesStats != null)
        {
            await metaStorage.SaveDataAsync(heroesStats, (DateTime)updateTime);
        }
    }

    public async Task LoadLastDataAsync()
    {
        heroesNames = stratzHeroParser.ParseHeroesNames(await stratzApiService.GetHeroesNames());
        var data = await metaStorage.GetLastMetaUpdateAsync();
        updateTime = data.dateTime;
        heroesStats = (List<HeroStat>?)data.heroStats;
    }
}