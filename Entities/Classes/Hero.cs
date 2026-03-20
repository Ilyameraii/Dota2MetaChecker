using System.Text.Json.Serialization;
using Entities.Enums;

namespace Entities.Classes;

public class Hero
{
    // Уникальный ID героя из API (обычно int)
    public int HeroId { get; set; }

    // Название героя (например, "Anti-Mage")
    public string Name { get; set; } = string.Empty;

    // Конкретный ранг, для которого приведена статистика
    // Используем обычный Enum, так как у одной записи один ранг
    public HeroRank Rank { get; set; } 

    // Конкретная роль
    public HeroRole Role { get; set; }

    // Статистика
    public int WinCount { get; set; }   // Например, 52.4 (проценты)
    public int MatchCount { get; set; }  // Например, 15.2 (проценты)
}