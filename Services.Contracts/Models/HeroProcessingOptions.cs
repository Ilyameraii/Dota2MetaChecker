using Entities.Classes;
using Entities.Enums;
using Entities.Models;

namespace Services.Contracts.Models;

/// <summary>
/// Параметры для обработки статистики персонажей
/// </summary>
public class HeroProcessingOptions
{
    /// <summary>
    /// Фильтр по рангам
    /// </summary>
    public RankFlags Ranks { get; init; } = RankFlags.None;
    
    /// <summary>
    /// Фильтр по ролям
    /// </summary>
    public RoleFlags Roles { get; init; } = RoleFlags.None;
    
    /// <summary>
    /// Функция сортировки результатов
    /// </summary>
    public Func<IEnumerable<Hero>, IOrderedEnumerable<Hero>>? SortBy { get; init; }
}