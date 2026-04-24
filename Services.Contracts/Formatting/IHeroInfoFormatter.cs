using Entities.Classes;
using Entities.Models;

namespace Services.Contracts.Formatting;

public interface IHeroInfoFormatter
{
    string Format(Hero hero);
    string Format(Hero hero, int totalMatchCount);
}