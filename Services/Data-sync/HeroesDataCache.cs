using Entities.Classes;
using Entities.Models;

namespace Services.Data_sync;

public class HeroesDataCache
{
    public List<HeroStat>? HeroesStats { get; set; }
    public Dictionary<int, string>? HeroesNames { get; set; }
    public DateTime? UpdateTime { get; set; }

    public bool IsLoaded => HeroesStats != null && HeroesNames != null;
}