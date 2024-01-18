using FluentAssertions;

namespace AzureArcDemo.Test;

public class BastaControllerTests
{
    [Fact]
    public void TestShouldAlwaysSucceed()
    {
        true.Should().BeTrue();
    }
}