using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Entities.Classes;
using Entities.Enums;

namespace Entities.Models;

/// <summary>
/// Модель для парсинга данных с STRATZ API
/// </summary>
public class HeroStat
{
    /// <summary>
    /// Идентификатор статистики - первичный ключ
    /// </summary>
    [Key]
    public int Id { get; set; } 

    /// <summary>
    /// Идентификатор персонажа
    /// </summary>
    public int HeroId { get; set; }
    
    /// <summary>
    /// Ранг матчей, на котором ведется статистика персонажа
    /// </summary>
    public Rank Rank { get; set; }
    
    /// <summary>
    /// Роль, на которой ведется статистика персонажа
    /// </summary>
    public Role Role { get; set; }
    
    /// <summary>
    /// Количество побед персонажа на конкретной роли и на конкретном ранге
    /// </summary>
    public int WinCount { get; set; }
    
    /// <summary>
    /// Количество матчей на персонаже на конкретной роли и на конкретном ранге
    /// </summary>
    public int MatchCount { get; set; }

    /// <summary>
    /// Идентификатор обновления статистики
    /// </summary>
    // Только для БД: внешний ключ и навигация
    [JsonIgnore] // ← не сериализуется в JSON-ответ
    public int MetaUpdateId { get; set; }
    
    /// <summary>
    /// Навигационное свойство для связи с таблицей MetaUpdate
    /// </summary>
    [JsonIgnore]
    public virtual MetaUpdate MetaUpdate { get; set; } = null!;
}