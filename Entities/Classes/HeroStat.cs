using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Entities.Enums;

namespace Entities.Classes;

/// <summary>
/// Модель для парсинга данных с STRATZ Api
/// </summary>
public class HeroStat
{
    // ⚠️ Для EF Core: первичный ключ (если нужен)
    [Key]
    public int Id { get; set; } 

    // Данные из API / для ответа
    public int HeroId { get; set; }
    public HeroRank Rank { get; set; }
    public HeroRole Role { get; set; }
    public int WinCount { get; set; }
    public int MatchCount { get; set; }

    // ⚠️ Только для БД: внешний ключ и навигация
    [JsonIgnore] // ← не сериализуется в JSON-ответ
    public int MetaUpdateId { get; set; }
    
    [JsonIgnore]
    public virtual MetaUpdate MetaUpdate { get; set; } = null!;
}