using Dota2MetaChecker.Common.Models;
using Services.Processing.Extensions.Constants;

namespace Services.Processing.Extensions;

public static class HeroCalculatorExtensions
{
    public static Hero WithWinrate(this Hero hero)
    {
        var winRate = hero.MatchCount > 0 
            ? (double)hero.WinCount / hero.MatchCount
            : 0;
            
        return hero with { WinRate = winRate };
    }
    
    public static Hero WithPickRate(this Hero hero, int totalMatchCount)
    {
        var pickRate = totalMatchCount > 0 
            ? (double)hero.MatchCount / totalMatchCount 
            : 0;
            
        return hero with { PickRate = pickRate };
    }

    public static Hero WithRating(this Hero hero)
    {
        if (hero.PickRate < HeroRatingConstants.MinPickrateForRating)
        {
            return hero with { Rating = double.MinValue };
        }
        
        // формула рейтинга
        var rating = HeroRatingConstants.WinrateImpactValue * hero.WinRate + hero.PickRate;
        
        return hero with { Rating = rating };
    }

    public static Hero WithDeltas(this Hero hero, Hero previous) =>
        hero with
        {
            WinRateDelta = hero.WinRate - previous.WinRate,
            PickRateDelta = hero.PickRate - previous.PickRate,
            RatingDelta = hero.Rating - previous.Rating
        };
}