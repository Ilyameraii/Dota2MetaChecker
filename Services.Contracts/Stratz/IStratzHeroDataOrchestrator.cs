using Entities.Classes;

namespace Services.Contracts.Stratz;

public interface IStratzHeroDataOrchestrator
{
    public Task<List<Hero>> GetHeroesAsync();
}