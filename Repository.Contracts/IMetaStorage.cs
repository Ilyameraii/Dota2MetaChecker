using Entities.Classes;

namespace Repository.Contracts;

public interface IMetaStorage
{
    Task SaveDataAsync(IReadOnlyList<HeroStat> heroStats, DateTime dateTime);
    
    Task<IReadOnlyList<HeroStat>> GetHeroStatsByMetaUpdateIdAsync(int metaUpdateId);

    Task<(IReadOnlyList<HeroStat> heroesStats, DateTime? dateTime)> GetLastMetaUpdateAsync();
}