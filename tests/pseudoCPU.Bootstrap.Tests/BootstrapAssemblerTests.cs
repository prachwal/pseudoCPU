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

    [Trait("Category", "Assembler")]
    [Theory]
    [InlineData("LDA $01", typeof(FormatException), "requires an operand starting with '#'")]
    [InlineData("STA #$1234", typeof(FormatException), "invalid hexadecimal literal")]
    [InlineData("LDA #$100", typeof(OverflowException), "does not fit in the expected size")]
    [InlineData("STA $10000", typeof(FormatException), "invalid hexadecimal literal")]
    [InlineData("NOP", typeof(NotSupportedException), "Unsupported bootstrap mnemonic")]
    [InlineData("LDA #", typeof(FormatException), "invalid hexadecimal literal")]
    [InlineData("STA", typeof(FormatException), "requires an operand")]
    public void RejectsInvalidBootstrapAssembly(string source, Type expectedException, string messageFragment)
    {
        var exception = Assert.Throws(expectedException, () => BootstrapAssembler.Assemble(source));

        Assert.Contains(messageFragment, exception.Message);
    }
}
