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
    public void StepAfterHaltDoesNotAdvanceState()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x00, 0xA9, 0xFF], 0x0800);
        cpu.Step();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x0801, cpu.PC);

        cpu.Step();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x0801, cpu.PC);
        Assert.Equal(0x00, cpu.A);
        Assert.Equal(0x00, cpu.X);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void RunThrowsWhenProgramDoesNotHaltWithinStepLimit()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x01]);

        var exception = Assert.Throws<InvalidOperationException>(() => cpu.Run(1));

        Assert.Contains("did not halt", exception.Message);
        Assert.False(cpu.IsHalted);
        Assert.Equal(0x0002, cpu.PC);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void RunStepsExecutesFixedNumberOfInstructionsWithoutRequiringHalt()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x01, 0xAA, 0x00], 0x0400);

        cpu.RunSteps(2);

        Assert.False(cpu.IsHalted);
        Assert.Equal(0x01, cpu.A);
        Assert.Equal(0x01, cpu.X);
        Assert.Equal(0x0403, cpu.PC);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void LoadProgramRejectsProgramThatDoesNotFitInMemory()
    {
        var cpu = new BootstrapCpu();

        var program = new byte[] { 0xA9, 0x01 };

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => cpu.LoadProgram(program, 0xFFFF));

        Assert.Contains("does not fit in memory", exception.Message);
    }

    [Trait("Category", "InstructionSlice")]
    [Theory]
    [InlineData(0x0000)]
    [InlineData(0xFFFF)]
    public void StaAbsoluteSupportsBoundaryAddresses(ushort address)
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x7E, 0x8D, (byte)(address & 0xFF), (byte)(address >> 8), 0x00]);

        cpu.Run();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x7E, cpu.ReadByte(address));
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void UnsupportedOpcodeThrowsAControlledError()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x02]);

        var exception = Assert.Throws<NotSupportedException>(() => cpu.Step());

        Assert.Contains("Unsupported bootstrap opcode", exception.Message);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void RunStopsAtBrkAndPreservesFinalState()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x01, 0xAA, 0x8D, 0x00, 0x20, 0x00], 0x0400);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x01, cpu.A);
        Assert.Equal(0x01, cpu.X);
        Assert.False(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.Equal(0x01, cpu.ReadByte(0x2000));
        Assert.Equal(0x0407, cpu.PC);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void JmpCmpBeqAndBneWorkTogetherForControlFlow()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x03, 0xC9, 0x03, 0xF0, 0x02, 0xA9, 0xFF, 0x00], 0x0600);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x03, cpu.A);
        Assert.True(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.Equal(0x0609, cpu.PC);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void BneBranchesWhenZeroFlagIsClear()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x01, 0xC9, 0x02, 0xD0, 0x02, 0xA9, 0xFF, 0x00], 0x0700);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x01, cpu.A);
        Assert.False(cpu.Zero);
        Assert.True(cpu.Negative);
        Assert.Equal(0x0709, cpu.PC);
    }

}
