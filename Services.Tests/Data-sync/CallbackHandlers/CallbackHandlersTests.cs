using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using FluentAssertions;
using Services.Data_sync.CallbackHandlers;

namespace Services.Tests.Data_sync.CallbackHandlers;

public class CallbackHandlersTests
{
    [Fact]
    public void RoleCallbackHandler_CanHandle_ReturnsTrueForRolePrefix()
    {
        new RoleCallbackHandler().CanHandle(CallbackPrefixes.Role + "Safelane").Should().BeTrue();
    }

    [Fact]
    public void RoleCallbackHandler_CanHandle_ReturnsFalseForOtherPrefix()
    {
        new RoleCallbackHandler().CanHandle(CallbackPrefixes.Rank + "HeraldGuardian").Should().BeFalse();
    }

    [Fact]
    public void RoleCallbackHandler_Handle_TogglesRoleFlag()
    {
        var prefs = new UserPreferences();
        var handler = new RoleCallbackHandler();
        handler.Handle(prefs, CallbackPrefixes.Role + "Safelane");
        prefs.ProcessingOptions.Roles.Should().HaveFlag(RoleFlags.Safelane);

        handler.Handle(prefs, CallbackPrefixes.Role + "Safelane");
        prefs.ProcessingOptions.Roles.Should().NotHaveFlag(RoleFlags.Safelane);
    }

    [Fact]
    public void RankCallbackHandler_CanHandle_ReturnsTrueForRankPrefix()
    {
        new RankCallbackHandler().CanHandle(CallbackPrefixes.Rank + "HeraldGuardian").Should().BeTrue();
    }

    [Fact]
    public void RankCallbackHandler_Handle_TogglesRankFlag()
    {
        var prefs = new UserPreferences();
        var handler = new RankCallbackHandler();
        handler.Handle(prefs, CallbackPrefixes.Rank + "DivineImmortal");
        prefs.ProcessingOptions.Ranks.Should().HaveFlag(RankFlags.DivineImmortal);

        handler.Handle(prefs, CallbackPrefixes.Rank + "DivineImmortal");
        prefs.ProcessingOptions.Ranks.Should().NotHaveFlag(RankFlags.DivineImmortal);
    }

    [Fact]
    public void SortCallbackHandler_CanHandle_ReturnsTrueForSortPrefix()
    {
        new SortCallbackHandler().CanHandle(CallbackPrefixes.Sort + "WinRate").Should().BeTrue();
    }

    [Fact]
    public void SortCallbackHandler_Handle_SetsSortTypeAndDescending()
    {
        var prefs = new UserPreferences();
        var handler = new SortCallbackHandler();
        handler.Handle(prefs, CallbackPrefixes.Sort + SortType.WinRate);
        prefs.ProcessingOptions.SortBy.Should().Be(SortType.WinRate);
        prefs.ProcessingOptions.IsDescending.Should().BeTrue();
        prefs.PageNumber.Should().Be(0);
    }

    [Fact]
    public void SortCallbackHandler_Handle_TogglesDescending_WhenSameSortType()
    {
        var prefs = new UserPreferences();
        prefs.ProcessingOptions.SortBy = SortType.WinRate;
        prefs.ProcessingOptions.IsDescending = true;

        var handler = new SortCallbackHandler();
        handler.Handle(prefs, CallbackPrefixes.Sort + SortType.WinRate);
        prefs.ProcessingOptions.IsDescending.Should().BeFalse();
    }

    [Fact]
    public void PageCallbackHandler_CanHandle_ReturnsTrueForPagePrefix()
    {
        new PageCallbackHandler().CanHandle(CallbackPrefixes.Page + PageDirection.Next).Should().BeTrue();
    }

    [Fact]
    public void PageCallbackHandler_Handle_IncrementsPage_ForNext()
    {
        var prefs = new UserPreferences();
        var handler = new PageCallbackHandler();
        handler.Handle(prefs, CallbackPrefixes.Page + PageDirection.Next);
        prefs.PageNumber.Should().Be(1);
    }

    [Fact]
    public void PageCallbackHandler_Handle_DecrementsPage_ForPrevious()
    {
        var prefs = new UserPreferences();
        prefs.PageNumber = 2;
        var handler = new PageCallbackHandler();
        handler.Handle(prefs, CallbackPrefixes.Page + PageDirection.Previous);
        prefs.PageNumber.Should().Be(1);
    }

    [Fact]
    public void PageCallbackHandler_Handle_DoesNotGoBelowZero()
    {
        var prefs = new UserPreferences();
        var handler = new PageCallbackHandler();
        handler.Handle(prefs, CallbackPrefixes.Page + PageDirection.Previous);
        prefs.PageNumber.Should().Be(0);
    }

    [Fact]
    public void ClearOptionsCallbackHandler_CanHandle_ReturnsTrueForClearOptions()
    {
        new ClearOptionsCallbackHandler().CanHandle(CallbackConstants.ClearOptions).Should().BeTrue();
    }

    [Fact]
    public void ClearOptionsCallbackHandler_Handle_ResetsPreferences()
    {
        var prefs = new UserPreferences();
        prefs.ProcessingOptions.Roles = RoleFlags.Safelane;
        prefs.PageNumber = 5;
        var handler = new ClearOptionsCallbackHandler();
        handler.Handle(prefs, CallbackConstants.ClearOptions);
        prefs.ProcessingOptions.Roles.Should().Be(RoleFlags.None);
        prefs.PageNumber.Should().Be(0);
    }

    [Fact]
    public void SwitchFormatCallbackHandler_CanHandle_ReturnsTrueForSwitchFormat()
    {
        new SwitchFormatCallbackHandler().CanHandle(CallbackConstants.SwitchFormat).Should().BeTrue();
    }

    [Fact]
    public void SwitchFormatCallbackHandler_Handle_SwitchesFormat()
    {
        var prefs = new UserPreferences();
        var initialFormat = prefs.IsImageFormat;
        var handler = new SwitchFormatCallbackHandler();
        handler.Handle(prefs, CallbackConstants.SwitchFormat);
        prefs.IsImageFormat.Should().NotBe(initialFormat);
    }
}
