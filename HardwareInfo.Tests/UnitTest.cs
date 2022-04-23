using HardwareInfo.Info;
using Xunit;
using Xunit.Abstractions;

namespace HardwareInfo.Tests;

public class UnitTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void HardwareInfo()
    {
        IHardwareInfo hardwareInfo;
#if Linux
        hardwareInfo = new HardwareInfoLinux();
#elif Windows
        hardwareInfo = new HardwareInfoWindows();
#endif
        _testOutputHelper.WriteLine(hardwareInfo.HwInfo.ToString());

        Assert.NotEmpty(hardwareInfo.HwInfo);
    }
}