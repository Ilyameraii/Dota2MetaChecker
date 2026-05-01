using Dota2MetaChecker.Common.Models;
using Entities.Models;

namespace Services.Contracts.Processing;

/// <summary>
///     Процессор для обработки статистики персонажей
/// </summary>
public interface IHeroStatsProcessor
{
    /// <summary>
    ///     Выполняет полный пайплайн: фильтрация → агрегация → сортировка
    /// </summary>
    public List<Hero> GetProcessedHeroStats(
        IReadOnlyList<HeroStat> sourceStats,
        IReadOnlyList<HeroStat> oldSourceStats,
        IReadOnlyDictionary<int, string> heroNames,
        HeroProcessingOptions processingOptions);
}