using Entities.Models;

namespace Services.Data_sync;

/// <summary>
///     Кэш для хранения данных о персонажах и времени последнего обновления
/// </summary>
public class HeroesDataCache
{
    /// <summary>
    ///     Время последнего обновления
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    ///     Новая статистика персонажей
    /// </summary>
    public IReadOnlyList<HeroStat>? NewHeroesStats { get; set; }

    /// <summary>
    ///     Старая статистика персонажей
    /// </summary>
    public IReadOnlyList<HeroStat>? OldHeroesStats { get; set; }

    /// <summary>
    ///     Словарь соответствия идентификаторов и имен персонажей
    /// </summary>
    public Dictionary<int, string>? HeroesNames { get; set; }

    public int HeroCount => HeroesNames?.Count ?? 0;

    /// <summary>
    ///     Проверка загрузки данных
    /// </summary>
    public bool IsLoaded => NewHeroesStats != null && HeroesNames != null;
}