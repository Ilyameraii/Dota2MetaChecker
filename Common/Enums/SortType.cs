namespace Dota2MetaChecker.Common.Enums;

/// <summary>
///     Тип сортировки героев
/// </summary>
public enum SortType
{
    /// <summary>
    ///     По винрейту
    /// </summary>
    WinRate,

    /// <summary>
    ///     По
    /// </summary>
    WinrateDelta,

    /// <summary>
    ///     По количеству матчей
    /// </summary>
    MatchCount,

    /// <summary>
    ///     По
    /// </summary>
    PickrateDelta,


    /// <summary>
    ///     По рейтингу
    /// </summary>
    Rating,

    /// <summary>
    ///     По
    /// </summary>
    RatingDelta,
}