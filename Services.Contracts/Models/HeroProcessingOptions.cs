using Services.Contracts.Enums;
using Services.Contracts.Extensions;

namespace Services.Contracts.Models;

/// <summary>
///     Параметры для обработки статистики персонажей
/// </summary>
public class HeroProcessingOptions
{
    /// <summary>
    ///     Фильтр по рангам
    /// </summary>
    public RankFlags Ranks { get; set; } = RankFlags.None;

    /// <summary>
    ///     Фильтр по ролям
    /// </summary>
    public RoleFlags Roles { get; set; } = RoleFlags.None;

    /// <summary>
    ///     Тип сортировки
    /// </summary>
    public SortType SortBy { get; set; } = SortType.Rating;

    /// <summary>
    ///     Сортировка по убыванию
    /// </summary>
    public bool IsDescending { get; set; } = true;

    /// <summary>
    ///     Возвращает функцию сортировки на основе текущих настроек
    /// </summary>
    public Func<IEnumerable<Hero>, IOrderedEnumerable<Hero>> GetSortFunction()
    {
        return SortBy switch
        {
            SortType.MatchCount => h => h.OrderByMatchCount(IsDescending),
            SortType.WinRate => h => h.OrderByWinRate(IsDescending),
            SortType.Rating => h => h.OrderByRating(IsDescending),
            _ => h => h.OrderByRating(IsDescending)
        };
    }
}

/// <summary>
///     Тип сортировки героев
/// </summary>
public enum SortType
{
    /// <summary>
    ///     По количеству матчей
    /// </summary>
    MatchCount,

    /// <summary>
    ///     По винрейту
    /// </summary>
    WinRate,

    /// <summary>
    ///     По рейтингу
    /// </summary>
    Rating
}