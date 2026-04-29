using Moq;
using Services.Contracts.Data_sync;
using Services.Data_sync;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Services.Tests.Data_sync;

public class HeroDataUpdateHostedServiceTests
{
    private readonly Mock<IHeroesDataService> _mockDataService;
    private readonly HeroDataUpdateHostedService _service;

    public HeroDataUpdateHostedServiceTests()
    {
        _mockDataService = new Mock<IHeroesDataService>();
        _service = new HeroDataUpdateHostedService(_mockDataService.Object);
    }

    [Fact]
    public async Task RetryLogic_ShouldRetryThreeTimes_OnFailure()
    {
        _mockDataService.Setup(s => s.UpdateDataAsync()).ThrowsAsync(new Exception("Test error"));
        _mockDataService.Setup(s => s.SaveDataAsync()).ThrowsAsync(new Exception("Test error"));

        await _service.ExecuteUpdateWithRetryAsync(CancellationToken.None);

        _mockDataService.Verify(s => s.UpdateDataAsync(), Times.Exactly(3));

    }
}