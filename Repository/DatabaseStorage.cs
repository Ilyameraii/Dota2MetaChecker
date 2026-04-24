using Context;
using Entities.Classes;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;
using HeroStat = Entities.Models.HeroStat;

namespace Repository;

/// <summary>
/// Класс для работы с базой данных
/// </summary>
/// <param name="context">Контекст базы данных</param>
public class DatabaseStorage(DatabaseContext context) : IMetaStorage
{
    /// <summary>
    /// Сохранение статистики в БД
    /// </summary>
    /// <param name="heroStats">Статистика героев</param>
    /// <param name="dateTime">Время получения статистики</param>
    public async Task SaveDataAsync(IReadOnlyList<HeroStat> heroStats, DateTime dateTime)
    {
        var metaUpdate = new MetaUpdate
        {
            DateTime = dateTime,
        };
        
        foreach (var hero in heroStats)
        {
            hero.MetaUpdate = metaUpdate;
        }
        
        await context.MetaUpdates.AddAsync(metaUpdate);
        await context.HeroesStats.AddRangeAsync(heroStats);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Получение статистики персонажей по идентификатору обновления
    /// </summary>
    /// <param name="metaUpdateId">Идентификатор обновления</param>
    public async Task<IReadOnlyList<HeroStat>> GetHeroStatsByMetaUpdateIdAsync(int metaUpdateId)
    {
        return await context.HeroesStats.AsNoTracking().Where(h => h.MetaUpdateId == metaUpdateId).ToListAsync();
    }

    /// <summary>
    /// Получение последнего обновления статистики
    /// </summary>
    public async Task<(IReadOnlyList<HeroStat> heroStats, DateTime? dateTime)> GetLastMetaUpdateAsync()
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