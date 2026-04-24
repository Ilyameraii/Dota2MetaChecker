using Entities.Classes;
using Services.Contracts.Models;

namespace Services.Contracts.Processing;

public interface IHeroStatsProcessor
{
    /// <summary>
    /// Выполняет полный пайплайн: фильтрация → агрегация → сортировка
    /// </summary>
    public List<Hero> GetProcessedHeroStats(
        IReadOnlyList<HeroStat> sourceStats,
        IReadOnlyDictionary<int, string> heroNames,
        HeroProcessingOptions processingOptions);
}