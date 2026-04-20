using System.Runtime.Serialization;

namespace Entities.Enums;

/// <summary>
/// Значения ролей, на которой ведется статистика персонажа
/// </summary>
public enum Role
{
    /// <summary>
    /// Легкая линия / Керри /  1 позиция
    /// </summary>
    [EnumMember(Value = "POSITION_1")] Safelane,
    
    /// <summary>
    /// Центральная линия / Мидер / 2 позиция
    /// </summary>
    [EnumMember(Value = "POSITION_2")] Midlane,
    
    /// <summary>
    /// Сложная линия / Оффлейнер / 3 позиция
    /// </summary>
    [EnumMember(Value = "POSITION_3")] Offlane,
    
    /// <summary>
    /// Частичная поддержка / Саппорт / Четвертая позиция (на линии с оффлейнером)
    /// </summary>
    [EnumMember(Value = "POSITION_4")] Support,
    
    /// <summary>
    /// Полная поддержка / Фулсаппорт / Пятая позиция (на линии с керри)
    /// </summary>
    [EnumMember(Value = "POSITION_5")] HardSupport,
}