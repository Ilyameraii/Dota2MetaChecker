namespace Entities.Classes;

public class MetaUpdate
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public virtual ICollection<HeroStat> HeroStats { get; set; } = new List<HeroStat>();
}
