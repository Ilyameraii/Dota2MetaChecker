namespace Services.Contracts.Enums;

/// <summary>
/// Флаги для фильтрации по рангам
/// </summary>
[Flags]
public enum RankFlags
{
    /// <summary>
    /// Остутствие флагов
    /// </summary>
    None = 0,

    /// <summary>
    /// Флаг на статистику с неоткалиброванных званий
    /// </summary>
    Uncalibrated = 1 << 0,

    /// <summary>
    /// Флаг на статистику со званий Рекрут-Страж
    /// </summary>
    HeraldGuardian = 1 << 1,

    /// <summary>
    /// Флаг на статистику со званий Рыцарь-Герой
    /// </summary>
    CrusaderArchon = 1 << 2,

    /// <summary>
    /// Флаг на статистику со званий Легенда-Властелин
    /// </summary>
    LegendAncient = 1 << 3,

    /// <summary>
    /// Флаг на статистику со званий Божество-Титан
    /// </summary>
    DivineImmortal = 1 << 4,
}