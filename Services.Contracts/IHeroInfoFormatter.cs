using Entities.Classes;

namespace Services.Contracts.Stratz;

public interface IHeroInfoFormatter
{
    string Format(Hero hero, int totalMatches);
}