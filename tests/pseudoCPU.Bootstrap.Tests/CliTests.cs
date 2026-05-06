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
            Assert.Contains("BNE -5", output);
            Assert.Contains("STX $2000", output);
            Assert.Contains("Status: Halted", output);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }
}
