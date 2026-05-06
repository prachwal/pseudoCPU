using pseudoCPU.Cli;

namespace pseudoCPU.Bootstrap.Tests;

public class CliTests
{
    [Trait("Category", "Cli")]
    [Fact]
    public void ParsesTraceFlagAndRequiredOptions()
    {
        var options = CliOptionsParser.Parse(["--source", "program.asm", "--start", "0x0600", "--max-steps", "25", "--trace"]);

        Assert.Equal("program.asm", CliOptionsParser.RequireOption(options, "source"));
        Assert.Equal(0x0600, CliOptionsParser.ParseHexUShort(CliOptionsParser.RequireOption(options, "start")));
        Assert.Equal(25, CliOptionsParser.ParseIntOption(options, "max-steps", 10_000));
        Assert.True(options.ContainsKey("trace"));
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void ParserRejectsMissingOptionValue()
    {
        var exception = Assert.Throws<ArgumentException>(() => CliOptionsParser.Parse(["--source"]));

        Assert.Equal("Missing value for option '--source'.", exception.Message);
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void RequireOptionRejectsMissingSource()
    {
        var exception = Assert.Throws<ArgumentException>(() => CliOptionsParser.RequireOption(new Dictionary<string, string>(), "source"));

        Assert.Equal("Missing required option '--source'.", exception.Message);
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void ParseHexUShortRejectsInvalidStartValue()
    {
        Assert.Throws<FormatException>(() => CliOptionsParser.ParseHexUShort("not-a-hex-value"));
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void HelpMentionsSourceOption()
    {
        var originalOut = Console.Out;

        try
        {
            using var standardOut = new StringWriter();
            Console.SetOut(standardOut);

            var exitCode = CliApplication.Run(["--help"]);

            Assert.Equal(0, exitCode);
            Assert.Contains("--source <file.asm>", standardOut.ToString());
            Assert.Contains("--source <file.bin>", standardOut.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void RunAsmEmitsPerStepTraceAndFinalStatus()
    {
        var sourcePath = Path.GetTempFileName();
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            File.WriteAllText(sourcePath, "LDA #$01\nBRK\n");

            using var standardOut = new StringWriter();
            using var standardError = new StringWriter();
            Console.SetOut(standardOut);
            Console.SetError(standardError);

            var exitCode = CliApplication.Run(["run-asm", "--source", sourcePath, "--start", "0x0400", "--max-steps", "10", "--trace"]);

            Assert.Equal(0, exitCode);
            Assert.Empty(standardError.ToString());
            var output = standardOut.ToString();
            Assert.Contains("#0001 PC=0x0400 OPC=0xA9 LDA #$01", output);
            Assert.Contains("C=false", output);
            Assert.Contains("#0002 PC=0x0402 OPC=0x00 BRK", output);
            Assert.Contains("Status: Halted", output);
            Assert.Contains("Steps: 2", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
            File.Delete(sourcePath);
        }
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void RunAsmExampleEmitsTraceForXCounterLoopInstructions()
    {
        var examplePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "examples", "x-counter-loop.asm"));
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            Assert.True(File.Exists(examplePath), examplePath);

            using var standardOut = new StringWriter();
            using var standardError = new StringWriter();
            Console.SetOut(standardOut);
            Console.SetError(standardError);

            var exitCode = CliApplication.Run(["run-asm", "--source", examplePath, "--start", "0x0600", "--max-steps", "100", "--trace"]);

            Assert.Equal(0, exitCode);
            Assert.Empty(standardError.ToString());
            var output = standardOut.ToString();
            Assert.Contains("LDX #$03", output);
            Assert.Contains("DEX", output);
            Assert.Contains("CPX #$00", output);
            Assert.Contains("BNE $FB", output);
            Assert.Contains("STX $2000", output);
            Assert.Contains("Y: 0x00", output);
            Assert.Contains("Status: Halted", output);
            Assert.Contains("Carry: true", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void RunAsmEmitsTraceForYCounterLoopInstructions()
    {
        var examplePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "examples", "y-counter-loop.asm"));
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            Assert.True(File.Exists(examplePath), examplePath);

            using var standardOut = new StringWriter();
            using var standardError = new StringWriter();
            Console.SetOut(standardOut);
            Console.SetError(standardError);

            var exitCode = CliApplication.Run(["run-asm", "--source", examplePath, "--start", "0x0600", "--max-steps", "100", "--trace"]);

            Assert.Equal(0, exitCode);
            Assert.Empty(standardError.ToString());
            var output = standardOut.ToString();
            Assert.Contains("LDY #$03", output);
            Assert.Contains("INY", output);
            Assert.Contains("DEY", output);
            Assert.Contains("CPY #$00", output);
            Assert.Contains("BNE $FB", output);
            Assert.Contains("STY $2000", output);
            Assert.Contains("Y=0x03", output);
            Assert.Contains("Y: 0x00", output);
            Assert.Contains("Status: Halted", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void RunAsmEmitsTraceForStackOpcodes()
    {
        var examplePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "examples", "stack-opcodes.asm"));
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            Assert.True(File.Exists(examplePath), examplePath);

            using var standardOut = new StringWriter();
            using var standardError = new StringWriter();
            Console.SetOut(standardOut);
            Console.SetError(standardError);

            var exitCode = CliApplication.Run(["run-asm", "--source", examplePath, "--start", "0x0600", "--max-steps", "100", "--trace"]);

            Assert.Equal(0, exitCode);
            Assert.Empty(standardError.ToString());
            var output = standardOut.ToString();
            Assert.Contains("PHA", output);
            Assert.Contains("PHP", output);
            Assert.Contains("PLP", output);
            Assert.Contains("PLA", output);
            Assert.Contains("Status: Halted", output);
            Assert.Contains("Carry: true", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void RunAsmEmitsTraceForStatusAndArithmeticSliceInstructions()
    {
        var sourcePath = Path.GetTempFileName();
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            File.WriteAllText(sourcePath, "LDA #$40\nSTA $0010\nCLC\nADC #$01\nSEC\nSBC #$01\nBIT $10\nASL A\nLSR A\nROL A\nROR A\nBRK\n");

            using var standardOut = new StringWriter();
            using var standardError = new StringWriter();
            Console.SetOut(standardOut);
            Console.SetError(standardError);

            var exitCode = CliApplication.Run(["run-asm", "--source", sourcePath, "--start", "0x0400", "--max-steps", "20", "--trace"]);

            Assert.Equal(0, exitCode);
            Assert.Empty(standardError.ToString());
            var output = standardOut.ToString();
            Assert.Contains("CLC", output);
            Assert.Contains("ADC #$01", output);
            Assert.Contains("SEC", output);
            Assert.Contains("SBC #$01", output);
            Assert.Contains("BIT $10", output);
            Assert.Contains("ASL A", output);
            Assert.Contains("LSR A", output);
            Assert.Contains("ROL A", output);
            Assert.Contains("ROR A", output);
            Assert.Contains("P=0x", output);
            Assert.Contains("V=", output);
            Assert.Contains("Status: Halted", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
            File.Delete(sourcePath);
        }
    }

    [Trait("Category", "Cli")]
    [Fact]
    public void RunAsmEmitsTraceForJsrAndRtsInstructions()
    {
        var sourcePath = Path.GetTempFileName();
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            File.WriteAllText(sourcePath, "JSR $0406\nLDA #$2A\nBRK\nLDX #$05\nRTS\n");

            using var standardOut = new StringWriter();
            using var standardError = new StringWriter();
            Console.SetOut(standardOut);
            Console.SetError(standardError);

            var exitCode = CliApplication.Run(["run-asm", "--source", sourcePath, "--start", "0x0400", "--max-steps", "10", "--trace"]);

            Assert.Equal(0, exitCode);
            Assert.Empty(standardError.ToString());
            var output = standardOut.ToString();
            Assert.Contains("JSR $0406", output);
            Assert.Contains("RTS", output);
            Assert.Contains("Status: Halted", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
            File.Delete(sourcePath);
        }
    }
}
