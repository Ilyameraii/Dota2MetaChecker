using Entities.Models;

namespace Repository.Contracts;

/// <summary>
/// Интерфейс, предоставляющий доступ к БД, храняющей статистику персонажей
/// </summary>
public interface IMetaStorage
{
    /// <summary>
    /// Сохранение статистики в БД
    /// </summary>
    /// <param name="heroStats">Статистика героев</param>
    /// <param name="dateTime">Время получения статистики</param>
    /// <returns></returns>
    Task SaveDataAsync(IReadOnlyList<HeroStat> heroStats, DateTime dateTime);
    
    /// <summary>
    /// Получение статистики персонажей по идентификатору обновления
    /// </summary>
    /// <param name="metaUpdateId">Идентификатор обновления</param>
    Task<IReadOnlyList<HeroStat>> GetHeroStatsByMetaUpdateIdAsync(int metaUpdateId);

    /// <summary>
    /// Получение последнего обновления статистики
    /// </summary>
    Task<(IReadOnlyList<HeroStat> heroStats, DateTime? dateTime)> GetLastMetaUpdateAsync();
}