namespace Dota2MetaChecker.Common.Constants;

public static class HeroRatingConstants
{
    /// <summary>
    /// Коэффициент влияния процента побед на рейтинг
    /// </summary>
    public const int WinrateImpactValue = 20;
    
    /// <summary>
    /// Минимальное количество матчей героя для вычисления у него рейтинга
    /// </summary>
    public const int MinMatchesForRating = 200;
}