namespace Entities.Classes;

/// <summary>
/// Модель для отображения данных
/// </summary>
public class Hero
{
    /// <summary>
    /// Айди персонажа
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Имя персонажа
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Количество побед на персонаже
    /// </summary>
    public int WinCount { get; set; }
    
    /// <summary>
    /// Количество матчей на персонаже
    /// </summary>
    public int MatchCount { get; set; }
    
    /// <summary>
    /// Рейтинг персонажа (разница побед и поражений)
    /// </summary>
    public int Rating => WinCount * 2 - MatchCount;
}