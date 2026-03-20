using Entities.Classes;
using Services.Contracts.Stratz;

namespace Services.Stratz;

public class StratzHeroDataOrchestrator(StratzApiService api, StratzHeroParser heroParser):IStratzHeroDataOrchestrator
{
    public async Task<List<Hero>> GetHeroesAsync()
    {
        // Порядок гарантирован — имена всегда первые
        var namesJson = await api.GetHeroesNames();
        var names     = heroParser.ParseHeroesNames(namesJson);

        var statsJson = await api.GetHeroesStats();
        var heroes    = heroParser.ParseHeroStats(statsJson, names);

        return heroes;
    }
}