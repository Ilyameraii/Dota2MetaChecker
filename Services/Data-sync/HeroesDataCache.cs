using Entities.Classes;
using Entities.Enums;
using Entities.Models;
using Services.Contracts.Models;

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
    /// Настройки пользователей (номер страницы и параметры обработки)
    /// </summary>
    public Dictionary<long, UserPreferences> UserPreferences { get; set; } = new();

    /// <summary>
    /// Проверка загрузки данных
    /// </summary>
    public bool IsLoaded => HeroesStats != null && HeroesNames != null;

    /// <summary>
    /// Проверка, устарели ли данные (больше чем 1 час)
    /// </summary>
    public bool IsStale => !UpdateTime.HasValue || DateTime.UtcNow - UpdateTime.Value > TimeSpan.FromHours(1);
}

/// <summary>
/// Настройки пользователя для отображения списка героев
/// </summary>
public class UserPreferences
{
    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public int PageNumber { get; set; } = 0;

    /// <summary>
    /// Параметры обработки (сортировка и фильтрация)
    /// </summary>
    public HeroProcessingOptions ProcessingOptions { get; set; } = new();
}