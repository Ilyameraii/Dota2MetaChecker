namespace Services.Contracts.Enums;

/// <summary>
/// Флаги для фильтрации по ролям
/// </summary>
[Flags]
public enum RoleFlags
{ 
    /// <summary>
    /// Отсутствие флагов
    /// </summary>
    None = 0,

    /// <summary>
    /// Флаг на роль - Легкая линия / Керри /  1 позиция
    /// </summary>
    Safelane = 1 << 0,  

    /// <summary>
    /// Флаг на роль - Центральная линия / Мидер / 2 позиция
    /// </summary>
    Midlane = 1 << 1,   

    /// <summary>
    /// Флаг на роль - Сложная линия / Оффлейнер / 3 позиция
    /// </summary>
    Offlane = 1 << 2,   

    /// <summary>
    /// Флаг на роль - Частичная поддержка / Саппорт / Четвертая позиция (на линии с оффлейнером)
    /// </summary>
    Support = 1 << 3,   

    /// <summary>
    /// Флаг на роль - Полная поддержка / Фулсаппорт / Пятая позиция (на линии с керри)
    /// </summary>
    HardSupport = 1 << 4,  
}