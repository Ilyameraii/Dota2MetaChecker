using System.Text.Json.Serialization;
using Entities.Enums;

namespace Entities.Classes;

/// <summary>
/// Модель для парсинга данных с STRATZ Api
/// </summary>
public class HeroStat
{
    // Уникальный ID героя из API (обычно int)
    public int HeroId { get; set; }


    // Конкретный ранг, для которого приведена статистика
    // Используем обычный Enum, так как у одной записи один ранг
    public HeroRank Rank { get; set; } 

    // Конкретная роль
    public HeroRole Role { get; set; }

    // Статистика
    public int WinCount { get; set; }  
    public int MatchCount { get; set; } 
    
}