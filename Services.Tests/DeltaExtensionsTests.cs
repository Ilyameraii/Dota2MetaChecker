using FluentAssertions;
using Services.Formatting.Extensions;

namespace Services.Tests;

public class DeltaExtensionsTests
{
    [Theory]
    [InlineData(0.1234, "+12.34")]
    [InlineData(-0.5678, "-56.78")]
    [InlineData(0.0, "+0.00")]
    [InlineData(1.0, "+100.00")]
    [InlineData(-0.001, "-0.10")]
    [InlineData(0.005, "+0.50")]
    public void FormatDelta_ReturnsCorrectString(double delta, string expected)
    {
        delta.FormatDelta().Should().Be(expected);
    }
}
