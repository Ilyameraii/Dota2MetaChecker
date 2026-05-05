using Dota2MetaChecker.Common.Enums;
using FluentAssertions;
using Services.Extensions;
using Xunit;

namespace Services.Tests;

public class ExtensionsTests
{
    [Fact]
    public void FlagExtensions_ToRankFlag_ReturnsCorrectFlag()
    {
        Rank.Uncalibrated.ToFlag().Should().Be(RankFlags.Uncalibrated);
        Rank.HeraldGuardian.ToFlag().Should().Be(RankFlags.HeraldGuardian);
        Rank.CrusaderArchon.ToFlag().Should().Be(RankFlags.CrusaderArchon);
        Rank.LegendAncient.ToFlag().Should().Be(RankFlags.LegendAncient);
        Rank.DivineImmortal.ToFlag().Should().Be(RankFlags.DivineImmortal);
        ((Rank)999).ToFlag().Should().Be(RankFlags.None);
    }

    [Fact]
    public void FlagExtensions_ToRoleFlag_ReturnsCorrectFlag()
    {
        Role.Safelane.ToFlag().Should().Be(RoleFlags.Safelane);
        Role.Midlane.ToFlag().Should().Be(RoleFlags.Midlane);
        Role.Offlane.ToFlag().Should().Be(RoleFlags.Offlane);
        Role.Support.ToFlag().Should().Be(RoleFlags.Support);
        Role.HardSupport.ToFlag().Should().Be(RoleFlags.HardSupport);
        ((Role)999).ToFlag().Should().Be(RoleFlags.None);
    }

    [Theory]
    [InlineData(Rank.Uncalibrated, RankFlags.Uncalibrated, true)]
    [InlineData(Rank.HeraldGuardian, RankFlags.HeraldGuardian, true)]
    [InlineData(Rank.CrusaderArchon, RankFlags.CrusaderArchon, true)]
    [InlineData(Rank.LegendAncient, RankFlags.LegendAncient, true)]
    [InlineData(Rank.DivineImmortal, RankFlags.DivineImmortal, true)]
    [InlineData(Rank.Uncalibrated, RankFlags.DivineImmortal, false)]
    [InlineData(Rank.HeraldGuardian, RankFlags.None, false)]
    public void RankExtensions_IsIncludedIn_ReturnsExpected(Rank rank, RankFlags flags, bool expected)
    {
        rank.IsIncludedIn(flags).Should().Be(expected);
    }

    [Theory]
    [InlineData(Role.Safelane, RoleFlags.Safelane, true)]
    [InlineData(Role.Midlane, RoleFlags.Midlane, true)]
    [InlineData(Role.Offlane, RoleFlags.Offlane, true)]
    [InlineData(Role.Support, RoleFlags.Support, true)]
    [InlineData(Role.HardSupport, RoleFlags.HardSupport, true)]
    [InlineData(Role.Safelane, RoleFlags.Offlane, false)]
    [InlineData(Role.Midlane, RoleFlags.None, false)]
    public void RoleExtensions_IsIncludedIn_ReturnsExpected(Role role, RoleFlags flags, bool expected)
    {
        role.IsIncludedIn(flags).Should().Be(expected);
    }
}
