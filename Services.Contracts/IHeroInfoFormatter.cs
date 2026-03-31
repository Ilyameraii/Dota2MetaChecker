using Entities.Classes;

namespace Services.Contracts;

public interface IHeroInfoFormatter
{
    string Format(Hero hero, int totalMatches);
}