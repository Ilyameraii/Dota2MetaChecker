namespace Entities.Enums;

/// <summary>
/// Флаги рангов для фильтрации статистики
/// </summary>
[Flags]
public enum RankFlags
{
    /// <summary>
    /// Нет выбранных рангов (значение по умолчанию)
    /// </summary>
    None = 0,

    /// <summary>
    /// Статистика с неоткалиброванных званий
    /// </summary>
    Uncalibrated = 1 << 0,  // 1

    /// <summary>
    /// Статистика со званий Рекрут-Страж
    /// </summary>
    HeraldGuardian = 1 << 1,  // 2

    /// <summary>
    /// Статистика со званий Рыцарь-Герой
    /// </summary>
    CrusaderArchon = 1 << 2,  // 4

    /// <summary>
    /// Статистика со званий Легенда-Властелин
    /// </summary>
    LegendAncient = 1 << 3,  // 8

    /// <summary>
    /// Статистика со званий Божество-Бессмертный
    /// </summary>
    DivineImmortal = 1 << 4,  // 16
}