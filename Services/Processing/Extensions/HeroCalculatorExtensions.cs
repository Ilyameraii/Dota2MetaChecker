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
        var penalty = 1;
        var rating = HeroRatingConstants.WinrateImpactValue * hero.WinRate + hero.PickRate;
        
        if (hero.PickRate < HeroRatingConstants.MinPickrateForRating)
        {
            // формула рейтинга
            rating -= penalty;
        }
        return hero with { Rating = rating };
    }

    public static Hero WithDeltas(this Hero hero, Hero previous) =>
        hero with
        {
            WinRateDelta = CalculateWinrateDelta(hero, previous),
            PickRateDelta = CalculatePickrateDelta(hero, previous),
            RatingDelta = CalculateRatingDelta(hero, previous)
        };

    private static double CalculateWinrateDelta(Hero hero, Hero previous) => hero.WinRate - previous.WinRate;

    private static double CalculatePickrateDelta(Hero hero, Hero previous) => hero.PickRate - previous.PickRate;

    private static double CalculateRatingDelta(Hero hero, Hero previous)
    {
        const int penalty = 1;
        var heroBelowMin = hero.PickRate < HeroRatingConstants.MinPickrateForRating;
        var previousBelowMin = previous.PickRate < HeroRatingConstants.MinPickrateForRating;
        var delta = hero.Rating - previous.Rating;

        return (heroBelowMin, previousBelowMin) switch
        {
            (true, false) => delta + penalty,
            (_, true)     => delta - penalty,
            _             => delta
        };
    }
}