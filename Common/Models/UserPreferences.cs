namespace Dota2MetaChecker.Common.Models;

/// <summary>
///     Настройки пользователя для отображения списка героев
/// </summary>
public class UserPreferences
{
    /// <summary>
    ///     Номер текущей страницы
    /// </summary>
    public int PageNumber { get; set; } = 0;

    /// <summary>
    ///     Параметры обработки (сортировка и фильтрация)
    /// </summary>
    public HeroProcessingOptions ProcessingOptions { get; set; } = new();
}