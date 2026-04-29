using Moq;
using Services.Contracts.Data_sync;
using Services.Data_sync;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Services.Tests.Data_sync;

/// <summary>
/// Тесты для HeroDataUpdateHostedService.
/// </summary>
public class HeroDataUpdateHostedServiceTests
{
    private readonly Mock<IHeroesDataService> mockDataService;
    private readonly HeroDataUpdateHostedService service;

    /// <summary>
    /// Инициализирует тесты.
    /// </summary>
    public HeroDataUpdateHostedServiceTests()
    {
        mockDataService = new Mock<IHeroesDataService>();
        service = new HeroDataUpdateHostedService(mockDataService.Object);
    }

    /// <summary>
    /// Проверяет, что при сбое метод повторяет попытки 3 раза.
    /// </summary>
    [Fact]
    public async Task RetryLogic_ShouldRetryThreeTimes_OnFailure()
    {
        mockDataService.Setup(s => s.UpdateDataAsync()).ThrowsAsync(new Exception("Test error"));

        await service.ExecuteUpdateWithRetryAsync(CancellationToken.None);

        mockDataService.Verify(s => s.UpdateDataAsync(), Times.Exactly(3));
    }
}
