using Entities.Models;

namespace Services.Contracts.Data_sync;

/// <summary>
///     Сервис для управления данными персонажей
/// </summary>
public interface IHeroesDataService
{
    /// <summary>
    ///     Обновляет данные о персонажах из STRATZ API
    /// </summary>
    Task UpdateNewStatsAsync();

    /// <summary>
    ///     Сохранение статистики в БД
    /// </summary>
    /// <returns></returns>
    Task SaveNewStatsAsync();

    /// <summary>
    ///     Получение идентификатора обновления старой статистики
    /// </summary>
    Task UpdateOldStatsAsync();

    /// <summary>
    ///     Получение статистики персонажей по идентификатору обновления
    /// </summary>
    /// <param name="metaUpdateId">Идентификатор обновления</param>
    Task<IReadOnlyList<HeroStat>> GetHeroStatsByMetaUpdateIdAsync(int metaUpdateId);

    /// <summary>
    ///     Удаление обновления статистики по идентификатору
    /// </summary>
    Task RemoveNeedlessStatsAsync();
}