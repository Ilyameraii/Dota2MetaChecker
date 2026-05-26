using Context;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Services.Contracts.Data_sync;
using Services.Contracts.Deserialization;

namespace Services.Data_sync;

/// <summary>
///     Сервис для управления данными персонажей: обновление, сохранение, загрузка
/// </summary>
public class HeroesDataService(
    IStratzApiService apiService,
    IStratzHeroParser heroParser,
    DatabaseContext context,
    HeroesDataCache cache) : IHeroesDataService
{
    /// <summary>
    ///     Обновляет данные о персонажах из API
    /// </summary>
    public async Task UpdateNewStatsAsync(CancellationToken cancellationToken = default)
    {
        cache.NewHeroesStats = heroParser.ParseHeroStats(
            await apiService.GetHeroesStats());
        cache.HeroesNames = heroParser.ParseHeroesNames(
            await apiService.GetHeroesNames());
        cache.UpdateTime = DateTime.UtcNow;
    }

    /// <summary>
    ///     Сохраняет статистику персонажей в БД
    /// </summary>
    public async Task SaveNewStatsAsync()
    {
        if (cache is { UpdateTime: not null, NewHeroesStats: not null })
        {
            var metaUpdate = new MetaUpdate
            {
                DateTime = cache.UpdateTime.Value
            };

            var heroStats = cache.NewHeroesStats;

            foreach (var hero in heroStats) hero.MetaUpdate = metaUpdate;

            await context.MetaUpdates.AddAsync(metaUpdate);
            await context.HeroesStats.AddRangeAsync(heroStats);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    ///     Возвращает статистику персонажей по идентификатору обновления
    /// </summary>
    /// <param name="metaUpdateId">Идентификатор обновления</param>
    public async Task<IReadOnlyList<HeroStat>> GetHeroStatsByMetaUpdateIdAsync(int metaUpdateId)
    {
        return await context.HeroesStats
            .AsNoTracking()
            .Where(h => h.MetaUpdateId == metaUpdateId)
            .ToListAsync();
    }

    /// <summary>
    ///     Загружает в кэш статистику недельной давности
    /// </summary>
    public async Task UpdateOldStatsAsync()
    {
        var lastUpdateId = await GetLastUpdateId();
        var weekOldUpdateId = Math.Max(lastUpdateId - 7 * 24, 1);

        cache.OldHeroesStats = await context.HeroesStats
            .AsNoTracking()
            .Where(h => h.MetaUpdateId == weekOldUpdateId)
            .ToListAsync();
    }

    /// <summary>
    ///     Удаляет обновления старше недели из БД
    /// </summary>
    public async Task RemoveNeedlessStatsAsync()
    {
        var lastUpdateId = await GetLastUpdateId();
        var oldestNeededUpdateId = lastUpdateId - 7 * 24;

        if (oldestNeededUpdateId > 1)
            await context.MetaUpdates
                .Where(m => m.Id < oldestNeededUpdateId)
                .ExecuteDeleteAsync();
    }

    public async Task DuplicateLastStatsAsync()
    {
        var lastUpdateId = await GetLastUpdateId();
        if (lastUpdateId == 0)
        {
            return;
        }

        var lastStats = await context.HeroesStats
            .AsNoTracking()
            .Where(h => h.MetaUpdateId == lastUpdateId)
            .ToListAsync();

        if (!lastStats.Any())
        {
            return;
        }

        var metaUpdate = new MetaUpdate
        {
            DateTime = DateTime.UtcNow
        };

        var duplicatedStats = lastStats.Select(h => new HeroStat
        {
            // копируем все поля кроме Id и MetaUpdateId
            HeroId = h.HeroId,
            WinCount = h.WinCount,
            MatchCount = h.MatchCount,
            Rank = h.Rank,
            Role =  h.Role,
            MetaUpdate = metaUpdate
        }).ToList();

        await context.MetaUpdates.AddAsync(metaUpdate);
        await context.HeroesStats.AddRangeAsync(duplicatedStats);
        await context.SaveChangesAsync();

        // Обновляем кэш чтобы бот не отдавал пустые данные
        cache.NewHeroesStats = duplicatedStats;
        cache.UpdateTime = metaUpdate.DateTime;

        Console.WriteLine("Данные продублированы из обновления {0}: {1}", lastUpdateId, DateTime.UtcNow);
    }
    
    private async Task<int> GetLastUpdateId()
    {
        return await context.MetaUpdates
            .OrderByDescending(m => m.Id)
            .Select(m => m.Id)
            .FirstOrDefaultAsync();
    }
}