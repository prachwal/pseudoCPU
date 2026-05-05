using pseudoCPU.Core;

namespace pseudoCPU.Bootstrap.Tests;

public class BootstrapCpuTests
{
    [Trait("Category", "InstructionSlice")]
    [Theory]
    [InlineData(0x00, true, false)]
    [InlineData(0x80, false, true)]
    public void LdaImmediateUpdatesZeroAndNegativeFlags(byte value, bool expectedZero, bool expectedNegative)
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, value, 0x00]);

        cpu.Step();

        Assert.Equal(value, cpu.A);
        Assert.Equal(expectedZero, cpu.Zero);
        Assert.Equal(expectedNegative, cpu.Negative);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void TaxAndInxUpdateFlagsAndStaAbsoluteStoresAccumulator()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0xFF, 0xAA, 0xE8, 0x8D, 0x34, 0x12, 0x00], 0x0600);

        cpu.Step();
        Assert.Equal(0xFF, cpu.A);
        Assert.True(cpu.Negative);
        Assert.False(cpu.Zero);

        cpu.Step();
        Assert.Equal(0xFF, cpu.X);
        Assert.True(cpu.Negative);
        Assert.False(cpu.Zero);

        cpu.Step();
        Assert.Equal(0x00, cpu.X);
        Assert.False(cpu.Negative);
        Assert.True(cpu.Zero);

        cpu.Step();
        Assert.Equal(0xFF, cpu.ReadByte(0x1234));
        Assert.False(cpu.IsHalted);

        cpu.Step();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x0608, cpu.PC);
        Assert.Equal(0xFF, cpu.ReadByte(0x1234));
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void RunStopsAtBrkAndPreservesFinalState()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x01, 0xAA, 0x8D, 0x00, 0x20, 0x00], 0x0400);

        cpu.Run();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x01, cpu.A);
        Assert.Equal(0x01, cpu.X);
        Assert.False(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.Equal(0x01, cpu.ReadByte(0x2000));
        Assert.Equal(0x0407, cpu.PC);
    }
}
