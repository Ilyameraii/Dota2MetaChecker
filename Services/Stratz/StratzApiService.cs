using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Dota2Models.Stratz;
using Services.Contracts.Stratz;

namespace Services.Stratz;

public class StratzApiService: IStratzApiService
{
    private const string GraphQlEndpoint = "https://api.stratz.com/graphql";
    private readonly HttpClient httpClient;

    public StratzApiService(string? apiToken = null)
    {
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "STRATZ_API");

        if (!string.IsNullOrWhiteSpace(apiToken))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiToken);
        }
    }

    private string QueryOfHeroesNames()
    {
        var dotaQueryQueryBuilder = new DotaQueryQueryBuilder()
            .WithConstants(new ConstantQueryQueryBuilder()
                .WithHeroes(new HeroTypeQueryBuilder()
                    .WithId()
                    .WithDisplayName()
                )
            );
        return dotaQueryQueryBuilder.Build();
    }
    
    private string QueryOfHeroesStats()
    {
        var dotaQueryQueryBuilder = new DotaQueryQueryBuilder()
            .WithHeroStats(new HeroStatsQueryQueryBuilder()
                .WithStats(
                    new HeroPositionTimeDetailTypeQueryBuilder()
                        .WithHeroId()
                        .WithBracketBasicIds()
                        .WithPosition()
                        .WithWinCount()
                        .WithMatchCount(),
                    bracketBasicIds: new RankBracketBasicEnum?[]
                    {
                        RankBracketBasicEnum.Uncalibrated,
                        RankBracketBasicEnum.HeraldGuardian,
                        RankBracketBasicEnum.CrusaderArchon,
                        RankBracketBasicEnum.LegendAncient,
                        RankBracketBasicEnum.DivineImmortal
                    },
                    positionIds: new MatchPlayerPositionType?[]
                    {
                        MatchPlayerPositionType.Position1,
                        MatchPlayerPositionType.Position2,
                        MatchPlayerPositionType.Position3,
                        MatchPlayerPositionType.Position4,
                        MatchPlayerPositionType.Position5
                    },
                    groupByPosition: true,
                    groupByBracket: true
                )
            );

        return dotaQueryQueryBuilder.Build();
    }

    public async Task<string> GetHeroesStats()
    {
        string query = QueryOfHeroesStats();

        // Оборачиваем запрос в JSON объект
        var bodyObj = new { query };
        var bodyJson = JsonSerializer.Serialize(bodyObj);
        var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(GraphQlEndpoint, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> GetHeroesNames()
    {
        string query = QueryOfHeroesNames();

        // Оборачиваем запрос в JSON объект
        var bodyObj = new { query };
        var bodyJson = JsonSerializer.Serialize(bodyObj);
        var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(GraphQlEndpoint, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}

