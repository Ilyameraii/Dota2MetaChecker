namespace Entities.Classes;

/// <summary>
/// Модель обновления статистики
/// </summary>
public class MetaUpdate
{
    /// <summary>
    /// Идентификатор обновления статистики
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Время обновления статистики
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    ///  Навигационное свойство для связи с таблицей HeroStat
    /// </summary>
    public virtual ICollection<HeroStat> HeroStats { get; set; } = new List<HeroStat>();
}
