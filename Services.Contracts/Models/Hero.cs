namespace Services.Contracts.Models;

/// <summary>
/// Модель персонажа с агрегированной статистикой
/// </summary>
public class Hero
{
    /// <summary>
    /// Идентификатор персонажа
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Имя персонажа
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Количество побед
    /// </summary>
    public int WinCount { get; init; }
    
    /// <summary>
    /// Количество матчей
    /// </summary>
    public int MatchCount { get; init; }

    /// <summary>
    /// Процент побед
    /// </summary>
    public float WinRate => MatchCount > 0 ? (float)WinCount / MatchCount : 0;
    
    /// <summary>
    /// Рейтинг персонажа
    /// </summary>
    public int Rating => WinCount * 2 - MatchCount;
}