using pseudoCPU.Core;

namespace pseudoCPU.Cli;

internal static class CliApplication
{
    public static int Run(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h")
        {
            CliHelpPrinter.Print();
            return 0;
        }

        var command = args[0];

        try
        {
            var options = CliOptionsParser.Parse(args[1..]);

            switch (command)
            {
                case "run-asm":
                    RunAssemblerProgram(options);
                    return 0;
                case "run-bin":
                    RunBinaryProgram(options);
                    return 0;
                default:
                    Console.Error.WriteLine($"Unknown command '{command}'.");
                    return 1;
            }
        }
        catch (Exception exception) when (exception is FormatException or NotSupportedException or ArgumentException or ArgumentOutOfRangeException or OverflowException)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static void RunAssemblerProgram(IReadOnlyDictionary<string, string> options)
    {
        var sourcePath = CliOptionsParser.RequireOption(options, "source");
        var startAddress = CliOptionsParser.ParseHexUShort(CliOptionsParser.RequireOption(options, "start"));
        var maxSteps = CliOptionsParser.ParseIntOption(options, "max-steps", 10_000);
        var trace = options.ContainsKey("trace");

        var source = File.ReadAllText(sourcePath);
        var program = BootstrapAssembler.Assemble(source);
        ExecuteProgram(program, startAddress, maxSteps, trace);
    }

    private static void RunBinaryProgram(IReadOnlyDictionary<string, string> options)
    {
        var binaryPath = CliOptionsParser.RequireOption(options, "source");
        var startAddress = CliOptionsParser.ParseHexUShort(CliOptionsParser.RequireOption(options, "start"));
        var maxSteps = CliOptionsParser.ParseIntOption(options, "max-steps", 10_000);
        var trace = options.ContainsKey("trace");

        var program = File.ReadAllBytes(binaryPath);
        ExecuteProgram(program, startAddress, maxSteps, trace);
    }

    private static void ExecuteProgram(byte[] program, ushort startAddress, int maxSteps, bool trace)
    {
        if (maxSteps <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSteps));
        }

        var cpu = new BootstrapCpu();
        cpu.LoadProgram(program, startAddress);

        var steps = 0;
        while (steps < maxSteps && !cpu.IsHalted)
        {
            var pc = cpu.PC;
            var opcode = cpu.ReadByte(pc);

            cpu.Step();
            steps++;

            if (trace)
            {
                Console.WriteLine(CliTraceFormatter.FormatStep(steps, pc, opcode, cpu));
            }
        }

        var status = cpu.IsHalted ? "Halted" : "StepLimitReached";

        Console.WriteLine($"Status: {status}");
        Console.WriteLine($"Steps: {steps}");
        Console.WriteLine($"PC: 0x{cpu.PC:X4}");
        Console.WriteLine($"A: 0x{cpu.A:X2}");
        Console.WriteLine($"X: 0x{cpu.X:X2}");
        Console.WriteLine($"Y: 0x{cpu.Y:X2}");
        Console.WriteLine($"Zero: {cpu.Zero.ToString().ToLowerInvariant()}");
        Console.WriteLine($"Negative: {cpu.Negative.ToString().ToLowerInvariant()}");
        Console.WriteLine($"Carry: {cpu.Carry.ToString().ToLowerInvariant()}");
    }
}
