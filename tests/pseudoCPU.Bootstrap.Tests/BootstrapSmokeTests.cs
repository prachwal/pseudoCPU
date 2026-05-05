using pseudoCPU.Core;

namespace pseudoCPU.Bootstrap.Tests;

public class BootstrapSmokeTests
{
    [Trait("Category", "Bootstrap")]
    [Fact]
    public void CoreAssemblyIsAvailable()
    {
        Assert.Equal("pseudoCPU.Core", typeof(BootstrapMarker).Assembly.GetName().Name);
    }
}
