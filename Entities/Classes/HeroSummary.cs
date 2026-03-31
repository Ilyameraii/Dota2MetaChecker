namespace Entities.Classes;

public class HeroSummary
{
    public int HeroId { get; set; }
    public string? Name { get; set; }

    public int WinCount { get; set; }
    public int MatchCount { get; set; }
    public int Rating => WinCount * 2 - MatchCount;
}