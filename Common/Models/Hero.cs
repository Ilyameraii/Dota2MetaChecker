namespace Dota2MetaChecker.Common.Models;

/// <summary>
///     Модель персонажа с агрегированной статистикой
/// </summary>
public record Hero
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public int WinCount { get; init; }
    public int MatchCount { get; init; }
    public double WinRate { get; init; }
    public double PickRate { get; init; }
    public double Rating { get; init; }
    public double PickRateDelta { get; init; }
    public double WinRateDelta { get; init; }
    public double RatingDelta { get; init; }
}