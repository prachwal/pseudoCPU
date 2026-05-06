using pseudoCPU.Core;

namespace pseudoCPU.Bootstrap.Tests;

public class BootstrapMemoryBusTests
{
    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void LoadProgramClearsRamBeforeCopyingProgram()
    {
        var bus = new BootstrapMemoryBus();

        bus.WriteByte(0x1234, 0xAA);
        bus.LoadProgram([0x11, 0x22], 0x0600);

        Assert.Equal(0x11, bus.ReadByte(0x0600));
        Assert.Equal(0x22, bus.ReadByte(0x0601));
        Assert.Equal(0x00, bus.ReadByte(0x1234));
    }

    [Trait("Category", "InstructionSlice")]
    [Theory]
    [InlineData(0x00, 0x00, 0x0000)]
    [InlineData(0x10, 0x00, 0x0010)]
    [InlineData(0xFF, 0x01, 0x0000)]
    [InlineData(0x80, 0x7F, 0x00FF)]
    public void ZeroPageAddressResolutionKeepsAddressesInsidePage(byte zeroPageAddress, byte index, ushort expectedAddress)
    {
        var bus = new BootstrapMemoryBus();

        Assert.Equal((ushort)zeroPageAddress, bus.ResolveZeroPageAddress(zeroPageAddress));
        Assert.Equal(expectedAddress, bus.ResolveZeroPageIndexedAddress(zeroPageAddress, index));
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void CpuUsesRamOnlyMemoryBusFoundationForDirectAccess()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x00], 0x0600);
        cpu.MemoryBus.WriteByte(0x0042, 0xCC);

        Assert.Equal(0xCC, cpu.ReadByte(0x0042));
        cpu.WriteByte(0x0043, 0xDD);
        Assert.Equal(0xDD, cpu.MemoryBus.ReadByte(0x0043));
    }
}
