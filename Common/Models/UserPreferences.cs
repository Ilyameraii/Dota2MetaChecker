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
    
    /// <summary>
    ///     Сбрасывает настройки пользователя до значений по умолчанию.
    /// </summary>
    public void Reset()
    {
        ProcessingOptions = new HeroProcessingOptions();
        PageNumber = 0;
    }
}