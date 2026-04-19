using Entities.Classes;

namespace Repository.Contracts;

public interface IMetaStorage
{
    Task SaveDataAsync(List<HeroStat>? heroStats, DateTime dateTime);
}