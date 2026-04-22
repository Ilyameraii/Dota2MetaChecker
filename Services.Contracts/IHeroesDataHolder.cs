using Entities.Classes;

namespace Services.Contracts.Stratz;

public interface IHeroesDataHolder
{
    public IReadOnlyList<HeroStat> HeroesStats { get; }
    
    public IReadOnlyDictionary<int, string> HeroesNames { get; }
    
    public Task UpdateDataAsync();

    public  Task SaveDataAsync();

    public  Task LoadLastDataAsync();
}