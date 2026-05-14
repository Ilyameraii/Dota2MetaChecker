using FluentAssertions;
using Services.ImageProviders;
using Xunit;

namespace Services.Tests;

public class ImageProvidersTests
{
    [Fact]
    public void ImageResourceProvider_ListAll_ReturnsNonEmpty()
    {
        var resources = ImageResourceProvider.ListAll();
        resources.Should().NotBeEmpty();
    }

    [Fact]
    public void ImageResourceProvider_GetRankIcon_ReturnsNullForInvalid()
    {
        var result = ImageResourceProvider.GetRankIcon("invalid_rank_name");
        result.Should().BeNull();
    }

    [Fact]
    public void ImageResourceProvider_GetRoleIcon_ReturnsNullForInvalid()
    {
        var result = ImageResourceProvider.GetRoleIcon("invalid_role_name");
        result.Should().BeNull();
    }
}