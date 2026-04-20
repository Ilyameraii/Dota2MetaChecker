using Context;
using Entities.Classes;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;
using HeroStat = Entities.Classes.HeroStat;

namespace Repository;

public class DatabaseStorage(DatabaseContext context) : IMetaStorage
{
    public async Task SaveDataAsync(IReadOnlyList<HeroStat> heroesData, DateTime dateTime)
    {
        var metaUpdate = new MetaUpdate
        {
            DateTime = dateTime,
        };
        
        foreach (var hero in heroesData)
        {
            hero.MetaUpdate = metaUpdate;
        }
        
        await context.MetaUpdates.AddAsync(metaUpdate);
        await context.HeroesStats.AddRangeAsync(heroesData);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<HeroStat>> GetHeroStatsByMetaUpdateIdAsync(int metaUpdateId)
    {
        return await context.HeroesStats.AsNoTracking().Where(h => h.MetaUpdateId == metaUpdateId).ToListAsync();
    }

    public async Task<(IReadOnlyList<HeroStat> heroesStats, DateTime? dateTime)> GetLastMetaUpdateAsync()
    {
        // Находим последнее обновление и сразу загружаем связанные данные
        var lastUpdate = await context.MetaUpdates
            .AsNoTracking()
            .OrderByDescending(m => m.Id)
            .Select(m => new
            {
                m.Id,
                m.DateTime,
                m.HeroStats // навигационное свойство
            })
            .FirstOrDefaultAsync();

        if (lastUpdate == null)
            return (Array.Empty<HeroStat>(), null);

        return (lastUpdate.HeroStats.ToList(), lastUpdate.DateTime);
    }
}