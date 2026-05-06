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
    [Fact]
    public void AssemblesStackOpcodesToExpectedBytes()
    {
        const string source = """
            PHA
            PLA
            PHP
            PLP
            """;

        var bytes = BootstrapAssembler.Assemble(source);

        Assert.Equal([0x48, 0x68, 0x08, 0x28], bytes);
    }

    [Trait("Category", "AsmExecution")]
    [Fact]
    public void AssemblesAndRunsStackOpcodeProgramToExpectedCpuState()
    {
        const string source = """
            LDA #$2A
            PHA
            LDA #$00
            CMP #$00
            PHP
            LDA #$00
            CMP #$01
            PLP
            PLA
            BRK
            """;

        var cpu = new BootstrapCpu();
        cpu.LoadProgram(BootstrapAssembler.Assemble(source), 0x0400);
        cpu.Run();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x2A, cpu.A);
        Assert.True(cpu.Carry);
        Assert.False(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.Equal(0xFF, cpu.SP);
        Assert.Equal(0x040F, cpu.PC);
    }

    [Trait("Category", "Assembler")]
    [Fact]
    public void AssemblesJsrAndRtsToExpectedBytes()
    {
        const string source = """
            JSR $1234
            RTS
            """;

        var bytes = BootstrapAssembler.Assemble(source);

        Assert.Equal([0x20, 0x34, 0x12, 0x60], bytes);
    }

    [Trait("Category", "AsmExecution")]
    [Fact]
    public void AssemblesAndRunsSubroutineProgramToExpectedCpuState()
    {
        const string source = """
            JSR $0406
            LDA #$2A
            BRK
            LDX #$05
            RTS
            """;

        var cpu = new BootstrapCpu();
        cpu.LoadProgram(BootstrapAssembler.Assemble(source), 0x0400);
        cpu.Run();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x2A, cpu.A);
        Assert.Equal(0x05, cpu.X);
        Assert.Equal(0xFF, cpu.SP);
        Assert.Equal(0x0406, cpu.PC);
    }


    [Trait("Category", "ControlFlow")]
    [Fact]
    public void AssemblesControlFlowSliceToExpectedBytes()
    {
        const string source = """
            LDA #$03
            CMP #$03
            BEQ $02
            BNE $FE
            JMP $1234
            BRK
            """;

        var bytes = BootstrapAssembler.Assemble(source);

        Assert.Equal([0xA9, 0x03, 0xC9, 0x03, 0xF0, 0x02, 0xD0, 0xFE, 0x4C, 0x34, 0x12, 0x00], bytes);
    }

    [Trait("Category", "Assembler")]
    [Fact]
    public void AssemblesStatusAndArithmeticSliceToExpectedBytes()
    {
        const string source = """
            CLC
            SEC
            CLI
            SEI
            CLD
            SED
            CLV
            ADC #$01
            SBC #$02
            BIT $10
            ASL A
            LSR A
            ROL A
            ROR A
            BPL $02
            BMI $FE
            BVC $02
            BVS $FE
            BCC $02
            BCS $FE
            BRK
            """;

        var bytes = BootstrapAssembler.Assemble(source);

        Assert.Equal([0x18, 0x38, 0x58, 0x78, 0xD8, 0xF8, 0xB8, 0x69, 0x01, 0xE9, 0x02, 0x24, 0x10, 0x0A, 0x4A, 0x2A, 0x6A, 0x10, 0x02, 0x30, 0xFE, 0x50, 0x02, 0x70, 0xFE, 0x90, 0x02, 0xB0, 0xFE, 0x00], bytes);
    }

    [Trait("Category", "Assembler")]
    [Fact]
    public void AssemblesXCounterSliceProgramToExpectedBytes()
    {
        const string source = """
            LDX #$03
            DEX
            CPX #$00
            BNE $FB
            STX $2000
            BRK
            """;

        var bytes = BootstrapAssembler.Assemble(source);

        Assert.Equal([0xA2, 0x03, 0xCA, 0xE0, 0x00, 0xD0, 0xFB, 0x8E, 0x00, 0x20, 0x00], bytes);
    }

    [Trait("Category", "Assembler")]
    [Fact]
    public void AssemblesYSliceProgramToExpectedBytes()
    {
        const string source = """
            LDY #$03
            INY
            DEY
            CPY #$00
            BNE $FB
            STY $2000
            BRK
            """;

        var bytes = BootstrapAssembler.Assemble(source);

        Assert.Equal([0xA0, 0x03, 0xC8, 0x88, 0xC0, 0x00, 0xD0, 0xFB, 0x8C, 0x00, 0x20, 0x00], bytes);
    }

    [Trait("Category", "AsmExecution")]
    [Fact]
    public void AssemblesAndRunsXCounterProgramToExpectedCpuState()
    {
        // Raw branch offsets only; labels are intentionally unsupported.
        const string source = """
            LDX #$03
            DEX
            CPX #$00
            BNE $FB
            STX $2000
            BRK
            """;

        var cpu = new BootstrapCpu();
        cpu.LoadProgram(BootstrapAssembler.Assemble(source), 0x0400);
        cpu.Run();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x00, cpu.X);
        Assert.True(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.True(cpu.Carry);
        Assert.Equal(0x00, cpu.ReadByte(0x2000));
        Assert.Equal(0x040B, cpu.PC);
    }

    [Trait("Category", "AsmExecution")]
    [Fact]
    public void AssemblesAndRunsYSliceProgramToExpectedCpuState()
    {
        var examplePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "examples", "y-counter-loop.asm"));
        var source = File.ReadAllText(examplePath);

        var cpu = new BootstrapCpu();
        cpu.LoadProgram(BootstrapAssembler.Assemble(source), 0x0400);
        cpu.Run();

        Assert.True(cpu.IsHalted);
        Assert.Equal(0x00, cpu.Y);
        Assert.True(cpu.Zero);
        Assert.False(cpu.Negative);
        Assert.True(cpu.Carry);
        Assert.Equal(0x00, cpu.ReadByte(0x2000));
        Assert.Equal(0x040C, cpu.PC);
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
