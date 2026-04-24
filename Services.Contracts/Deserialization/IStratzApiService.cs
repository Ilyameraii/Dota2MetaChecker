namespace Services.Contracts.Deserialization;

public interface IStratzApiService
{
    public Task<string> GetHeroesStats();
    public Task<string> GetHeroesNames();
}

