using Entities.Classes;
using Entities.Models;

namespace Services.Data_sync;

/// <summary>
/// Кэш для хранения данных о персонажах и времени последнего обновления
/// </summary>
public class HeroesDataCache
{
    /// <summary>
    /// Статистика персонажей
    /// </summary>
    public List<HeroStat>? HeroesStats { get; set; }
    
    /// <summary>
    /// Словарь соответствия идентификаторов и имен персонажей
    /// </summary>
    public Dictionary<int, string>? HeroesNames { get; set; }
    
    /// <summary>
    /// Время последнего обновления
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// Проверка загрузки данных
    /// </summary>
    public bool IsLoaded => HeroesStats != null && HeroesNames != null;
}