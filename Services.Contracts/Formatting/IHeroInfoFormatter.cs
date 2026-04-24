using Entities.Classes;

namespace Services.Contracts.Formatting;

public interface IHeroInfoFormatter
{
    string Format(Hero hero);
    string Format(Hero hero, int totalMatchCount);
}