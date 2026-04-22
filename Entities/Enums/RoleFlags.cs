using System.Runtime.Serialization;

namespace Entities.Enums;

/// <summary>
/// Флаги ролей для фильтрации статистики
/// </summary>
[Flags]
public enum RoleFlags
{
    /// <summary>
    /// Нет выбранных ролей (значение по умолчанию)
    /// </summary>
    None = 0,

    /// <summary>
    /// Легкая линия / Керри / 1 позиция
    /// </summary>
    Safelane = 1 << 0,  // 1

    /// <summary>
    /// Центральная линия / Мидер / 2 позиция
    /// </summary>
    Midlane = 1 << 1,   // 2

    /// <summary>
    /// Сложная линия / Оффлейнер / 3 позиция
    /// </summary>
    Offlane = 1 << 2,   // 4

    /// <summary>
    /// Частичная поддержка / Саппорт / 4 позиция
    /// </summary>
    Support = 1 << 3,   // 8

    /// <summary>
    /// Полная поддержка / Фулсаппорт / 5 позиция
    /// </summary>
    HardSupport = 1 << 4,  // 16
}