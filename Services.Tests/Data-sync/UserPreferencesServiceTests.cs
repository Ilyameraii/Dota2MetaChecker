using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using FluentAssertions;
using Moq;
using Services.Contracts.Data_sync;
using Services.Data_sync;
using Xunit;

namespace Services.Tests.Data_sync;

public class UserPreferencesServiceTests
{
    private readonly Mock<ICallbackHandler> _handlerMock = new();
    private readonly UserPreferencesService _service;

    public UserPreferencesServiceTests()
    {
        _service = new UserPreferencesService(new[] { _handlerMock.Object });
    }

    [Fact]
    public void GetOrCreate_CreatesNew_WhenFirstAccess()
    {
        var result = _service.GetOrCreate(123);
        result.Should().NotBeNull();
        result.ProcessingOptions.Ranks.Should().Be(RankFlags.None);
        result.ProcessingOptions.Roles.Should().Be(RoleFlags.None);
    }

    [Fact]
    public void GetOrCreate_ReturnsSame_WhenCalledAgain()
    {
        var result1 = _service.GetOrCreate(123);
        var result2 = _service.GetOrCreate(123);
        result2.Should().BeSameAs(result1);
    }

    [Fact]
    public void Apply_CallsCorrectHandler()
    {
        _handlerMock.Setup(h => h.CanHandle("test_data")).Returns(true);
        var prefs = _service.GetOrCreate(123);
        _service.Apply(123, "test_data");
        _handlerMock.Verify(h => h.Handle(prefs, "test_data"), Times.Once);
    }

    [Fact]
    public void Apply_NoHandler_CallsNothing()
    {
        _handlerMock.Setup(h => h.CanHandle(It.IsAny<string>())).Returns(false);
        _service.Apply(123, "unknown");
        _handlerMock.Verify(h => h.Handle(It.IsAny<UserPreferences>(), It.IsAny<string>()), Times.Never);
    }
}
