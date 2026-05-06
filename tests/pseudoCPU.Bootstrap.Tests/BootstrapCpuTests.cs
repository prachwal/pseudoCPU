using pseudoCPU.Core;

namespace pseudoCPU.Bootstrap.Tests;

public class BootstrapCpuTests
{
    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void LoadProgramInitializesStackPointerToFf()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x00], 0x0600);

        Assert.Equal(0xFF, cpu.SP);
        Assert.Equal(0x0600, cpu.PC);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void StackPushAndPopUsePage0100AndWrapAround()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x00]);

        cpu.PushByte(0xAA);
        Assert.Equal(0xFE, cpu.SP);
        Assert.Equal(0xAA, cpu.ReadByte(0x01FF));

        cpu.PushByte(0xBB);
        Assert.Equal(0xFD, cpu.SP);
        Assert.Equal(0xBB, cpu.ReadByte(0x01FE));

        Assert.Equal(0xBB, cpu.PopByte());
        Assert.Equal(0xFE, cpu.SP);
        Assert.Equal(0xAA, cpu.PopByte());
        Assert.Equal(0xFF, cpu.SP);

        for (var value = 0; value < 256; value++)
        {
            cpu.PushByte((byte)value);
        }

        Assert.Equal(0xFF, cpu.SP);
        Assert.Equal(0xFF, cpu.ReadByte(0x0100));
        Assert.Equal(0x00, cpu.ReadByte(0x01FF));

        cpu.PushByte(0xCC);

        Assert.Equal(0xFE, cpu.SP);
        Assert.Equal(0xCC, cpu.ReadByte(0x01FF));
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void PhpPushesSupportedFlagsIntoBootstrapStatusSnapshot()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x00, 0xC9, 0x00, 0x08, 0x00], 0x0600);

        cpu.RunSteps(3);

        Assert.Equal(0xFE, cpu.SP);
        Assert.Equal(0x03, cpu.ReadByte(0x01FF));
        Assert.True(cpu.Carry);
        Assert.True(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void PlpRestoresOnlySupportedFlagsFromBootstrapStatusSnapshot()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x28, 0x00], 0x0600);
        cpu.PushByte(0xFF);

        cpu.Step();

        Assert.Equal(0xFF, cpu.SP);
        Assert.True(cpu.Carry);
        Assert.True(cpu.Zero);
        Assert.True(cpu.Negative);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void PhaPushesAccumulatorWithoutChangingFlags()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x03, 0xC9, 0x01, 0x48, 0x00], 0x0600);

        cpu.Step();
        cpu.Step();

        Assert.Equal(0x03, cpu.A);
        Assert.False(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.True(cpu.Carry);

        cpu.Step();

        Assert.Equal(0x03, cpu.A);
        Assert.Equal(0xFE, cpu.SP);
        Assert.Equal(0x03, cpu.ReadByte(0x01FF));
        Assert.False(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.True(cpu.Carry);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "InstructionSlice")]
    [Theory]
    [InlineData(0x00, true, false)]
    [InlineData(0x80, false, true)]
    public void PlaPopsAccumulatorAndUpdatesZeroAndNegativeFlags(byte value, bool expectedZero, bool expectedNegative)
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x68, 0x00], 0x0600);
        cpu.PushByte(value);

        cpu.Step();

        Assert.Equal(value, cpu.A);
        Assert.Equal(expectedZero, cpu.Zero);
        Assert.Equal(expectedNegative, cpu.Negative);
        Assert.Equal(0xFF, cpu.SP);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "StackOpcode")]
    [Fact]
    public void PhaAndPlaPreserveLifoOrderingAndRestoreStackPointer()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x11, 0x48, 0xA9, 0x22, 0x48, 0x68, 0xAA, 0x68, 0x00], 0x0600);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x11, cpu.A);
        Assert.Equal(0x22, cpu.X);
        Assert.Equal(0xFF, cpu.SP);
        Assert.Equal(0x060A, cpu.PC);
    }

    [Trait("Category", "StackOpcode")]
    [Fact]
    public void PhpAndPlpRoundTripSupportedStatusBitsAndIgnoreReservedBits()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x28, 0x08, 0x00], 0x0600);
        cpu.PushByte(0xFF);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.True(cpu.Carry);
        Assert.True(cpu.Zero);
        Assert.True(cpu.Negative);
        Assert.Equal(0xFE, cpu.SP);
        Assert.Equal(0x83, cpu.ReadByte(0x01FF));
        Assert.Equal(0x0603, cpu.PC);
    }

    [Trait("Category", "StackOpcode")]
    [Fact]
    public void MixedStackOpcodeSequenceRestoresStatusAndAccumulatorInOrder()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x2A, 0x48, 0xA9, 0x00, 0xC9, 0x00, 0x08, 0xA9, 0x00, 0xC9, 0x01, 0x28, 0x68, 0x00], 0x0600);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x2A, cpu.A);
        Assert.True(cpu.Carry);
        Assert.False(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.Equal(0xFF, cpu.SP);
        Assert.Equal(0x060F, cpu.PC);
    }

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
        Assert.False(cpu.Carry);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "InstructionSlice")]
    [Theory]
    [InlineData(0x00, true, false)]
    [InlineData(0x80, false, true)]
    public void LdxImmediateUpdatesZeroAndNegativeFlags(byte value, bool expectedZero, bool expectedNegative)
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA2, value, 0x00]);

        cpu.Step();

        Assert.Equal(value, cpu.X);
        Assert.Equal(expectedZero, cpu.Zero);
        Assert.Equal(expectedNegative, cpu.Negative);
        Assert.False(cpu.Carry);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "InstructionSlice")]
    [Theory]
    [InlineData(0x03, 0x03, true, false, true)]
    [InlineData(0x04, 0x03, false, false, true)]
    [InlineData(0x02, 0x03, false, true, false)]
    public void CmpImmediateUpdatesZeroNegativeAndCarryFlags(byte accumulator, byte operand, bool expectedZero, bool expectedNegative, bool expectedCarry)
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, accumulator, 0xC9, operand, 0x00]);

        cpu.Step();
        cpu.Step();

        Assert.Equal(accumulator, cpu.A);
        Assert.Equal(expectedZero, cpu.Zero);
        Assert.Equal(expectedNegative, cpu.Negative);
        Assert.Equal(expectedCarry, cpu.Carry);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "InstructionSlice")]
    [Theory]
    [InlineData(0x03, 0x03, true, false, true)]
    [InlineData(0x04, 0x03, false, false, true)]
    [InlineData(0x02, 0x03, false, true, false)]
    public void CpxImmediateUpdatesZeroNegativeAndCarryFlags(byte index, byte operand, bool expectedZero, bool expectedNegative, bool expectedCarry)
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA2, index, 0xE0, operand, 0x00]);

        cpu.Step();
        cpu.Step();

        Assert.Equal(index, cpu.X);
        Assert.Equal(expectedZero, cpu.Zero);
        Assert.Equal(expectedNegative, cpu.Negative);
        Assert.Equal(expectedCarry, cpu.Carry);
        Assert.False(cpu.IsHalted);
    }

    [Trait("Category", "InstructionSlice")]
    [Fact]
    public void LoadProgramResetsCarryFlag()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x03, 0xC9, 0x01, 0x00]);
        cpu.RunSteps(2);

        Assert.True(cpu.Carry);

        cpu.LoadProgram([0x00]);

        Assert.False(cpu.Carry);
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
    public void DexWrapsAndStxAbsoluteStoresIndexRegister()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA2, 0x00, 0xCA, 0x8E, 0x78, 0x56, 0x00], 0x0500);

        cpu.Step();
        Assert.Equal(0x00, cpu.X);
        Assert.True(cpu.Zero);
        Assert.False(cpu.Negative);

        cpu.Step();
        Assert.Equal(0xFF, cpu.X);
        Assert.False(cpu.Zero);
        Assert.True(cpu.Negative);

        cpu.Step();

        Assert.Equal(0xFF, cpu.ReadByte(0x5678));
        Assert.False(cpu.IsHalted);
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

        var exception = Assert.Throws<BootstrapCpuStepLimitExceededException>(() => cpu.Run(1));

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
        Assert.False(cpu.Carry);
        Assert.Equal(0x01, cpu.ReadByte(0x2000));
        Assert.Equal(0x0407, cpu.PC);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void JsrAbsolutePushesReturnAddressAndJumpsToTarget()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x20, 0x34, 0x12, 0x00], 0x0600);

        cpu.Step();

        Assert.Equal(0x1234, cpu.PC);
        Assert.Equal(0xFD, cpu.SP);
        Assert.Equal(0x06, cpu.ReadByte(0x01FF));
        Assert.Equal(0x02, cpu.ReadByte(0x01FE));
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void RtsPopsReturnAddressAndResumesAtNextInstruction()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x20, 0x06, 0x06, 0x00, 0x00, 0x00, 0x60, 0x00], 0x0600);

        cpu.Step();
        cpu.Step();

        Assert.Equal(0x0603, cpu.PC);
        Assert.Equal(0xFF, cpu.SP);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void JsrAndRtsReturnToInstructionAfterCall()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x20, 0x06, 0x06, 0xA9, 0x42, 0x00, 0xA2, 0x05, 0x60], 0x0600);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x42, cpu.A);
        Assert.Equal(0x05, cpu.X);
        Assert.Equal(0xFF, cpu.SP);
        Assert.Equal(0x0606, cpu.PC);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void NestedSubroutinesPreserveStackDiscipline()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x20, 0x04, 0x06, 0x00, 0xA2, 0x01, 0x20, 0x0B, 0x06, 0xE8, 0x60, 0xA9, 0xAA, 0x60], 0x0600);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0xAA, cpu.A);
        Assert.Equal(0x02, cpu.X);
        Assert.Equal(0xFF, cpu.SP);
        Assert.Equal(0x0604, cpu.PC);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void JmpCmpBeqAndBneWorkTogetherForControlFlow()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0x4C, 0x06, 0x06, 0xA9, 0xFF, 0x00, 0xA9, 0x03, 0xC9, 0x03, 0xF0, 0x02, 0xA9, 0xFF, 0x00], 0x0600);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x03, cpu.A);
        Assert.True(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.True(cpu.Carry);
        Assert.Equal(0x060F, cpu.PC);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void BeqDoesNotBranchWhenZeroFlagIsClear()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x01, 0xC9, 0x02, 0xF0, 0x02, 0xA9, 0xFF, 0x00], 0x0700);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0xFF, cpu.A);
        Assert.False(cpu.Zero);
        Assert.True(cpu.Negative);
        Assert.False(cpu.Carry);
        Assert.Equal(0x0709, cpu.PC);
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
        Assert.False(cpu.Carry);
        Assert.Equal(0x0709, cpu.PC);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void BneDoesNotBranchWhenZeroFlagIsSet()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x01, 0xC9, 0x01, 0xD0, 0x02, 0xA9, 0xFF, 0x00], 0x0800);

        cpu.RunUntilHalt();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0xFF, cpu.A);
        Assert.False(cpu.Zero);
        Assert.True(cpu.Negative);
        Assert.True(cpu.Carry);
        Assert.Equal(0x0809, cpu.PC);
    }

    [Trait("Category", "ControlFlow")]
    [Fact]
    public void BneBranchesBackwardWithNegativeOffset()
    {
        var cpu = new BootstrapCpu();

        cpu.LoadProgram([0xA9, 0x01, 0xC9, 0x02, 0xD0, 0xFE, 0x00], 0x0900);

        cpu.RunSteps(3);

        Assert.False(cpu.IsHalted);
        Assert.False(cpu.Zero);
        Assert.True(cpu.Negative);
        Assert.False(cpu.Carry);
        Assert.Equal(0x0904, cpu.PC);
    }

}
