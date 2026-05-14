using Dota2MetaChecker.Common.Enums;

namespace Dota2MetaChecker.Common.Models;

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
}