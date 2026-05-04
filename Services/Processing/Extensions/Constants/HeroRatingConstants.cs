namespace Services.Processing.Extensions.Constants;

public static class HeroRatingConstants
{
    /// <summary>
    /// Коэффициент влияния процента побед на рейтинг
    /// </summary>
    public const int WinrateImpactValue = 20;
    
    /// <summary>
    /// Минимальное количество матчей героя для вычисления у него рейтинга
    /// </summary>
    public const double MinPickrateForRating = 0.002;
}