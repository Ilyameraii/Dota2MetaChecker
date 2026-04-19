using Context.Data;
using Context.Models;
using Repository.Contracts;
using HeroStat = Entities.Classes.HeroStat;

namespace Repository;

public class DatabaseStorage(DatabaseContext context) : IMetaStorage
{

    public async Task SaveDataAsync(List<HeroStat> heroesData, DateTime dateTime)
    {
        var metaUpdate = new MetaUpdate
        {
            DateTime = dateTime,
        };
        var heroStats = heroesData.Select(s => new Context.Models.HeroStat()
        {
            HeroId = s.HeroId,
            HeroRank =  s.Rank.ToString(),
            HeroRole =  s.Role.ToString(),
            WinCount = s.WinCount,
            MatchCount = s.MatchCount,
            MetaUpdate = metaUpdate
        }).ToList();
        await context.MetaUpdates.AddAsync(metaUpdate);    
        await context.HeroStats.AddRangeAsync(heroStats); 
        await context.SaveChangesAsync();
    }
}