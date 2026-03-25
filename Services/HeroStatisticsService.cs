using Entities.Classes;
using Services.Contracts.Stratz;

namespace Services;

public class HeroStatisticsService(IStratzHeroDataOrchestrator stratzHeroDataOrchestrator)
{
    public DateTime TimeOfLastUpdate { get; private set; }
    public List<Hero>? Heroes { get; private set; }
 
    public async Task UpdateDataAsync()
    {
        TimeOfLastUpdate = DateTime.UtcNow;
        Heroes = await stratzHeroDataOrchestrator.GetHeroesAsync();
    }
}