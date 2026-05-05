using pseudoCPU.Core;

namespace pseudoCPU.Bootstrap.Tests;

public class BootstrapAssemblerTests
{
    [Trait("Category", "Assembler")]
    [Fact]
    public void AssemblesBootstrapSliceProgramToExpectedBytes()
    {
        const string source = """
            ; set up the accumulator and store it
            LDA #$01
            TAX
            INX
            STA $1234
            BRK
            """;

        var bytes = BootstrapAssembler.Assemble(source);

        Assert.Equal([0xA9, 0x01, 0xAA, 0xE8, 0x8D, 0x34, 0x12, 0x00], bytes);
    }

    [Trait("Category", "AsmExecution")]
    [Fact]
    public void AssemblesAndRunsBootstrapProgramToExpectedCpuState()
    {
        const string source = """
            LDA #$2A
            TAX
            INX
            STA $2000
            BRK
            """;

        var cpu = new BootstrapCpu();
        cpu.LoadProgram(BootstrapAssembler.Assemble(source), 0x0400);
        cpu.Run();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x2A, cpu.A);
        Assert.Equal(0x2B, cpu.X);
        Assert.False(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.Equal(0x2A, cpu.ReadByte(0x2000));
        Assert.Equal(0x0408, cpu.PC);
    }
}
