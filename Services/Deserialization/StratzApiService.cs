using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Services.Contracts.Deserialization;
using STRATZ;

namespace Services.Deserialization;

/// <summary>
///     Сервис для взаимодействия с STRATZ GraphQL API
/// </summary>
public class StratzApiService : IStratzApiService
{
    private const string GraphQlEndpoint = "https://api.stratz.com/graphql";
    private readonly HttpClient httpClient;

    /// <summary>
    ///     Конструктор
    /// </summary>
    /// <param name="apiToken">API токен для аутентификации</param>
    public StratzApiService(string? apiToken = null)
    {
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "STRATZ_API");

        if (!string.IsNullOrWhiteSpace(apiToken))
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiToken);
    }

    /// <summary>
    ///     Получает статистику персонажей из STRATZ API
    /// </summary>
    public async Task<string> GetHeroesStats()
    {
        var query = QueryOfHeroesStats();

        // Оборачиваем запрос в JSON объект
        var bodyObj = new { query };
        var bodyJson = JsonSerializer.Serialize(bodyObj);
        var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(GraphQlEndpoint, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    ///     Получает список имён персонажей из STRATZ API
    /// </summary>
    public async Task<string> GetHeroesNames()
    {
        var query = QueryOfHeroesNames();

        // Оборачиваем запрос в JSON объект
        var bodyObj = new { query };
        var bodyJson = JsonSerializer.Serialize(bodyObj);
        var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(GraphQlEndpoint, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    ///     Формирует GraphQL запрос для получения имён персонажей
    /// </summary>
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

    /// <summary>
    ///     Формирует GraphQL запрос для получения статистики персонажей
    /// </summary>
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
                    week: 2941,
                    groupByPosition: true,
                    groupByBracket: true
                )
            );

        return dotaQueryQueryBuilder.Build();
    }
}