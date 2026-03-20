using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services.Contracts.Stratz;

public interface IStratzApiService
{
    public Task<string> GetHeroesStats();
    public Task<string> GetHeroesNames();
}

