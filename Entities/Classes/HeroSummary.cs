namespace Entities.Classes;

public class HeroSummary
{
    public int HeroId { get; set; }
    public string Name { get; set; } = string.Empty;

    public int WinCount { get; set; }
    public int MatchCount { get; set; }
}